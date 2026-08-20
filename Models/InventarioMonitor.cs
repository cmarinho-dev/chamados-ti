using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventariomonitores")]
public class InventarioMonitor
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string InventarioNumero { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Patrimonio { get; set; }

    [StringLength(60)]
    public string? Marca { get; set; }

    [StringLength(60)]
    public string? Modelo { get; set; }

    [StringLength(10)]
    public string? Polegadas { get; set; }

    [StringLength(300)]
    public string? Observacao { get; set; }

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? AtualizadoEm { get; set; }

    public List<InventarioItem> InventarioItems { get; set; } = new();
}
