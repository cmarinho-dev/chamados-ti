using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventariochaveslicencas")]
public class InventarioChaveLicenca
{
    public int Id { get; set; }

    public InventarioChaveTipo Tipo { get; set; }

    [Required, StringLength(80)]
    public string Produto { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Chave { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Observacao { get; set; }

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;

    public List<InventarioItem> InventarioItems { get; set; } = new();
}
