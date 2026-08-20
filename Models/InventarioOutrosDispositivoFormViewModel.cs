using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChamadosTI.Models;

public class InventarioOutrosDispositivoFormViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "N° Inventário")]
    public string InventarioNumero { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Patrimonio { get; set; }

    [Required]
    [Display(Name = "Tipo")]
    public InventarioTipoEquipamento TipoEquipamento { get; set; }

    [StringLength(120)]
    [Display(Name = "Pessoa responsável")]
    public string? PessoaResponsavel { get; set; }

    [Required]
    [Display(Name = "Departamento")]
    public int SetorId { get; set; }

    [StringLength(600)]
    public string? Observacao { get; set; }

    public List<SelectListItem> TiposEquipamento { get; set; } = new();
    public List<SelectListItem> Setores { get; set; } = new();
}
