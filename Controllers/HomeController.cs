using System.Diagnostics;
using ChamadosTI.Data;
using ChamadosTI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChamadosTI.Controllers;

public class HomeController : Controller
{
    private readonly ContextoChamados _db;

    public HomeController(ContextoChamados db)
    {
        _db = db;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(new CriarChamadoViewModel());
    }

    [HttpPost("/")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CriarChamadoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var chamado = new Chamado
        {
            NomeSolicitante = model.NomeSolicitante.Trim(),
            Setor = model.Setor.Trim(),
            Situacao = "Aberto",
            CriadoEm = DateTimeOffset.UtcNow
        };

        _db.Chamados.Add(chamado);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Chamado aberto com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
