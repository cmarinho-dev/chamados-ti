namespace ChamadosTI.Models;

public class AdminSqlExecutorViewModel
{
    public string Sql { get; set; } = string.Empty;
    public string? Mensagem { get; set; }
    public string? TipoResultado { get; set; }
    public IReadOnlyList<string>? Colunas { get; set; }
    public IReadOnlyList<Dictionary<string, object?>>? Resultados { get; set; }
    public int LinhasAfetadas { get; set; }
}
