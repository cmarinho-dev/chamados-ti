using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("chamados")]
public class Chamado
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string NomeSolicitante { get; set; } = string.Empty;

    public int? InventarioItemId { get; set; }
    public InventarioItem? InventarioItem { get; set; }

    [Required, StringLength(80)]
    public string Setor { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? DescricaoProblema { get; set; }

    [Required, StringLength(10)]
    public string Periodo { get; set; } = string.Empty;

    public int? TecnicoTiId { get; set; }
    public TecnicoTi? TecnicoTi { get; set; }

    [StringLength(2000)]
    public string? ParecerFinal { get; set; }

    [Required, StringLength(20)]
    public string Situacao { get; set; } = "Aberto";

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? FinalizadoEm { get; set; }
}
