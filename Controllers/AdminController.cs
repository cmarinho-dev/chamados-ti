using System.Data;
using System.Text.RegularExpressions;
using ChamadosTI.Data;
using ChamadosTI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ChamadosTI.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    private const string ChaveSessao = "AdminAutenticado";
    private readonly ContextoChamados _db;
    private readonly IConfiguration _configuration;

    public AdminController(ContextoChamados db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? pesquisa)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction(nameof(Login));
        }

        var termo = pesquisa?.Trim();
        var query = _db.Chamados.AsQueryable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            query = query.Where(c =>
                c.NomeSolicitante.Contains(termo) ||
                c.Setor.Contains(termo) ||
                c.Situacao.Contains(termo));
        }

        var chamados = await query
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();

        var ultimoId = chamados.Count == 0 ? 0 : chamados.Max(c => c.Id);
        var ultimoSolicitante = chamados.FirstOrDefault()?.NomeSolicitante;

        var viewModel = new PainelAdminViewModel
        {
            Chamados = chamados,
            TotalChamados = chamados.Count,
            UltimoId = ultimoId,
            UltimoSolicitante = ultimoSolicitante,
            Pesquisa = termo,
            PaginaAtual = 1,
            TotalPaginas = 1,
            ItensPorPagina = 20
        };

        return View(viewModel);
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View(new LoginAdminViewModel());
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginAdminViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuarioEsperado = _configuration["Admin:Usuario"] ?? "admin";
        var senhaEsperada = _configuration["Admin:Senha"] ?? "admin123";

        if (!string.Equals(model.Usuario, usuarioEsperado, StringComparison.OrdinalIgnoreCase)
            || model.Senha != senhaEsperada)
        {
            ModelState.AddModelError(string.Empty, "Usuario ou senha invalidos.");
            return View(model);
        }

        HttpContext.Session.SetString(ChaveSessao, "true");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(ChaveSessao);
        return RedirectToAction(nameof(Login));
    }

    [HttpPost("atualizar-situacao")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarSituacao(int id, string situacao)
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var permitido = new[] { "Aberto", "Em andamento", "Finalizado" };
        if (!permitido.Contains(situacao))
        {
            return BadRequest();
        }

        var chamado = await _db.Chamados.FirstOrDefaultAsync(c => c.Id == id);
        if (chamado == null)
        {
            return NotFound();
        }

        chamado.Situacao = situacao;
        if (situacao == "Finalizado")
        {
            chamado.FinalizadoEm = DateTimeOffset.UtcNow;
        }
        else
        {
            chamado.FinalizadoEm = null;
        }
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("limpar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Limpar()
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        _db.Chamados.RemoveRange(_db.Chamados);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("updates")]
    public async Task<IActionResult> Updates()
    {
        if (!EstaAutenticado())
        {
            return Unauthorized();
        }

        var ultimoId = await _db.Chamados
            .MaxAsync(c => (int?)c.Id) ?? 0;

        var totalAbertos = await _db.Chamados.CountAsync();

        return Json(new { latestId = ultimoId, totalOpen = totalAbertos });
    }

    [HttpGet("sql")]
    public IActionResult SqlExecutor()
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction(nameof(Login));
        }

        return View(new AdminSqlExecutorViewModel());
    }

    [HttpPost("sql")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SqlExecutor(AdminSqlExecutorViewModel model)
    {
        if (!EstaAutenticado())
        {
            return RedirectToAction(nameof(Login));
        }

        if (string.IsNullOrWhiteSpace(model.Sql))
        {
            model.TipoResultado = "erro";
            model.Mensagem = "Digite um comando SQL.";
            return View(model);
        }

        var sql = model.Sql.Trim();
        if (!EhComandoPermitido(sql))
        {
            model.TipoResultado = "erro";
            model.Mensagem = "Apenas comandos SELECT, INSERT, UPDATE e DELETE são permitidos.";
            return View(model);
        }

        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            var isQuery = sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase);
            if (isQuery)
            {
                await using var reader = await command.ExecuteReaderAsync();
                var columns = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetName(i));
                }

                var rows = new List<Dictionary<string, object?>>();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object?>();
                    foreach (var column in columns)
                    {
                        row[column] = reader[column] is DBNull ? null : reader[column];
                    }

                    rows.Add(row);
                }

                model.TipoResultado = "sucesso";
                model.Colunas = columns;
                model.Resultados = rows;
                model.Mensagem = $"Consulta executada com sucesso. {rows.Count} linha(s) retornada(s).";
            }
            else
            {
                var linhasAfetadas = await command.ExecuteNonQueryAsync();
                model.TipoResultado = "sucesso";
                model.LinhasAfetadas = linhasAfetadas;
                model.Mensagem = $"Comando executado com sucesso. {linhasAfetadas} linha(s) afetada(s).";
            }
        }
        catch (Exception ex)
        {
            model.TipoResultado = "erro";
            model.Mensagem = ex.Message;
        }

        return View(model);
    }

    private static bool EhComandoPermitido(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            return false;
        }

        var normalized = Regex.Replace(sql.Trim(), @"\s+", " ").Trim();
        if (string.IsNullOrEmpty(normalized))
        {
            return false;
        }

        var primeiroComando = normalized.Split(' ', 2)[0].ToUpperInvariant();
        return primeiroComando is "SELECT" or "INSERT" or "UPDATE" or "DELETE";
    }

    private bool EstaAutenticado()
    {
        return HttpContext.Session.GetString(ChaveSessao) == "true";
    }
}
