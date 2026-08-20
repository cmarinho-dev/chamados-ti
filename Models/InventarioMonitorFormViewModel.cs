using System.ComponentModel.DataAnnotations;

namespace ChamadosTI.Models;

public class InventarioMonitorFormViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "N° Inventário")]
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
}
