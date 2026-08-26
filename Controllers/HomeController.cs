using System.Diagnostics;
using ChamadosTI.Data;
using ChamadosTI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChamadosTI.Controllers;

public class HomeController : Controller
{
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
        if (model.Periodo != "Manhã" && model.Periodo != "Tarde")
        {
            ModelState.AddModelError(nameof(model.Periodo), "Selecione Manhã ou Tarde.");
        }

        InventarioItem? inventario = null;
        if (model.InventarioItemId.HasValue)
        {
            inventario = await _db.InventarioItems
                .Include(i => i.Setor)
                .FirstOrDefaultAsync(i => i.Id == model.InventarioItemId.Value
                    && (i.TipoEquipamento == InventarioTipoEquipamento.Computador
                        || i.TipoEquipamento == InventarioTipoEquipamento.Notebook)
                    && i.PessoaResponsavel != null
                    && i.PessoaResponsavel != "");
        }

        if (inventario == null)
        {
            ModelState.AddModelError(nameof(model.InventarioItemId), "Selecione um nome disponível no inventário de computadores.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarPessoasDisponiveisAsync(model);
            return View(model);
        }

        var chamado = new Chamado
        {
            NomeSolicitante = inventario!.PessoaResponsavel!.Trim(),
            InventarioItemId = inventario.Id,
            Setor = inventario.Setor?.Nome ?? "Não informado",
            Periodo = model.Periodo,
            DescricaoProblema = Limpar(model.DescricaoProblema),
            Situacao = "Aberto",
            CriadoEm = DateTimeOffset.UtcNow
        };

        _db.Chamados.Add(chamado);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Chamado aberto com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarPessoasDisponiveisAsync(CriarChamadoViewModel model)
    {
        var inventarios = await _db.InventarioItems
            .Include(i => i.Setor)
            .Where(i => (i.TipoEquipamento == InventarioTipoEquipamento.Computador
                    || i.TipoEquipamento == InventarioTipoEquipamento.Notebook)
                && i.PessoaResponsavel != null
                && i.PessoaResponsavel != "")
            .OrderBy(i => i.PessoaResponsavel)
            .ThenBy(i => i.InventarioNumero)
            .ToListAsync();

        model.PessoasDisponiveis = inventarios
            .Select(i => new SelectListItem(
                $"{i.PessoaResponsavel} — {i.Setor?.Nome ?? "Sem setor"} (Inv. {i.InventarioNumero})",
                i.Id.ToString(),
                model.InventarioItemId == i.Id))
            .ToList();
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
