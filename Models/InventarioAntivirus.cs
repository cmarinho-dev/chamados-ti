using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventarioantivirus")]
public class InventarioAntivirus
{
    public int Id { get; set; }

    [Required, StringLength(40)]
    public string Nome { get; set; } = string.Empty;
}
