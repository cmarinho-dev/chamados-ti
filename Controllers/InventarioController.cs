using ChamadosTI.Data;
using ChamadosTI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ChamadosTI.Controllers;

[Route("admin/inventario")]
public class InventarioController : Controller
{
    private const string ChaveSessao = "AdminAutenticado";
    private readonly ContextoChamados _db;

    public InventarioController(ContextoChamados db)
    {
        _db = db;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Computadores));
    }

    [HttpGet("relatorios")]
    public async Task<IActionResult> Relatorios([FromQuery] InventarioRelatoriosViewModel filtro, string? export)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        filtro.Aba = string.IsNullOrWhiteSpace(filtro.Aba) ? "geral" : filtro.Aba.ToLowerInvariant();
        if (filtro.SomenteBackup)
        {
            filtro.OcultarBackups = false;
        }

        var query = _db.InventarioItems
            .Include(i => i.Setor)
            .Include(i => i.Monitores)
            .Include(i => i.ChavesLicencas)
            .Include(i => i.ArmazenamentosQuantidades)
            .AsQueryable();

        if (filtro.Aba == "computadores")
        {
            query = query.Where(i => i.TipoEquipamento == InventarioTipoEquipamento.Computador);
        }
        else if (filtro.Aba == "notebooks")
        {
            query = query.Where(i => i.TipoEquipamento == InventarioTipoEquipamento.Notebook);
        }
        else if (filtro.TipoEquipamento.HasValue)
        {
            query = query.Where(i => i.TipoEquipamento == filtro.TipoEquipamento.Value);
        }

        if (filtro.SetorId.HasValue)
        {
            query = query.Where(i => i.SetorId == filtro.SetorId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Responsavel))
        {
            var responsavel = filtro.Responsavel.Trim();
            query = query.Where(i => i.PessoaResponsavel != null && i.PessoaResponsavel.Contains(responsavel));
        }

        if (filtro.SomenteBackup)
        {
            query = query.Where(i => i.EhBackup);
        }
        else if (filtro.OcultarBackups)
        {
            query = query.Where(i => !i.EhBackup);
        }

        if (filtro.SemPatrimonio)
        {
            query = query.Where(i => i.Patrimonio == null || i.Patrimonio == "");
        }

        if (filtro.SemResponsavel)
        {
            query = query.Where(i => i.PessoaResponsavel == null || i.PessoaResponsavel == "");
        }

        if (filtro.ComMonitor)
        {
            query = query.Where(i => i.Monitores.Any());
        }

        if (filtro.ComChave)
        {
            query = query.Where(i => i.ChavesLicencas.Any());
        }

        if (filtro.TipoChave.HasValue)
        {
            query = query.Where(i => i.ChavesLicencas.Any(c => c.Tipo == filtro.TipoChave.Value));
        }

        var itens = await query
            .OrderBy(i => i.Setor!.Nome)
            .ThenBy(i => i.InventarioNumero)
            .ToListAsync();

        if (string.Equals(export, "csv", StringComparison.OrdinalIgnoreCase))
        {
            return File(GerarCsv(itens), "text/csv; charset=utf-8", "relatorio-equipamentos.csv");
        }

        filtro.TotalEquipamentos = itens.Count;
        filtro.TotalComputadores = itens.Count(i => i.TipoEquipamento == InventarioTipoEquipamento.Computador);
        filtro.TotalNotebooks = itens.Count(i => i.TipoEquipamento == InventarioTipoEquipamento.Notebook);
        filtro.TotalOutros = itens.Count(i => i.TipoEquipamento != InventarioTipoEquipamento.Computador && i.TipoEquipamento != InventarioTipoEquipamento.Notebook);
        filtro.TotalMonitoresVinculados = itens.Sum(i => i.Monitores.Count);
        filtro.TotalSemResponsavel = itens.Count(i => string.IsNullOrWhiteSpace(i.PessoaResponsavel));
        filtro.TotalSemPatrimonio = itens.Count(i => string.IsNullOrWhiteSpace(i.Patrimonio));

        filtro.Linhas = itens.Select(i => new InventarioRelatorioLinhaViewModel
        {
            Id = i.Id,
            InventarioNumero = i.InventarioNumero,
            Patrimonio = i.Patrimonio,
            TipoEquipamento = DescreverTipoEquipamento(i.TipoEquipamento),
            TipoEquipamentoValor = i.TipoEquipamento,
            Setor = i.Setor?.Nome,
            Responsavel = i.PessoaResponsavel,
            EhBackup = i.EhBackup,
            Monitores = i.Monitores.Count,
            Chaves = i.ChavesLicencas.Count,
            Armazenamentos = i.ArmazenamentosQuantidades.Sum(a => a.Quantidade),
            Observacao = i.Observacao,
            Ip = i.Ip
        }).ToList();

        filtro.Conformidade = MontarConformidade(itens);
        filtro.Licencas = await MontarLicencasAsync();
        filtro.Monitores = await MontarMonitoresAsync();

        await PopularListasRelatoriosAsync(filtro);
        return View(filtro);
    }

    [HttpGet("computadores")]
    public async Task<IActionResult> Computadores(int pagina = 1, int itensPorPagina = 5, string? busca = null)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var pageSize = Math.Clamp(itensPorPagina, 1, 100);
        var termoBusca = busca?.Trim() ?? string.Empty;
        var query = _db.InventarioItems
            .Include(i => i.Setor)
            .Include(i => i.SistemaOperacional)
            .Include(i => i.Office)
            .Include(i => i.Antivirus)
            .Include(i => i.MemoriasQuantidades)
                .ThenInclude(i => i.Memoria)
            .Include(i => i.Processadores)
            .Include(i => i.ArmazenamentosQuantidades)
                .ThenInclude(i => i.Armazenamento)
            .Include(i => i.Monitores)
            .Include(i => i.PerifericosQuantidades)
                .ThenInclude(i => i.Periferico)
            .Include(i => i.CabosQuantidades)
                .ThenInclude(i => i.Cabo)
            .Include(i => i.ChavesLicencas)
            .Where(i => i.TipoEquipamento == InventarioTipoEquipamento.Computador || i.TipoEquipamento == InventarioTipoEquipamento.Notebook)
            .AsQueryable();

        if (termoBusca.Length > 0)
        {
            query = query.Where(i =>
                i.InventarioNumero.Contains(termoBusca) ||
                (i.Patrimonio != null && i.Patrimonio.Contains(termoBusca)) ||
                (i.PessoaResponsavel != null && i.PessoaResponsavel.Contains(termoBusca)) ||
                (i.Setor != null && i.Setor.Nome.Contains(termoBusca)) ||
                (i.Ip != null && i.Ip.Contains(termoBusca)) ||
                (i.Observacao != null && i.Observacao.Contains(termoBusca)) ||
                i.Processadores.Any(c => c.Descricao.Contains(termoBusca)) ||
                i.MemoriasQuantidades.Any(c => c.Memoria != null && c.Memoria.Descricao.Contains(termoBusca)) ||
                i.ArmazenamentosQuantidades.Any(c => c.Armazenamento != null && c.Armazenamento.Descricao.Contains(termoBusca)) ||
                i.Monitores.Any(c =>
                    c.InventarioNumero.Contains(termoBusca) ||
                    (c.Patrimonio != null && c.Patrimonio.Contains(termoBusca)) ||
                    (c.Marca != null && c.Marca.Contains(termoBusca)) ||
                    (c.Modelo != null && c.Modelo.Contains(termoBusca))));
        }

        var totalItens = await query.CountAsync();
        var totalPaginas = totalItens == 0 ? 1 : (int)Math.Ceiling(totalItens / (double)pageSize);
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var itens = await query
            .OrderBy(i => i.Setor!.Nome)
            .ThenBy(i => i.InventarioNumero)
            .Skip((pagina - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new InventarioComputadoresViewModel
        {
            Itens = itens,
            Paginacao = new InventarioPaginacaoViewModel
            {
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                ItensPorPagina = pageSize,
                TotalItens = totalItens,
                Busca = termoBusca
            }
        };

        return View(model);
    }

    [HttpGet("computadores/novo")]
    public async Task<IActionResult> NovoComputador()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var model = new InventarioFormViewModel();
        await CarregarListasComputadorAsync(model);
        return View(model);
    }

    [HttpPost("computadores/novo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NovoComputador(InventarioFormViewModel model)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (model.TipoEquipamento != InventarioTipoEquipamento.Computador && model.TipoEquipamento != InventarioTipoEquipamento.Notebook)
        {
            ModelState.AddModelError(nameof(model.TipoEquipamento), "Escolha Computador ou Notebook.");
        }

        await ValidarRecursosExclusivosAsync(model, null);

        if (!ModelState.IsValid)
        {
            await CarregarListasComputadorAsync(model);
            return View(model);
        }

        var memoriasQuantidades = ObterQuantidadesSelecionadas(model.MemoriasComponentes);
        var armazenamentosQuantidades = ObterQuantidadesSelecionadas(model.ArmazenamentosComponentes);
        var perifericosQuantidades = ObterQuantidadesSelecionadas(model.PerifericosComponentes);
        var cabosQuantidades = ObterQuantidadesSelecionadas(model.CabosComponentes);

        var item = new InventarioItem
        {
            InventarioNumero = model.InventarioNumero.Trim(),
            Patrimonio = Limpar(model.Patrimonio),
            TipoEquipamento = model.TipoEquipamento,
            EhBackup = model.EhBackup,
            PessoaResponsavel = Limpar(model.PessoaResponsavel),
            SetorId = model.SetorId,
            SistemaOperacionalId = model.SistemaOperacionalId,
            OfficeId = model.OfficeId,
            AntivirusId = model.AntivirusId,
            ConexaoId = model.ConexaoId,
            Ip = Limpar(model.Ip),
            Observacao = Limpar(model.Observacao),
            CriadoEm = DateTimeOffset.UtcNow,
            Processadores = model.ProcessadorId.HasValue
                ? await _db.InventarioProcessadores.Where(c => c.Id == model.ProcessadorId.Value).ToListAsync()
                : new List<InventarioProcessador>(),
            Monitores = await _db.InventarioMonitores.Where(c => model.MonitorIds.Contains(c.Id)).ToListAsync(),
            MemoriasQuantidades = memoriasQuantidades.Select(kv => new InventarioItemMemoriaQuantidade { MemoriaId = kv.Key, Quantidade = kv.Value }).ToList(),
            ArmazenamentosQuantidades = armazenamentosQuantidades.Select(kv => new InventarioItemArmazenamentoQuantidade { ArmazenamentoId = kv.Key, Quantidade = kv.Value }).ToList(),
            PerifericosQuantidades = perifericosQuantidades.Select(kv => new InventarioItemPerifericoQuantidade { PerifericoId = kv.Key, Quantidade = kv.Value }).ToList(),
            CabosQuantidades = cabosQuantidades.Select(kv => new InventarioItemCaboQuantidade { CaboId = kv.Key, Quantidade = kv.Value }).ToList(),
            ChavesLicencas = await ObterChavesSelecionadasAsync(model)
        };

        _db.InventarioItems.Add(item);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Computador cadastrado com sucesso.";
        return RedirectToAction(nameof(Computadores));
    }

    [HttpGet("computadores/editar/{id:int}")]
    public async Task<IActionResult> EditarComputador(int id)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var item = await _db.InventarioItems
            .Include(i => i.MemoriasQuantidades)
            .Include(i => i.Processadores)
            .Include(i => i.ArmazenamentosQuantidades)
            .Include(i => i.Monitores)
            .Include(i => i.PerifericosQuantidades)
            .Include(i => i.CabosQuantidades)
            .Include(i => i.ChavesLicencas)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        if (item.TipoEquipamento != InventarioTipoEquipamento.Computador && item.TipoEquipamento != InventarioTipoEquipamento.Notebook)
        {
            return RedirectToAction(nameof(OutrosDispositivos));
        }

        var model = new InventarioFormViewModel
        {
            Id = item.Id,
            InventarioNumero = item.InventarioNumero,
            Patrimonio = item.Patrimonio,
            TipoEquipamento = item.TipoEquipamento,
            EhBackup = item.EhBackup,
            PessoaResponsavel = item.PessoaResponsavel,
            SetorId = item.SetorId,
            SistemaOperacionalId = item.SistemaOperacionalId,
            OfficeId = item.OfficeId,
            AntivirusId = item.AntivirusId,
            ConexaoId = item.ConexaoId,
            Ip = item.Ip,
            Observacao = item.Observacao,
            MemoriaIds = item.MemoriasQuantidades.Select(c => c.MemoriaId).ToList(),
            ProcessadorId = item.Processadores.Select(c => (int?)c.Id).FirstOrDefault(),
            ArmazenamentoIds = item.ArmazenamentosQuantidades.Select(c => c.ArmazenamentoId).ToList(),
            MonitorIds = item.Monitores.Select(c => c.Id).ToList(),
            PerifericoIds = item.PerifericosQuantidades.Select(c => c.PerifericoId).ToList(),
            CaboIds = item.CabosQuantidades.Select(c => c.CaboId).ToList(),
            MemoriasComponentes = item.MemoriasQuantidades.Select(c => new InventarioComponenteQuantidadeItemViewModel
            {
                Id = c.MemoriaId,
                Quantidade = c.Quantidade,
                Selecionado = true
            }).ToList(),
            ArmazenamentosComponentes = item.ArmazenamentosQuantidades.Select(c => new InventarioComponenteQuantidadeItemViewModel
            {
                Id = c.ArmazenamentoId,
                Quantidade = c.Quantidade,
                Selecionado = true
            }).ToList(),
            PerifericosComponentes = item.PerifericosQuantidades.Select(c => new InventarioComponenteQuantidadeItemViewModel
            {
                Id = c.PerifericoId,
                Quantidade = c.Quantidade,
                Selecionado = true
            }).ToList(),
            CabosComponentes = item.CabosQuantidades.Select(c => new InventarioComponenteQuantidadeItemViewModel
            {
                Id = c.CaboId,
                Quantidade = c.Quantidade,
                Selecionado = true
            }).ToList(),
            ChaveWindowsIds = item.ChavesLicencas.Where(c => c.Tipo == InventarioChaveTipo.Windows).Select(c => c.Id).ToList(),
            ChaveOfficeIds = item.ChavesLicencas.Where(c => c.Tipo == InventarioChaveTipo.Office).Select(c => c.Id).ToList(),
            ChaveAntivirusIds = item.ChavesLicencas.Where(c => c.Tipo == InventarioChaveTipo.Antivirus).Select(c => c.Id).ToList(),
            ChaveOutrosIds = item.ChavesLicencas.Where(c => c.Tipo == InventarioChaveTipo.Outros).Select(c => c.Id).ToList()
        };

        await CarregarListasComputadorAsync(model);
        return View(model);
    }

    [HttpPost("computadores/editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarComputador(int id, InventarioFormViewModel model)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (model.TipoEquipamento != InventarioTipoEquipamento.Computador && model.TipoEquipamento != InventarioTipoEquipamento.Notebook)
        {
            ModelState.AddModelError(nameof(model.TipoEquipamento), "Escolha Computador ou Notebook.");
        }

        await ValidarRecursosExclusivosAsync(model, id);

        if (!ModelState.IsValid)
        {
            await CarregarListasComputadorAsync(model);
            return View(model);
        }

        var memoriasQuantidades = ObterQuantidadesSelecionadas(model.MemoriasComponentes);
        var armazenamentosQuantidades = ObterQuantidadesSelecionadas(model.ArmazenamentosComponentes);
        var perifericosQuantidades = ObterQuantidadesSelecionadas(model.PerifericosComponentes);
        var cabosQuantidades = ObterQuantidadesSelecionadas(model.CabosComponentes);

        var item = await _db.InventarioItems
            .Include(i => i.MemoriasQuantidades)
            .Include(i => i.Processadores)
            .Include(i => i.ArmazenamentosQuantidades)
            .Include(i => i.Monitores)
            .Include(i => i.PerifericosQuantidades)
            .Include(i => i.CabosQuantidades)
            .Include(i => i.ChavesLicencas)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        item.InventarioNumero = model.InventarioNumero.Trim();
        item.Patrimonio = Limpar(model.Patrimonio);
        item.TipoEquipamento = model.TipoEquipamento;
        item.EhBackup = model.EhBackup;
        item.PessoaResponsavel = Limpar(model.PessoaResponsavel);
        item.SetorId = model.SetorId;
        item.SistemaOperacionalId = model.SistemaOperacionalId;
        item.OfficeId = model.OfficeId;
        item.AntivirusId = model.AntivirusId;
        item.ConexaoId = model.ConexaoId;
        item.Ip = Limpar(model.Ip);
        item.Observacao = Limpar(model.Observacao);
        item.AtualizadoEm = DateTimeOffset.UtcNow;

        item.Processadores = model.ProcessadorId.HasValue
            ? await _db.InventarioProcessadores.Where(c => c.Id == model.ProcessadorId.Value).ToListAsync()
            : new List<InventarioProcessador>();
        item.Monitores = await _db.InventarioMonitores.Where(c => model.MonitorIds.Contains(c.Id)).ToListAsync();
        item.MemoriasQuantidades = memoriasQuantidades.Select(kv => new InventarioItemMemoriaQuantidade { InventarioItemId = item.Id, MemoriaId = kv.Key, Quantidade = kv.Value }).ToList();
        item.ArmazenamentosQuantidades = armazenamentosQuantidades.Select(kv => new InventarioItemArmazenamentoQuantidade { InventarioItemId = item.Id, ArmazenamentoId = kv.Key, Quantidade = kv.Value }).ToList();
        item.PerifericosQuantidades = perifericosQuantidades.Select(kv => new InventarioItemPerifericoQuantidade { InventarioItemId = item.Id, PerifericoId = kv.Key, Quantidade = kv.Value }).ToList();
        item.CabosQuantidades = cabosQuantidades.Select(kv => new InventarioItemCaboQuantidade { InventarioItemId = item.Id, CaboId = kv.Key, Quantidade = kv.Value }).ToList();
        item.ChavesLicencas = await ObterChavesSelecionadasAsync(model);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Computador atualizado com sucesso.";
        return RedirectToAction(nameof(Computadores));
    }

    [HttpPost("computadores/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirComputador(int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var item = await _db.InventarioItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        _db.InventarioItems.Remove(item);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Computador removido.";
        return RedirectToAction(nameof(Computadores));
    }

    [HttpGet("outros-dispositivos")]
    public async Task<IActionResult> OutrosDispositivos(int pagina = 1, int itensPorPagina = 5, string? busca = null)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var pageSize = Math.Clamp(itensPorPagina, 1, 100);
        var termoBusca = busca?.Trim() ?? string.Empty;
        var query = _db.InventarioItems
            .Include(i => i.Setor)
            .Where(i => i.TipoEquipamento == InventarioTipoEquipamento.Tablet
                || i.TipoEquipamento == InventarioTipoEquipamento.TV
                || i.TipoEquipamento == InventarioTipoEquipamento.Projetor
                || i.TipoEquipamento == InventarioTipoEquipamento.Impressora)
            .AsQueryable();

        if (termoBusca.Length > 0)
        {
            var termoTipo = termoBusca.ToLowerInvariant();
            var buscaPorTablet = "tablet".Contains(termoTipo) || termoTipo.Contains("tablet");
            var buscaPorTv = "tv".Contains(termoTipo) || termoTipo.Contains("tv");
            var buscaPorProjetor = "projetor".Contains(termoTipo);
            var buscaPorImpressora = "impressora".Contains(termoTipo);

            query = query.Where(i =>
                i.InventarioNumero.Contains(termoBusca) ||
                (i.Patrimonio != null && i.Patrimonio.Contains(termoBusca)) ||
                (i.PessoaResponsavel != null && i.PessoaResponsavel.Contains(termoBusca)) ||
                (i.Setor != null && i.Setor.Nome.Contains(termoBusca)) ||
                (i.Observacao != null && i.Observacao.Contains(termoBusca)) ||
                (buscaPorTablet && i.TipoEquipamento == InventarioTipoEquipamento.Tablet) ||
                (buscaPorTv && i.TipoEquipamento == InventarioTipoEquipamento.TV) ||
                (buscaPorProjetor && i.TipoEquipamento == InventarioTipoEquipamento.Projetor) ||
                (buscaPorImpressora && i.TipoEquipamento == InventarioTipoEquipamento.Impressora));
        }

        var totalItens = await query.CountAsync();
        var totalPaginas = totalItens == 0 ? 1 : (int)Math.Ceiling(totalItens / (double)pageSize);
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var itens = await query
            .OrderBy(i => i.TipoEquipamento)
            .ThenBy(i => i.InventarioNumero)
            .Skip((pagina - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new InventarioOutrosViewModel
        {
            Dispositivos = itens,
            Paginacao = new InventarioPaginacaoViewModel
            {
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                ItensPorPagina = pageSize,
                TotalItens = totalItens,
                Busca = termoBusca
            }
        };

        return View(model);
    }

    [HttpGet("monitores")]
    public async Task<IActionResult> Monitores(int pagina = 1, int itensPorPagina = 5, string? busca = null)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var pageSize = Math.Clamp(itensPorPagina, 1, 100);
        var termoBusca = busca?.Trim() ?? string.Empty;
        var query = _db.InventarioMonitores
            .Include(m => m.InventarioItems)
            .AsQueryable();

        if (termoBusca.Length > 0)
        {
            query = query.Where(m =>
                m.InventarioNumero.Contains(termoBusca) ||
                (m.Patrimonio != null && m.Patrimonio.Contains(termoBusca)) ||
                (m.Marca != null && m.Marca.Contains(termoBusca)) ||
                (m.Modelo != null && m.Modelo.Contains(termoBusca)) ||
                (m.Polegadas != null && m.Polegadas.Contains(termoBusca)) ||
                (m.Observacao != null && m.Observacao.Contains(termoBusca)));
        }

        var totalItens = await query.CountAsync();
        var totalPaginas = totalItens == 0 ? 1 : (int)Math.Ceiling(totalItens / (double)pageSize);
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var monitores = await query
            .OrderBy(m => m.InventarioNumero)
            .Skip((pagina - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new InventarioMonitoresViewModel
        {
            Monitores = monitores,
            Paginacao = new InventarioPaginacaoViewModel
            {
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                ItensPorPagina = pageSize,
                TotalItens = totalItens,
                Busca = termoBusca
            }
        };

        return View(model);
    }

    [HttpGet("outros-dispositivos/novo")]
    public async Task<IActionResult> NovoOutroDispositivo()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var model = new InventarioOutrosDispositivoFormViewModel
        {
            TipoEquipamento = InventarioTipoEquipamento.Tablet
        };

        await CarregarListasOutrosAsync(model);
        return View(model);
    }

    [HttpPost("outros-dispositivos/novo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NovoOutroDispositivo(InventarioOutrosDispositivoFormViewModel model)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (!TipoEhOutro(model.TipoEquipamento))
        {
            ModelState.AddModelError(nameof(model.TipoEquipamento), "Escolha Tablet, TV, Projetor ou Impressora.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarListasOutrosAsync(model);
            return View(model);
        }

        var item = new InventarioItem
        {
            InventarioNumero = model.InventarioNumero.Trim(),
            Patrimonio = Limpar(model.Patrimonio),
            TipoEquipamento = model.TipoEquipamento,
            PessoaResponsavel = Limpar(model.PessoaResponsavel),
            SetorId = model.SetorId,
            Observacao = Limpar(model.Observacao),
            CriadoEm = DateTimeOffset.UtcNow
        };

        _db.InventarioItems.Add(item);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Dispositivo cadastrado com sucesso.";
        return RedirectToAction(nameof(OutrosDispositivos));
    }

    [HttpGet("outros-dispositivos/editar/{id:int}")]
    public async Task<IActionResult> EditarOutroDispositivo(int id)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var item = await _db.InventarioItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        if (!TipoEhOutro(item.TipoEquipamento))
        {
            return RedirectToAction(nameof(Computadores));
        }

        var model = new InventarioOutrosDispositivoFormViewModel
        {
            Id = item.Id,
            InventarioNumero = item.InventarioNumero,
            Patrimonio = item.Patrimonio,
            TipoEquipamento = item.TipoEquipamento,
            PessoaResponsavel = item.PessoaResponsavel,
            SetorId = item.SetorId,
            Observacao = item.Observacao
        };

        await CarregarListasOutrosAsync(model);
        return View(model);
    }

    [HttpPost("outros-dispositivos/editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarOutroDispositivo(int id, InventarioOutrosDispositivoFormViewModel model)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (!TipoEhOutro(model.TipoEquipamento))
        {
            ModelState.AddModelError(nameof(model.TipoEquipamento), "Escolha Tablet, TV, Projetor ou Impressora.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarListasOutrosAsync(model);
            return View(model);
        }

        var item = await _db.InventarioItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        item.InventarioNumero = model.InventarioNumero.Trim();
        item.Patrimonio = Limpar(model.Patrimonio);
        item.TipoEquipamento = model.TipoEquipamento;
        item.PessoaResponsavel = Limpar(model.PessoaResponsavel);
        item.SetorId = model.SetorId;
        item.Observacao = Limpar(model.Observacao);
        item.AtualizadoEm = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Dispositivo atualizado com sucesso.";
        return RedirectToAction(nameof(OutrosDispositivos));
    }

    [HttpPost("outros-dispositivos/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirOutroDispositivo(int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var item = await _db.InventarioItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        _db.InventarioItems.Remove(item);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Dispositivo removido.";
        return RedirectToAction(nameof(OutrosDispositivos));
    }

    [HttpGet("monitores/novo")]
    [HttpGet("outros-dispositivos/monitores/novo")]
    public IActionResult NovoMonitor()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        return View(new InventarioMonitorFormViewModel());
    }

    [HttpPost("monitores/novo")]
    [HttpPost("outros-dispositivos/monitores/novo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NovoMonitor(InventarioMonitorFormViewModel model)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var inventarioNumero = model.InventarioNumero.Trim();
        var existe = await _db.InventarioMonitores.AnyAsync(m => m.InventarioNumero == inventarioNumero);
        if (existe)
        {
            ModelState.AddModelError(nameof(model.InventarioNumero), "Já existe monitor com este número de inventário.");
            return View(model);
        }

        _db.InventarioMonitores.Add(new InventarioMonitor
        {
            InventarioNumero = inventarioNumero,
            Patrimonio = Limpar(model.Patrimonio),
            Marca = Limpar(model.Marca),
            Modelo = Limpar(model.Modelo),
            Polegadas = Limpar(model.Polegadas),
            Observacao = Limpar(model.Observacao),
            CriadoEm = DateTimeOffset.UtcNow
        });

        await _db.SaveChangesAsync();
        TempData["Success"] = "Monitor cadastrado com sucesso.";
        return RedirectToAction(nameof(Monitores));
    }

    [HttpGet("monitores/editar/{id:int}")]
    [HttpGet("outros-dispositivos/monitores/editar/{id:int}")]
    public async Task<IActionResult> EditarMonitor(int id)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var monitor = await _db.InventarioMonitores.FirstOrDefaultAsync(m => m.Id == id);
        if (monitor == null)
        {
            return NotFound();
        }

        var model = new InventarioMonitorFormViewModel
        {
            Id = monitor.Id,
            InventarioNumero = monitor.InventarioNumero,
            Patrimonio = monitor.Patrimonio,
            Marca = monitor.Marca,
            Modelo = monitor.Modelo,
            Polegadas = monitor.Polegadas,
            Observacao = monitor.Observacao
        };

        return View(model);
    }

    [HttpPost("monitores/editar/{id:int}")]
    [HttpPost("outros-dispositivos/monitores/editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarMonitor(int id, InventarioMonitorFormViewModel model)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var monitor = await _db.InventarioMonitores.FirstOrDefaultAsync(m => m.Id == id);
        if (monitor == null)
        {
            return NotFound();
        }

        var inventarioNumero = model.InventarioNumero.Trim();
        var inventarioDuplicado = await _db.InventarioMonitores
            .AnyAsync(m => m.InventarioNumero == inventarioNumero && m.Id != id);

        if (inventarioDuplicado)
        {
            ModelState.AddModelError(nameof(model.InventarioNumero), "Já existe monitor com este número de inventário.");
            return View(model);
        }

        monitor.InventarioNumero = inventarioNumero;
        monitor.Patrimonio = Limpar(model.Patrimonio);
        monitor.Marca = Limpar(model.Marca);
        monitor.Modelo = Limpar(model.Modelo);
        monitor.Polegadas = Limpar(model.Polegadas);
        monitor.Observacao = Limpar(model.Observacao);
        monitor.AtualizadoEm = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Monitor atualizado com sucesso.";
        return RedirectToAction(nameof(Monitores));
    }

    [HttpPost("monitores/excluir/{id:int}")]
    [HttpPost("outros-dispositivos/monitores/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirMonitor(int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var monitor = await _db.InventarioMonitores.FirstOrDefaultAsync(m => m.Id == id);
        if (monitor == null)
        {
            return NotFound();
        }

        _db.InventarioMonitores.Remove(monitor);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Monitor removido.";
        return RedirectToAction(nameof(Monitores));
    }

    [HttpGet("componentes")]
    public async Task<IActionResult> Componentes()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var model = new InventarioComponentesViewModel
        {
            Memorias = await _db.InventarioMemorias.OrderBy(c => c.Descricao).ToListAsync(),
            Processadores = await _db.InventarioProcessadores.OrderBy(c => c.Descricao).ToListAsync(),
            Armazenamentos = await _db.InventarioArmazenamentos.OrderBy(c => c.Descricao).ToListAsync(),
            Perifericos = await _db.InventarioPerifericos.OrderBy(c => c.Descricao).ToListAsync(),
            Cabos = await _db.InventarioCabos.OrderBy(c => c.Descricao).ToListAsync()
        };

        return View(model);
    }

    [HttpPost("componentes/{tipo}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarComponente(string tipo, string descricao)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var valor = Limpar(descricao);
        if (valor == null)
        {
            TempData["Error"] = "Informe a descrição do componente.";
            return RedirectToAction(nameof(Componentes));
        }

        switch (tipo.ToLowerInvariant())
        {
            case "memoria":
                if (!await _db.InventarioMemorias.AnyAsync(c => c.Descricao == valor))
                {
                    _db.InventarioMemorias.Add(new InventarioMemoria { Descricao = valor });
                }
                break;
            case "processador":
                if (!await _db.InventarioProcessadores.AnyAsync(c => c.Descricao == valor))
                {
                    _db.InventarioProcessadores.Add(new InventarioProcessador { Descricao = valor });
                }
                break;
            case "armazenamento":
                if (!await _db.InventarioArmazenamentos.AnyAsync(c => c.Descricao == valor))
                {
                    _db.InventarioArmazenamentos.Add(new InventarioArmazenamento { Descricao = valor });
                }
                break;
            case "periferico":
                if (!await _db.InventarioPerifericos.AnyAsync(c => c.Descricao == valor))
                {
                    _db.InventarioPerifericos.Add(new InventarioPeriferico { Descricao = valor });
                }
                break;
            case "cabo":
                if (!await _db.InventarioCabos.AnyAsync(c => c.Descricao == valor))
                {
                    _db.InventarioCabos.Add(new InventarioCabo { Descricao = valor });
                }
                break;
            default:
                return NotFound();
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Componente salvo.";
        return RedirectToAction(nameof(Componentes));
    }

    [HttpPost("componentes/{tipo}/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirComponente(string tipo, int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        switch (tipo.ToLowerInvariant())
        {
            case "memoria":
            {
                var componente = await _db.InventarioMemorias.FirstOrDefaultAsync(c => c.Id == id);
                if (componente != null) _db.InventarioMemorias.Remove(componente);
                break;
            }
            case "processador":
            {
                var componente = await _db.InventarioProcessadores.FirstOrDefaultAsync(c => c.Id == id);
                if (componente != null) _db.InventarioProcessadores.Remove(componente);
                break;
            }
            case "armazenamento":
            {
                var componente = await _db.InventarioArmazenamentos.FirstOrDefaultAsync(c => c.Id == id);
                if (componente != null) _db.InventarioArmazenamentos.Remove(componente);
                break;
            }
            case "periferico":
            {
                var componente = await _db.InventarioPerifericos.FirstOrDefaultAsync(c => c.Id == id);
                if (componente != null) _db.InventarioPerifericos.Remove(componente);
                break;
            }
            case "cabo":
            {
                var componente = await _db.InventarioCabos.FirstOrDefaultAsync(c => c.Id == id);
                if (componente != null) _db.InventarioCabos.Remove(componente);
                break;
            }
            default:
                return NotFound();
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Componente removido.";
        return RedirectToAction(nameof(Componentes));
    }

    [HttpGet("chaves")]
    public async Task<IActionResult> Chaves()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var model = new InventarioChavesViewModel
        {
            Windows = await _db.InventarioChavesLicencas
                .Where(c => c.Tipo == InventarioChaveTipo.Windows)
                .OrderByDescending(c => c.CriadoEm)
                .ToListAsync(),
            Office = await _db.InventarioChavesLicencas
                .Where(c => c.Tipo == InventarioChaveTipo.Office)
                .OrderByDescending(c => c.CriadoEm)
                .ToListAsync(),
            Antivirus = await _db.InventarioChavesLicencas
                .Where(c => c.Tipo == InventarioChaveTipo.Antivirus)
                .OrderByDescending(c => c.CriadoEm)
                .ToListAsync(),
            Outros = await _db.InventarioChavesLicencas
                .Where(c => c.Tipo == InventarioChaveTipo.Outros)
                .OrderByDescending(c => c.CriadoEm)
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost("chaves/{tipo}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarChave(string tipo, string produto, string chave, string? observacao)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (!TryParseTipoChave(tipo, out var tipoChave))
        {
            return NotFound();
        }

        var produtoLimpo = Limpar(produto);
        var chaveLimpa = Limpar(chave);

        if (produtoLimpo == null || chaveLimpa == null)
        {
            TempData["Error"] = "Informe produto e chave.";
            return RedirectToAction(nameof(Chaves));
        }

        var existe = await _db.InventarioChavesLicencas
            .AnyAsync(c => c.Tipo == tipoChave && c.Chave == chaveLimpa);

        if (existe)
        {
            TempData["Error"] = "Esta chave já está cadastrada para este tipo.";
            return RedirectToAction(nameof(Chaves));
        }

        _db.InventarioChavesLicencas.Add(new InventarioChaveLicenca
        {
            Tipo = tipoChave,
            Produto = produtoLimpo,
            Chave = chaveLimpa,
            Observacao = Limpar(observacao),
            CriadoEm = DateTimeOffset.UtcNow
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = "Chave cadastrada com sucesso.";
        return RedirectToAction(nameof(Chaves));
    }

    [HttpPost("chaves/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirChave(int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var chave = await _db.InventarioChavesLicencas.FirstOrDefaultAsync(c => c.Id == id);
        if (chave == null)
        {
            return NotFound();
        }

        _db.InventarioChavesLicencas.Remove(chave);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Chave removida.";
        return RedirectToAction(nameof(Chaves));
    }

    private async Task CarregarListasComputadorAsync(InventarioFormViewModel model)
    {
        model.Setores = await _db.InventarioSetores
            .OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
            .ToListAsync();

        model.SistemasOperacionais = await _db.InventarioSistemasOperacionais
            .OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
            .ToListAsync();

        model.Offices = await _db.InventarioOffices
            .OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
            .ToListAsync();

        model.Antiviruses = await _db.InventarioAntivirus
            .OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
            .ToListAsync();

        model.Conexoes = await _db.InventarioConexoes
            .OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
            .ToListAsync();

        model.TiposEquipamento = new List<SelectListItem>
        {
            new("Computador", ((int)InventarioTipoEquipamento.Computador).ToString()),
            new("Notebook", ((int)InventarioTipoEquipamento.Notebook).ToString())
        };

        model.MemoriasDisponiveis = await _db.InventarioMemorias
            .OrderBy(c => c.Descricao)
            .Select(c => new SelectListItem(c.Descricao, c.Id.ToString()))
            .ToListAsync();

        model.ProcessadoresDisponiveis = await _db.InventarioProcessadores
            .OrderBy(c => c.Descricao)
            .Select(c => new SelectListItem(c.Descricao, c.Id.ToString()))
            .ToListAsync();

        model.ArmazenamentosDisponiveis = await _db.InventarioArmazenamentos
            .OrderBy(c => c.Descricao)
            .Select(c => new SelectListItem(c.Descricao, c.Id.ToString()))
            .ToListAsync();

        var monitores = await _db.InventarioMonitores
            .Include(c => c.InventarioItems)
            .OrderBy(c => c.InventarioNumero)
            .ToListAsync();

        model.MonitoresDisponiveis = monitores
            .Select(c =>
            {
                var emUsoPorOutro = c.InventarioItems.Any(i => !model.Id.HasValue || i.Id != model.Id.Value);
                var outroItem = c.InventarioItems.FirstOrDefault(i => !model.Id.HasValue || i.Id != model.Id.Value);
                var texto = c.InventarioNumero + " - "
                    + (c.Marca ?? "Sem marca")
                    + (string.IsNullOrWhiteSpace(c.Modelo) ? string.Empty : " " + c.Modelo)
                    + (string.IsNullOrWhiteSpace(c.Polegadas) ? string.Empty : " (" + c.Polegadas + "\")");

                if (emUsoPorOutro && outroItem != null)
                {
                    texto += " [em uso em " + outroItem.InventarioNumero + "]";
                }

                return new SelectListItem(texto, c.Id.ToString())
                {
                    Disabled = emUsoPorOutro
                };
            })
            .ToList();

        model.PerifericosDisponiveis = await _db.InventarioPerifericos
            .OrderBy(c => c.Descricao)
            .Select(c => new SelectListItem(c.Descricao, c.Id.ToString()))
            .ToListAsync();

        model.CabosDisponiveis = await _db.InventarioCabos
            .OrderBy(c => c.Descricao)
            .Select(c => new SelectListItem(c.Descricao, c.Id.ToString()))
            .ToListAsync();

        model.MemoriasComponentes = MontarListaComponentesComQuantidade(
            model.MemoriasDisponiveis,
            model.MemoriasComponentes,
            model.MemoriaIds);

        model.ArmazenamentosComponentes = MontarListaComponentesComQuantidade(
            model.ArmazenamentosDisponiveis,
            model.ArmazenamentosComponentes,
            model.ArmazenamentoIds);

        model.PerifericosComponentes = MontarListaComponentesComQuantidade(
            model.PerifericosDisponiveis,
            model.PerifericosComponentes,
            model.PerifericoIds);

        model.CabosComponentes = MontarListaComponentesComQuantidade(
            model.CabosDisponiveis,
            model.CabosComponentes,
            model.CaboIds);

        model.ChavesWindowsDisponiveis = await CarregarChavesDisponiveisAsync(InventarioChaveTipo.Windows, model.Id);
        model.ChavesOfficeDisponiveis = await CarregarChavesDisponiveisAsync(InventarioChaveTipo.Office, model.Id);
        model.ChavesAntivirusDisponiveis = await CarregarChavesDisponiveisAsync(InventarioChaveTipo.Antivirus, model.Id);
        model.ChavesOutrosDisponiveis = await CarregarChavesDisponiveisAsync(InventarioChaveTipo.Outros, model.Id);
    }

    private async Task CarregarListasOutrosAsync(InventarioOutrosDispositivoFormViewModel model)
    {
        model.Setores = await _db.InventarioSetores
            .OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString()))
            .ToListAsync();

        model.TiposEquipamento = new List<SelectListItem>
        {
            new("Tablet", ((int)InventarioTipoEquipamento.Tablet).ToString()),
            new("TV", ((int)InventarioTipoEquipamento.TV).ToString()),
            new("Projetor", ((int)InventarioTipoEquipamento.Projetor).ToString()),
            new("Impressora", ((int)InventarioTipoEquipamento.Impressora).ToString())
        };
    }

    private static bool TipoEhOutro(InventarioTipoEquipamento tipo)
    {
        return tipo == InventarioTipoEquipamento.Tablet
            || tipo == InventarioTipoEquipamento.TV
            || tipo == InventarioTipoEquipamento.Projetor
            || tipo == InventarioTipoEquipamento.Impressora;
    }

    private static bool TryParseTipoChave(string tipo, out InventarioChaveTipo tipoChave)
    {
        switch (tipo.ToLowerInvariant())
        {
            case "windows":
                tipoChave = InventarioChaveTipo.Windows;
                return true;
            case "office":
                tipoChave = InventarioChaveTipo.Office;
                return true;
            case "antivirus":
                tipoChave = InventarioChaveTipo.Antivirus;
                return true;
            case "outros":
                tipoChave = InventarioChaveTipo.Outros;
                return true;
            default:
                tipoChave = default;
                return false;
        }
    }

    private async Task<List<InventarioRelatorioLicencaViewModel>> MontarLicencasAsync()
    {
        var chaves = await _db.InventarioChavesLicencas
            .Include(c => c.InventarioItems)
            .ToListAsync();

        return Enum.GetValues<InventarioChaveTipo>()
            .Select(tipo => new InventarioRelatorioLicencaViewModel
            {
                Tipo = DescreverTipoChave(tipo),
                EmUso = chaves.Count(c => c.Tipo == tipo && c.InventarioItems.Any()),
                Livres = chaves.Count(c => c.Tipo == tipo && !c.InventarioItems.Any())
            })
            .ToList();
    }

    private async Task<List<InventarioRelatorioMonitorViewModel>> MontarMonitoresAsync()
    {
        var monitores = await _db.InventarioMonitores
            .Include(m => m.InventarioItems)
            .OrderBy(m => m.InventarioNumero)
            .ToListAsync();

        return monitores.Select(m => new InventarioRelatorioMonitorViewModel
        {
            InventarioMonitor = m.InventarioNumero,
            Modelo = (m.Marca ?? "Sem marca") + (string.IsNullOrWhiteSpace(m.Modelo) ? string.Empty : " " + m.Modelo),
            Vinculos = m.InventarioItems.Count
        }).ToList();
    }

    private List<InventarioRelatorioConformidadeViewModel> MontarConformidade(IEnumerable<InventarioItem> itens)
    {
        var lista = new List<InventarioRelatorioConformidadeViewModel>();

        foreach (var item in itens)
        {
            if (string.IsNullOrWhiteSpace(item.Patrimonio))
            {
                lista.Add(new InventarioRelatorioConformidadeViewModel
                {
                    InventarioNumero = item.InventarioNumero,
                    Problema = "Sem patrimônio"
                });
            }

            if (string.IsNullOrWhiteSpace(item.PessoaResponsavel))
            {
                lista.Add(new InventarioRelatorioConformidadeViewModel
                {
                    InventarioNumero = item.InventarioNumero,
                    Problema = "Sem responsável"
                });
            }

            if ((item.TipoEquipamento == InventarioTipoEquipamento.Computador || item.TipoEquipamento == InventarioTipoEquipamento.Notebook) && !item.Monitores.Any())
            {
                lista.Add(new InventarioRelatorioConformidadeViewModel
                {
                    InventarioNumero = item.InventarioNumero,
                    Problema = "Sem monitor vinculado"
                });
            }
        }

        return lista;
    }

    private async Task PopularListasRelatoriosAsync(InventarioRelatoriosViewModel model)
    {
        model.Setores = await _db.InventarioSetores
            .OrderBy(s => s.Nome)
            .Select(s => new SelectListItem(s.Nome, s.Id.ToString(), model.SetorId == s.Id))
            .ToListAsync();

        model.TiposEquipamento = Enum.GetValues<InventarioTipoEquipamento>()
            .Select(t => new SelectListItem(DescreverTipoEquipamento(t), ((int)t).ToString(), model.TipoEquipamento == t))
            .ToList();

        model.TiposChave = Enum.GetValues<InventarioChaveTipo>()
            .Select(t => new SelectListItem(DescreverTipoChave(t), ((int)t).ToString(), model.TipoChave == t))
            .ToList();
    }

    private static string DescreverTipoEquipamento(InventarioTipoEquipamento tipo)
    {
        return tipo switch
        {
            InventarioTipoEquipamento.Computador => "Computador",
            InventarioTipoEquipamento.Notebook => "Notebook",
            InventarioTipoEquipamento.TV => "TV",
            InventarioTipoEquipamento.Projetor => "Projetor",
            InventarioTipoEquipamento.Impressora => "Impressora",
            InventarioTipoEquipamento.Tablet => "Tablet",
            _ => tipo.ToString()
        };
    }

    private static string DescreverTipoChave(InventarioChaveTipo tipo)
    {
        return tipo switch
        {
            InventarioChaveTipo.Windows => "Windows",
            InventarioChaveTipo.Office => "Office",
            InventarioChaveTipo.Antivirus => "Antivírus",
            InventarioChaveTipo.Outros => "Outros",
            _ => tipo.ToString()
        };
    }

    private static byte[] GerarCsv(IEnumerable<InventarioItem> itens)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Inventario,Patrimonio,Tipo,Setor,Responsavel,Backup,Monitores,Chaves,Armazenamentos,Observacao,IP");

        foreach (var i in itens)
        {
            sb.AppendLine(string.Join(",",
                Csv(i.InventarioNumero),
                Csv(i.Patrimonio),
                Csv(DescreverTipoEquipamento(i.TipoEquipamento)),
                Csv(i.Setor?.Nome),
                Csv(i.PessoaResponsavel),
                i.EhBackup ? "SIM" : "NAO",
                i.Monitores.Count,
                i.ChavesLicencas.Count,
                i.ArmazenamentosQuantidades.Sum(a => a.Quantidade),
                Csv(i.Observacao),
                Csv(i.Ip)));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Csv(string? valor)
    {
        if (string.IsNullOrEmpty(valor))
        {
            return "\"\"";
        }

        return "\"" + valor.Replace("\"", "\"\"") + "\"";
    }

    private async Task<List<InventarioChaveLicenca>> ObterChavesSelecionadasAsync(InventarioFormViewModel model)
    {
        var ids = model.ChaveWindowsIds
            .Concat(model.ChaveOfficeIds)
            .Concat(model.ChaveAntivirusIds)
            .Concat(model.ChaveOutrosIds)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return new List<InventarioChaveLicenca>();
        }

        return await _db.InventarioChavesLicencas
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
    }

    private async Task<List<SelectListItem>> CarregarChavesDisponiveisAsync(InventarioChaveTipo tipo, int? itemAtualId)
    {
        var chaves = await _db.InventarioChavesLicencas
            .Include(c => c.InventarioItems)
            .Where(c => c.Tipo == tipo)
            .OrderBy(c => c.Produto)
            .ToListAsync();

        return chaves.Select(c =>
        {
            var emUsoPorOutro = c.InventarioItems.Any(i => !itemAtualId.HasValue || i.Id != itemAtualId.Value);
            var outroItem = c.InventarioItems.FirstOrDefault(i => !itemAtualId.HasValue || i.Id != itemAtualId.Value);
            var texto = c.Produto + " - " + c.Chave;

            if (emUsoPorOutro && outroItem != null)
            {
                texto += " [em uso em " + outroItem.InventarioNumero + "]";
            }

            return new SelectListItem(texto, c.Id.ToString())
            {
                Disabled = emUsoPorOutro
            };
        }).ToList();
    }

    private async Task ValidarRecursosExclusivosAsync(InventarioFormViewModel model, int? itemAtualId)
    {
        if (model.MonitorIds.Count > 0)
        {
            var monitoresEmUso = await _db.InventarioMonitores
                .Where(m => model.MonitorIds.Contains(m.Id)
                    && m.InventarioItems.Any(i => !itemAtualId.HasValue || i.Id != itemAtualId.Value))
                .Select(m => m.InventarioNumero)
                .ToListAsync();

            if (monitoresEmUso.Count > 0)
            {
                ModelState.AddModelError(nameof(model.MonitorIds), "Monitor(es) já em uso: " + string.Join(", ", monitoresEmUso));
            }
        }

        var chaveIds = model.ChaveWindowsIds
            .Concat(model.ChaveOfficeIds)
            .Concat(model.ChaveAntivirusIds)
            .Concat(model.ChaveOutrosIds)
            .Distinct()
            .ToList();

        if (chaveIds.Count == 0)
        {
            return;
        }

        var chavesEmUso = await _db.InventarioChavesLicencas
            .Where(c => chaveIds.Contains(c.Id)
                && c.InventarioItems.Any(i => !itemAtualId.HasValue || i.Id != itemAtualId.Value))
            .Select(c => c.Produto + " - " + c.Chave)
            .ToListAsync();

        if (chavesEmUso.Count > 0)
        {
            ModelState.AddModelError(nameof(model.ChaveWindowsIds), "Chave(s) já em uso: " + string.Join(", ", chavesEmUso));
        }
    }

    private static Dictionary<int, int> ObterQuantidadesSelecionadas(List<InventarioComponenteQuantidadeItemViewModel> componentes)
    {
        return componentes
            .Where(c => c.Selecionado)
            .GroupBy(c => c.Id)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var quantidade = g.Max(x => x.Quantidade);
                    return quantidade <= 0 ? 1 : quantidade;
                });
    }

    private static List<InventarioComponenteQuantidadeItemViewModel> MontarListaComponentesComQuantidade(
        IEnumerable<SelectListItem> disponiveis,
        List<InventarioComponenteQuantidadeItemViewModel> valoresAtuais,
        List<int> selecionadosLegado)
    {
        var atualPorId = valoresAtuais
            .Where(c => c.Id > 0)
            .GroupBy(c => c.Id)
            .ToDictionary(g => g.Key, g => g.First());

        return disponiveis
            .Select(item =>
            {
                var id = int.Parse(item.Value!);
                var possuiAtual = atualPorId.TryGetValue(id, out var atual);
                var selecionado = possuiAtual
                    ? atual!.Selecionado
                    : selecionadosLegado.Contains(id);

                var quantidade = possuiAtual ? atual!.Quantidade : 1;
                if (quantidade <= 0)
                {
                    quantidade = 1;
                }

                return new InventarioComponenteQuantidadeItemViewModel
                {
                    Id = id,
                    Descricao = item.Text,
                    Selecionado = selecionado,
                    Quantidade = quantidade
                };
            })
            .ToList();
    }

    private static string? Limpar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private bool EstaAutenticado()
    {
        return HttpContext.Session.GetString(ChaveSessao) == "true";
    }
}
