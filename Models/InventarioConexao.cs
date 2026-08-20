using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("inventarioconexoes")]
public class InventarioConexao
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string Nome { get; set; } = string.Empty;
}
