using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventariosistemasoperacionais")]
public class InventarioSistemaOperacional
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Nome { get; set; } = string.Empty;
}
