namespace ChamadosTI.Models;

public class InventarioPaginacaoViewModel
{
    public int PaginaAtual { get; init; } = 1;
    public int TotalPaginas { get; init; } = 1;
    public int ItensPorPagina { get; init; } = 5;
    public int TotalItens { get; init; }
    public string? Busca { get; init; }
}

public class InventarioComputadoresViewModel
{
    public IReadOnlyList<InventarioItem> Itens { get; init; } = [];
    public InventarioPaginacaoViewModel Paginacao { get; init; } = new();
}

public class InventarioMonitoresViewModel
{
    public IReadOnlyList<InventarioMonitor> Monitores { get; init; } = [];
    public InventarioPaginacaoViewModel Paginacao { get; init; } = new();
}
