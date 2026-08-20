using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventarioitemmemoriasquantidades")]
public class InventarioItemMemoriaQuantidade
{
    public int InventarioItemId { get; set; }
    public InventarioItem? InventarioItem { get; set; }

    public int MemoriaId { get; set; }
    public InventarioMemoria? Memoria { get; set; }

    public int Quantidade { get; set; } = 1;
}

[Table("inventarioitemarmazenamentosquantidades")]
public class InventarioItemArmazenamentoQuantidade
{
    public int InventarioItemId { get; set; }
    public InventarioItem? InventarioItem { get; set; }

    public int ArmazenamentoId { get; set; }
    public InventarioArmazenamento? Armazenamento { get; set; }

    public int Quantidade { get; set; } = 1;
}

[Table("inventarioitemperifericosquantidades")]
public class InventarioItemPerifericoQuantidade
{
    public int InventarioItemId { get; set; }
    public InventarioItem? InventarioItem { get; set; }

    public int PerifericoId { get; set; }
    public InventarioPeriferico? Periferico { get; set; }

    public int Quantidade { get; set; } = 1;
}

[Table("inventarioitemcabosquantidades")]
public class InventarioItemCaboQuantidade
{
    public int InventarioItemId { get; set; }
    public InventarioItem? InventarioItem { get; set; }

    public int CaboId { get; set; }
    public InventarioCabo? Cabo { get; set; }

    public int Quantidade { get; set; } = 1;
}
