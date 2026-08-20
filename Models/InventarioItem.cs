using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventarioitems")]
public class InventarioItem
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string InventarioNumero { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Patrimonio { get; set; }

    [Required]
    public InventarioTipoEquipamento TipoEquipamento { get; set; } = InventarioTipoEquipamento.Computador;

    public bool EhBackup { get; set; }

    [StringLength(120)]
    public string? PessoaResponsavel { get; set; }

    public int SetorId { get; set; }
    public InventarioSetor? Setor { get; set; }

    public int? SistemaOperacionalId { get; set; }
    public InventarioSistemaOperacional? SistemaOperacional { get; set; }

    public int? OfficeId { get; set; }
    public InventarioOffice? Office { get; set; }

    public int? AntivirusId { get; set; }
    public InventarioAntivirus? Antivirus { get; set; }

    public int? ConexaoId { get; set; }
    public InventarioConexao? Conexao { get; set; }

    [StringLength(45)]
    public string? Ip { get; set; }

    [StringLength(600)]
    public string? Observacao { get; set; }

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? AtualizadoEm { get; set; }

    public List<InventarioProcessador> Processadores { get; set; } = new();
    public List<InventarioMonitor> Monitores { get; set; } = new();
    public List<InventarioItemMemoriaQuantidade> MemoriasQuantidades { get; set; } = new();
    public List<InventarioItemArmazenamentoQuantidade> ArmazenamentosQuantidades { get; set; } = new();
    public List<InventarioItemPerifericoQuantidade> PerifericosQuantidades { get; set; } = new();
    public List<InventarioItemCaboQuantidade> CabosQuantidades { get; set; } = new();
    public List<InventarioChaveLicenca> ChavesLicencas { get; set; } = new();
}
