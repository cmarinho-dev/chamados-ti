namespace ChamadosTI.Models;

public class InventarioComponenteQuantidadeItemViewModel
{
    public int Id { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public bool Selecionado { get; set; }

    public int Quantidade { get; set; } = 1;
}
