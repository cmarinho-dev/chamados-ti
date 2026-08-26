using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChamadosTI.Models;

[Table("tecnicosti")]
public class TecnicoTi
{
    public int Id { get; set; }

    [Required, StringLength(160)]
    public string Nome { get; set; } = string.Empty;

    public List<Chamado> Chamados { get; set; } = [];
}
