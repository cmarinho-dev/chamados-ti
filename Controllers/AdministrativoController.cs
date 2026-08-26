using ChamadosTI.Data;
using ChamadosTI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChamadosTI.Controllers;

[Route("admin/administrativo")]
public class AdministrativoController : Controller
{
    private const string ChaveSessao = "AdminAutenticado";
    private readonly ContextoChamados _db;

    public AdministrativoController(ContextoChamados db)
    {
        _db = db;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        return RedirectToAction(nameof(EquipeTi));
    }

    [HttpGet("departamentos")]
    public async Task<IActionResult> Departamentos(string? pesquisa)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var termo = pesquisa?.Trim();
        var query = _db.InventarioSetores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            query = query.Where(d => d.Nome.Contains(termo));
        }

        var model = new AdministrativoDepartamentosViewModel
        {
            Departamentos = await query.OrderBy(d => d.Nome).ToListAsync(),
            Pesquisa = termo,
            PaginaAtual = 1,
            TotalPaginas = 1,
            ItensPorPagina = 20
        };

        return View(model);
    }

    [HttpPost("departamentos")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarDepartamento(string nome)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var valor = Limpar(nome);
        if (valor == null)
        {
            TempData["Error"] = "Informe o nome do departamento.";
            return RedirectToAction(nameof(Departamentos));
        }

        var existe = await _db.InventarioSetores.AnyAsync(d => d.Nome == valor);
        if (existe)
        {
            TempData["Error"] = "Departamento já cadastrado.";
            return RedirectToAction(nameof(Departamentos));
        }

        _db.InventarioSetores.Add(new InventarioSetor { Nome = valor });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Departamento adicionado.";
        return RedirectToAction(nameof(Departamentos));
    }

    [HttpPost("departamentos/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirDepartamento(int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var departamento = await _db.InventarioSetores.FirstOrDefaultAsync(d => d.Id == id);
        if (departamento == null)
        {
            return NotFound();
        }

        var possuiUsuariosLegados = await _db.AdministrativoUsuarios.AnyAsync(u => u.DepartamentoId == id);
        if (possuiUsuariosLegados)
        {
            TempData["Error"] = "Não é possível excluir: departamento possui cadastros legados vinculados.";
            return RedirectToAction(nameof(Departamentos));
        }

        var possuiInventario = await _db.InventarioItems.AnyAsync(i => i.SetorId == id);
        if (possuiInventario)
        {
            TempData["Error"] = "Não é possível excluir: departamento está em uso no inventário.";
            return RedirectToAction(nameof(Departamentos));
        }

        _db.InventarioSetores.Remove(departamento);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Departamento removido.";
        return RedirectToAction(nameof(Departamentos));
    }

    [HttpGet("usuarios")]
    public IActionResult Usuarios()
    {
        return RedirectToAction(nameof(EquipeTi));
    }

    [HttpGet("equipe-ti")]
    public async Task<IActionResult> EquipeTi()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var model = new EquipeTiViewModel
        {
            Tecnicos = await _db.TecnicosTi
                .OrderBy(t => t.Periodo)
                .ThenBy(t => t.Nome)
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost("equipe-ti")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarTecnico(string nome, string periodo)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var nomeLimpo = Limpar(nome);
        if (nomeLimpo == null)
        {
            TempData["Error"] = "Informe o nome da pessoa de TI.";
            return RedirectToAction(nameof(EquipeTi));
        }

        if (nomeLimpo.Length > 160)
        {
            TempData["Error"] = "O nome deve ter no máximo 160 caracteres.";
            return RedirectToAction(nameof(EquipeTi));
        }

        if (!PeriodoValido(periodo))
        {
            TempData["Error"] = "Selecione o período Manhã ou Tarde.";
            return RedirectToAction(nameof(EquipeTi));
        }

        var tecnicoExiste = await _db.TecnicosTi.AnyAsync(t => t.Nome == nomeLimpo);
        if (tecnicoExiste)
        {
            TempData["Error"] = "Esta pessoa já está cadastrada na equipe de TI.";
            return RedirectToAction(nameof(EquipeTi));
        }

        _db.TecnicosTi.Add(new TecnicoTi
        {
            Nome = nomeLimpo,
            Periodo = periodo,
            OrdemDistribuicao = Random.Shared.Next(1, 1_000_000_000)
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Pessoa adicionada à equipe de TI.";
        return RedirectToAction(nameof(EquipeTi));
    }

    [HttpPost("equipe-ti/periodo/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarPeriodoTecnico(int id, string periodo)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        if (!PeriodoValido(periodo))
        {
            TempData["Error"] = "Selecione o período Manhã ou Tarde.";
            return RedirectToAction(nameof(EquipeTi));
        }

        var tecnico = await _db.TecnicosTi.FirstOrDefaultAsync(t => t.Id == id);
        if (tecnico == null)
        {
            return NotFound();
        }

        tecnico.Periodo = periodo;
        tecnico.OrdemDistribuicao = Random.Shared.Next(1, 1_000_000_000);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Período da pessoa de TI atualizado.";
        return RedirectToAction(nameof(EquipeTi));
    }

    [HttpPost("equipe-ti/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirTecnico(int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var tecnico = await _db.TecnicosTi.FirstOrDefaultAsync(t => t.Id == id);
        if (tecnico == null)
        {
            return NotFound();
        }

        var possuiChamados = await _db.Chamados.AnyAsync(c => c.TecnicoTiId == id);
        if (possuiChamados)
        {
            TempData["Error"] = "Não é possível excluir: esta pessoa possui chamados vinculados.";
            return RedirectToAction(nameof(EquipeTi));
        }

        _db.TecnicosTi.Remove(tecnico);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Pessoa removida da equipe de TI.";
        return RedirectToAction(nameof(EquipeTi));
    }

    private static string? Limpar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private static bool PeriodoValido(string periodo)
    {
        return periodo is "Manhã" or "Tarde";
    }

    private bool EstaAutenticado()
    {
        return HttpContext.Session.GetString(ChaveSessao) == "true";
    }
}
