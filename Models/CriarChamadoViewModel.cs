using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChamadosTI.Models;

public class CriarChamadoViewModel
{
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Selecione seu nome.")]
    public int? InventarioItemId { get; set; }

    [Display(Name = "Período")]
    [Required(ErrorMessage = "Informe se o atendimento é pela manhã ou à tarde.")]
    public string Periodo { get; set; } = string.Empty;

    [Display(Name = "Descrição do problema (opcional)")]
    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? DescricaoProblema { get; set; }

    public List<SelectListItem> PessoasDisponiveis { get; set; } = [];
}
