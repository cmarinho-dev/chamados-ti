using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChamadosTI.Models;

public class InventarioFormViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "N° Inventário")]
    public string InventarioNumero { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Patrimonio { get; set; }

    [Required]
    [Display(Name = "Tipo")]
    public InventarioTipoEquipamento TipoEquipamento { get; set; } = InventarioTipoEquipamento.Computador;

    [Display(Name = "Equipamento de backup")]
    public bool EhBackup { get; set; }

    [StringLength(120)]
    [Display(Name = "Pessoa responsável")]
    public string? PessoaResponsavel { get; set; }

    [Required]
    [Display(Name = "Departamento")]
    public int SetorId { get; set; }

    [Display(Name = "Sistema operacional")]
    public int? SistemaOperacionalId { get; set; }

    [Display(Name = "Office")]
    public int? OfficeId { get; set; }

    [Display(Name = "Antivírus")]
    public int? AntivirusId { get; set; }

    [Display(Name = "Conexão")]
    public int? ConexaoId { get; set; }

    [StringLength(45)]
    public string? Ip { get; set; }

    [StringLength(600)]
    public string? Observacao { get; set; }

    [Display(Name = "Memórias")]
    public List<int> MemoriaIds { get; set; } = new();

    [Display(Name = "Processador")]
    public int? ProcessadorId { get; set; }

    [Display(Name = "Armazenamentos")]
    public List<int> ArmazenamentoIds { get; set; } = new();

    [Display(Name = "Monitores")]
    public List<int> MonitorIds { get; set; } = new();

    [Display(Name = "Periféricos")]
    public List<int> PerifericoIds { get; set; } = new();

    [Display(Name = "Cabos")]
    public List<int> CaboIds { get; set; } = new();

    public List<InventarioComponenteQuantidadeItemViewModel> MemoriasComponentes { get; set; } = new();
    public List<InventarioComponenteQuantidadeItemViewModel> ArmazenamentosComponentes { get; set; } = new();
    public List<InventarioComponenteQuantidadeItemViewModel> PerifericosComponentes { get; set; } = new();
    public List<InventarioComponenteQuantidadeItemViewModel> CabosComponentes { get; set; } = new();

    [Display(Name = "Chaves Windows")]
    public List<int> ChaveWindowsIds { get; set; } = new();

    [Display(Name = "Chaves Office")]
    public List<int> ChaveOfficeIds { get; set; } = new();

    [Display(Name = "Chaves Antivírus")]
    public List<int> ChaveAntivirusIds { get; set; } = new();

    [Display(Name = "Chaves Outros")]
    public List<int> ChaveOutrosIds { get; set; } = new();

    public List<SelectListItem> Setores { get; set; } = new();
    public List<SelectListItem> SistemasOperacionais { get; set; } = new();
    public List<SelectListItem> Offices { get; set; } = new();
    public List<SelectListItem> Antiviruses { get; set; } = new();
    public List<SelectListItem> Conexoes { get; set; } = new();
    public List<SelectListItem> TiposEquipamento { get; set; } = new();

    public List<SelectListItem> MemoriasDisponiveis { get; set; } = new();
    public List<SelectListItem> ProcessadoresDisponiveis { get; set; } = new();
    public List<SelectListItem> ArmazenamentosDisponiveis { get; set; } = new();
    public List<SelectListItem> MonitoresDisponiveis { get; set; } = new();
    public List<SelectListItem> PerifericosDisponiveis { get; set; } = new();
    public List<SelectListItem> CabosDisponiveis { get; set; } = new();
    public List<SelectListItem> ChavesWindowsDisponiveis { get; set; } = new();
    public List<SelectListItem> ChavesOfficeDisponiveis { get; set; } = new();
    public List<SelectListItem> ChavesAntivirusDisponiveis { get; set; } = new();
    public List<SelectListItem> ChavesOutrosDisponiveis { get; set; } = new();
}
