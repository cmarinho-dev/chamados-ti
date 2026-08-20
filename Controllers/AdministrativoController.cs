using ChamadosTI.Data;
using ChamadosTI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        return RedirectToAction(nameof(Departamentos));
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
            Departamentos = await query
                .OrderBy(d => d.Nome)
                .ToListAsync(),
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

        var departamento = await _db.InventarioSetores
            .FirstOrDefaultAsync(d => d.Id == id);

        if (departamento == null)
        {
            return NotFound();
        }

        var possuiUsuarios = await _db.AdministrativoUsuarios.AnyAsync(u => u.DepartamentoId == id);
        if (possuiUsuarios)
        {
            TempData["Error"] = "Não é possível excluir: departamento possui usuários vinculados.";
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
    public async Task<IActionResult> Usuarios(string? pesquisa)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction("Login", "Admin");
        }

        var termo = pesquisa?.Trim();
        var query = _db.AdministrativoUsuarios
            .Include(u => u.Departamento)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            query = query.Where(u =>
                u.Nome.Contains(termo) ||
                u.Usuario.Contains(termo) ||
                (u.Departamento != null && u.Departamento.Nome.Contains(termo)));
        }

        var departamentos = await _db.InventarioSetores
            .OrderBy(d => d.Nome)
            .ToListAsync();

        var model = new AdministrativoUsuariosViewModel
        {
            Usuarios = await query
                .OrderBy(u => u.Nome)
                .ToListAsync(),
            Departamentos = departamentos
                .Select(d => new SelectListItem(d.Nome, d.Id.ToString()))
                .ToList(),
            Pesquisa = termo,
            PaginaAtual = 1,
            TotalPaginas = 1,
            ItensPorPagina = 20
        };

        return View(model);
    }

    [HttpPost("usuarios")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarUsuario(string nome, string usuario, int departamentoId)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var nomeLimpo = Limpar(nome);
        var usuarioLimpo = Limpar(usuario);

        if (nomeLimpo == null || usuarioLimpo == null)
        {
            TempData["Error"] = "Preencha nome e usuário.";
            return RedirectToAction(nameof(Usuarios));
        }

        var departamentoExiste = await _db.InventarioSetores.AnyAsync(d => d.Id == departamentoId);
        if (!departamentoExiste)
        {
            TempData["Error"] = "Departamento inválido.";
            return RedirectToAction(nameof(Usuarios));
        }

        var usuarioExiste = await _db.AdministrativoUsuarios.AnyAsync(u => u.Usuario == usuarioLimpo);
        if (usuarioExiste)
        {
            TempData["Error"] = "Usuário já cadastrado.";
            return RedirectToAction(nameof(Usuarios));
        }

        _db.AdministrativoUsuarios.Add(new AdministrativoUsuario
        {
            Nome = nomeLimpo,
            Usuario = usuarioLimpo,
            DepartamentoId = departamentoId
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = "Usuário adicionado.";
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpPost("usuarios/excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirUsuario(int id)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var usuario = await _db.AdministrativoUsuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null)
        {
            return NotFound();
        }

        _db.AdministrativoUsuarios.Remove(usuario);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Usuário removido.";
        return RedirectToAction(nameof(Usuarios));
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
