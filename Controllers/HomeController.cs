using System.Diagnostics;
using ChamadosTI.Data;
using ChamadosTI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChamadosTI.Controllers;

public class HomeController : Controller
{
    private static readonly string[] NomesOcultos =
    [
        "RESERVA",
        "COMPUTADOR",
        "DESCONHECIDO",
        "NOTEBOOK",
        "SALA",
        "SERVIDOR",
        "CABINE"
    ];

    private readonly ContextoChamados _db;

    public HomeController(ContextoChamados db)
    {
        _db = db;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var model = new CriarChamadoViewModel();
        await CarregarPessoasDisponiveisAsync(model);
        return View(model);
    }

    [HttpPost("/")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CriarChamadoViewModel model)
    {
        var nomeSolicitante = Limpar(model.NomeSolicitante);
        InventarioItem? inventario = null;
        if (nomeSolicitante != null && NomePermitido(nomeSolicitante))
        {
            inventario = await _db.InventarioItems
                .Include(i => i.Setor)
                .Where(i => (i.TipoEquipamento == InventarioTipoEquipamento.Computador
                        || i.TipoEquipamento == InventarioTipoEquipamento.Notebook)
                    && i.PessoaResponsavel != null
                    && i.PessoaResponsavel == nomeSolicitante)
                .OrderBy(i => i.InventarioNumero)
                .FirstOrDefaultAsync();
        }

        if (inventario == null || !NomePermitido(inventario.PessoaResponsavel!))
        {
            ModelState.AddModelError(nameof(model.NomeSolicitante), "Selecione um nome disponível no inventário de computadores.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarPessoasDisponiveisAsync(model);
            return View(model);
        }

        var periodo = ObterPeriodoAtual();
        var tecnico = await SelecionarProximoTecnicoAsync(periodo);
        var agora = DateTimeOffset.UtcNow;

        var chamado = new Chamado
        {
            NomeSolicitante = inventario!.PessoaResponsavel!.Trim(),
            InventarioItemId = inventario.Id,
            Setor = inventario.Setor?.Nome ?? "Não informado",
            Periodo = periodo,
            TecnicoTiId = tecnico?.Id,
            DescricaoProblema = Limpar(model.DescricaoProblema),
            Situacao = "Aberto",
            CriadoEm = agora
        };

        _db.Chamados.Add(chamado);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Chamado aberto com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarPessoasDisponiveisAsync(CriarChamadoViewModel model)
    {
        var nomes = await _db.InventarioItems
            .Where(i => (i.TipoEquipamento == InventarioTipoEquipamento.Computador
                    || i.TipoEquipamento == InventarioTipoEquipamento.Notebook)
                && i.PessoaResponsavel != null
                && i.PessoaResponsavel != "")
            .Select(i => i.PessoaResponsavel!)
            .ToListAsync();

        model.PessoasDisponiveis = nomes
            .Select(nome => nome.Trim())
            .Where(NomePermitido)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(nome => nome, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    private static bool NomePermitido(string nome)
    {
        return !NomesOcultos.Any(termo => nome.Contains(termo, StringComparison.OrdinalIgnoreCase));
    }

    private static string ObterPeriodoAtual()
    {
        var agora = DateTime.Now;
        return agora.Hour < 12 || (agora.Hour == 12 && agora.Minute == 0)
            ? "Manhã"
            : "Tarde";
    }

    private async Task<TecnicoTi?> SelecionarProximoTecnicoAsync(string periodo)
    {
        var tecnicos = await _db.TecnicosTi
            .Where(t => t.Periodo == periodo)
            .OrderBy(t => t.OrdemDistribuicao)
            .ThenBy(t => t.Id)
            .ToListAsync();

        var tecnico = tecnicos.FirstOrDefault();
        if (tecnico != null)
        {
            tecnico.OrdemDistribuicao = tecnicos.Max(t => t.OrdemDistribuicao) + 1;
        }

        return tecnico;
    }

    private static string? Limpar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
