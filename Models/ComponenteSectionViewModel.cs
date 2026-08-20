namespace ChamadosTI.Models;

public class ComponenteSectionViewModel
{
    public ComponenteSectionViewModel(string titulo, string tipo, IReadOnlyList<(int Id, string Descricao)> itens)
    {
        Titulo = titulo;
        Tipo = tipo;
        Itens = itens;
    }

    public string Titulo { get; }
    public string Tipo { get; }
    public IReadOnlyList<(int Id, string Descricao)> Itens { get; }
}
