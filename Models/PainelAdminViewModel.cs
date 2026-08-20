namespace ChamadosTI.Models;

public class PainelAdminViewModel
{
    public IReadOnlyList<Chamado> Chamados { get; init; } = Array.Empty<Chamado>();
    public int TotalChamados { get; set; }
    public int UltimoId { get; set; }
    public string? UltimoSolicitante { get; set; }
    public string? Pesquisa { get; set; }
    public int PaginaAtual { get; set; } = 1;
    public int TotalPaginas { get; set; }
    public int ItensPorPagina { get; set; } = 10;
}
