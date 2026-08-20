using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("chamados")]
public class Chamado
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string NomeSolicitante { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Setor { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Situacao { get; set; } = "Aberto";

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? FinalizadoEm { get; set; }
}
