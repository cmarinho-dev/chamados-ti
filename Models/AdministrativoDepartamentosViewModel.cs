namespace ChamadosTI.Models;

public class AdministrativoDepartamentosViewModel
{
    public IReadOnlyList<InventarioSetor> Departamentos { get; init; } = [];
    public string? Pesquisa { get; set; }
    public int PaginaAtual { get; set; } = 1;
    public int TotalPaginas { get; set; }
    public int ItensPorPagina { get; set; } = 10;
}
