namespace ChamadosTI.Models;

public class InventarioOutrosViewModel
{
    public IReadOnlyList<InventarioItem> Dispositivos { get; init; } = [];
    public IReadOnlyList<InventarioMonitor> Monitores { get; init; } = [];
    public InventarioPaginacaoViewModel Paginacao { get; init; } = new();
}
