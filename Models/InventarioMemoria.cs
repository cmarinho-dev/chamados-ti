using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventariomemorias")]
public class InventarioMemoria
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Descricao { get; set; } = string.Empty;

}
