namespace ChamadosTI.Models;

public class EquipeTiViewModel
{
    public IReadOnlyList<TecnicoTi> Tecnicos { get; init; } = [];
    public string? Pesquisa { get; set; }
}
