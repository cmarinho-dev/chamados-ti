using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChamadosTI.Models;

public class InventarioRelatoriosViewModel
{
    public string Aba { get; set; } = "geral";
    public InventarioTipoEquipamento? TipoEquipamento { get; set; }
    public int? SetorId { get; set; }
    public string? Responsavel { get; set; }
    public bool SomenteBackup { get; set; }
    public bool SemPatrimonio { get; set; }
    public bool SemResponsavel { get; set; }
    public bool ComMonitor { get; set; }
    public bool ComChave { get; set; }
    public InventarioChaveTipo? TipoChave { get; set; }

    public int TotalEquipamentos { get; set; }
    public int TotalComputadores { get; set; }
    public int TotalNotebooks { get; set; }
    public int TotalOutros { get; set; }
    public int TotalMonitoresVinculados { get; set; }
    public int TotalSemResponsavel { get; set; }
    public int TotalSemPatrimonio { get; set; }

    public List<InventarioRelatorioLinhaViewModel> Linhas { get; set; } = new();
    public List<InventarioRelatorioConformidadeViewModel> Conformidade { get; set; } = new();
    public List<InventarioRelatorioLicencaViewModel> Licencas { get; set; } = new();
    public List<InventarioRelatorioMonitorViewModel> Monitores { get; set; } = new();

    public List<SelectListItem> Setores { get; set; } = new();
    public List<SelectListItem> TiposEquipamento { get; set; } = new();
    public List<SelectListItem> TiposChave { get; set; } = new();
}

public class InventarioRelatorioLinhaViewModel
{
    public int Id { get; set; }
    public string InventarioNumero { get; set; } = string.Empty;
    public string? Patrimonio { get; set; }
    public string TipoEquipamento { get; set; } = string.Empty;
    public InventarioTipoEquipamento TipoEquipamentoValor { get; set; }
    public string? Setor { get; set; }
    public string? Responsavel { get; set; }
    public bool EhBackup { get; set; }
    public int Monitores { get; set; }
    public int Chaves { get; set; }
    public int Armazenamentos { get; set; }
    public string? Observacao { get; set; }
    public string? Ip { get; set; }
    public string PaginaBusca => TipoEquipamentoValor == InventarioTipoEquipamento.Computador
        || TipoEquipamentoValor == InventarioTipoEquipamento.Notebook
            ? "Computadores"
            : "OutrosDispositivos";
}

public class InventarioRelatorioConformidadeViewModel
{
    public string InventarioNumero { get; set; } = string.Empty;
    public string Problema { get; set; } = string.Empty;
}

public class InventarioRelatorioLicencaViewModel
{
    public string Tipo { get; set; } = string.Empty;
    public int EmUso { get; set; }
    public int Livres { get; set; }
}

public class InventarioRelatorioMonitorViewModel
{
    public string InventarioMonitor { get; set; } = string.Empty;
    public string? Modelo { get; set; }
    public int Vinculos { get; set; }
}
