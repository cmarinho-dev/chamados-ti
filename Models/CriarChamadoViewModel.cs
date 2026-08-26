using System.ComponentModel.DataAnnotations;

namespace ChamadosTI.Models;

public class CriarChamadoViewModel
{
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Informe seu nome.")]
    [StringLength(120)]
    public string NomeSolicitante { get; set; } = string.Empty;

    [Display(Name = "Período")]
    [Required(ErrorMessage = "Informe se o atendimento é pela manhã ou à tarde.")]
    public string Periodo { get; set; } = string.Empty;

    [Display(Name = "Descrição do problema (opcional)")]
    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? DescricaoProblema { get; set; }

    public List<string> PessoasDisponiveis { get; set; } = [];
}
