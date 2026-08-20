namespace ChamadosTI.Models;

public class InventarioChavesViewModel
{
    public IReadOnlyList<InventarioChaveLicenca> Windows { get; init; } = [];
    public IReadOnlyList<InventarioChaveLicenca> Office { get; init; } = [];
    public IReadOnlyList<InventarioChaveLicenca> Antivirus { get; init; } = [];
    public IReadOnlyList<InventarioChaveLicenca> Outros { get; init; } = [];
}
