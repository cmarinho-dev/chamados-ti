using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("administrativousuarios")]
public class AdministrativoUsuario
{
    public int Id { get; set; }

    [Required, StringLength(160)]
    public string Nome { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Usuario { get; set; } = string.Empty;

    public int DepartamentoId { get; set; }
    public InventarioSetor? Departamento { get; set; }
}
