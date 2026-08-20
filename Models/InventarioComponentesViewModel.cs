namespace ChamadosTI.Models;

public class InventarioComponentesViewModel
{
    public IReadOnlyList<InventarioMemoria> Memorias { get; init; } = [];
    public IReadOnlyList<InventarioProcessador> Processadores { get; init; } = [];
    public IReadOnlyList<InventarioArmazenamento> Armazenamentos { get; init; } = [];
    public IReadOnlyList<InventarioPeriferico> Perifericos { get; init; } = [];
    public IReadOnlyList<InventarioCabo> Cabos { get; init; } = [];
}
