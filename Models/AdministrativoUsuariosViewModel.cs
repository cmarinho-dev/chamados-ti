using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChamadosTI.Models;

public class AdministrativoUsuariosViewModel
{
    public IReadOnlyList<AdministrativoUsuario> Usuarios { get; init; } = [];
    public List<SelectListItem> Departamentos { get; init; } = [];
    public string? Pesquisa { get; set; }
    public int PaginaAtual { get; set; } = 1;
    public int TotalPaginas { get; set; }
    public int ItensPorPagina { get; set; } = 10;
}
