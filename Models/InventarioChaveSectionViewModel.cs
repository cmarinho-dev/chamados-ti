namespace ChamadosTI.Models;

public class InventarioChaveSectionViewModel
{
    public required string Titulo { get; init; }
    public required string TipoSlug { get; init; }
    public IReadOnlyList<InventarioChaveLicenca> Itens { get; init; } = [];
}
