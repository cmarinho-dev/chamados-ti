using System.ComponentModel.DataAnnotations;

namespace ChamadosTI.Models;

public class CriarChamadoViewModel
{
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Informe seu nome.")]
    [StringLength(80)]
    public string NomeSolicitante { get; set; } = string.Empty;

    [Display(Name = "Area")]
    [Required(ErrorMessage = "Informe o setor.")]
    [StringLength(80)]
    public string Setor { get; set; } = string.Empty;
}
