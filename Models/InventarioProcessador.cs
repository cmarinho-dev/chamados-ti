using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventarioprocessadores")]
public class InventarioProcessador
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Descricao { get; set; } = string.Empty;

    public List<InventarioItem> InventarioItems { get; set; } = new();
}
