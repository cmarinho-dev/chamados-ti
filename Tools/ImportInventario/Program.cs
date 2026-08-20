using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length < 2)
{
    Console.WriteLine("Uso: dotnet run -- <caminho_sql> <saida_sql>");
    return;
}

var inputPath = args[0];
var outputPath = args[1];

if (!File.Exists(inputPath))
{
    Console.WriteLine($"Arquivo nao encontrado: {inputPath}");
    return;
}

var content = File.ReadAllText(inputPath);
var insertRegex = new Regex(@"INSERT INTO `inventario` VALUES\s*(.+?);", RegexOptions.Singleline | RegexOptions.IgnoreCase);
var matches = insertRegex.Matches(content);

if (matches.Count == 0)
{
    Console.WriteLine("Nenhum INSERT de inventario encontrado.");
    return;
}

var setores = BuildSetorMap();
var sistemasOperacionais = BuildSistemaOperacionalMap();
var offices = BuildOfficeMap();
var antiviruses = BuildAntivirusMap();
var conexoes = BuildConexaoMap();

var itemValues = new List<string>();
var monitorValues = new List<string>();
var warnings = new List<string>();

foreach (Match match in matches)
{
    var valuesBlock = match.Groups[1].Value;
    foreach (var tuple in ParseTuples(valuesBlock))
    {
        if (tuple.Count < 25)
        {
            warnings.Add($"Registro ignorado (campos insuficientes): {string.Join("|", tuple)}");
            continue;
        }

        var id = ToInt(tuple[0]);
        var setorRaw = tuple[1];
        var inventarioNumero = SafeTrim(tuple[2]);
        if (string.IsNullOrWhiteSpace(inventarioNumero))
        {
            warnings.Add($"Registro ignorado (inventario vazio). Id={id}");
            continue;
        }

        var setorId = ResolveId(setores, setorRaw, 30, warnings, "Setor", id);
        var sistemaId = ResolveId(sistemasOperacionais, tuple[7], null, warnings, "SistemaOperacional", id);
        var officeId = ResolveId(offices, tuple[8], null, warnings, "Office", id);
        var antivirusId = ResolveId(antiviruses, tuple[19], null, warnings, "Antivirus", id);
        var conexaoId = ResolveId(conexoes, tuple[18], null, warnings, "Conexao", id);

        var criadoEm = ParseDate(tuple[20]) ?? DateTimeOffset.UtcNow;

        var observacao = NormalizeOptional(tuple[21]);
        if (string.IsNullOrWhiteSpace(observacao))
        {
            observacao = null;
        }

        itemValues.Add($"({id}, {SqlString(inventarioNumero)}, {SqlString(NormalizeOptional(tuple[10]))}, {SqlString(NormalizeOptional(tuple[3]))}, {SqlString(NormalizeOptional(tuple[4]))}, {SqlString(NormalizeOptional(tuple[5]))}, {SqlString(NormalizeOptional(tuple[6]))}, {SqlString(NormalizeOptional(tuple[9]))}, {setorId}, {SqlNullable(sistemaId)}, {SqlNullable(officeId)}, {SqlNullable(antivirusId)}, {SqlNullable(conexaoId)}, {SqlString(NormalizeOptional(tuple[24]))}, {SqlString(observacao)}, {SqlDate(criadoEm)}, NULL)");

        var monitor1Inventario = NormalizeOptional(tuple[12]);
        var monitor1Marca = NormalizeOptional(tuple[13]);
        var monitor1Polegadas = NormalizeOptional(tuple[14]);
        if (HasMonitor(monitor1Inventario, monitor1Marca, monitor1Polegadas))
        {
            monitorValues.Add($"({id}, 1, {SqlString(monitor1Inventario)}, {SqlString(monitor1Marca)}, {SqlString(monitor1Polegadas)})");
        }

        var monitor2Inventario = NormalizeOptional(tuple[15]);
        var monitor2Marca = NormalizeOptional(tuple[16]);
        var monitor2Polegadas = NormalizeOptional(tuple[17]);
        if (HasMonitor(monitor2Inventario, monitor2Marca, monitor2Polegadas))
        {
            monitorValues.Add($"({id}, 2, {SqlString(monitor2Inventario)}, {SqlString(monitor2Marca)}, {SqlString(monitor2Polegadas)})");
        }
    }
}

var sb = new StringBuilder();
sb.AppendLine("SET FOREIGN_KEY_CHECKS=0;");
sb.AppendLine();

if (itemValues.Count > 0)
{
    sb.AppendLine("INSERT INTO InventarioItems (Id, InventarioNumero, Patrimonio, Usuario, MemoriaRam, Processador, Geracao, Armazenamento, SetorId, SistemaOperacionalId, OfficeId, AntivirusId, ConexaoId, Ip, Observacao, CriadoEm, AtualizadoEm) VALUES");
    sb.AppendLine(string.Join(",\n", itemValues) + ";");
    sb.AppendLine();
}

if (monitorValues.Count > 0)
{
    sb.AppendLine("INSERT INTO InventarioMonitores (InventarioItemId, Numero, InventarioNumero, Marca, Polegadas) VALUES");
    sb.AppendLine(string.Join(",\n", monitorValues) + ";");
    sb.AppendLine();
}

sb.AppendLine("SET FOREIGN_KEY_CHECKS=1;");

File.WriteAllText(outputPath, sb.ToString(), new UTF8Encoding(false));

Console.WriteLine($"Arquivo gerado: {outputPath}");
if (warnings.Count > 0)
{
    Console.WriteLine("Avisos:");
    foreach (var warning in warnings)
    {
        Console.WriteLine($"- {warning}");
    }
}

static List<List<string?>> ParseTuples(string valuesBlock)
{
    var tuples = new List<List<string?>>();
    var i = 0;
    while (i < valuesBlock.Length)
    {
        if (valuesBlock[i] == '(')
        {
            i++;
            var fields = new List<string?>();
            var sb = new StringBuilder();
            var inString = false;
            while (i < valuesBlock.Length)
            {
                var c = valuesBlock[i];
                if (inString)
                {
                    if (c == '\\' && i + 1 < valuesBlock.Length)
                    {
                        sb.Append(valuesBlock[i + 1]);
                        i += 2;
                        continue;
                    }

                    if (c == '\'')
                    {
                        inString = false;
                        i++;
                        continue;
                    }

                    sb.Append(c);
                    i++;
                    continue;
                }

                if (c == '\'')
                {
                    inString = true;
                    i++;
                    continue;
                }

                if (c == ',')
                {
                    fields.Add(NormalizeToken(sb.ToString()));
                    sb.Clear();
                    i++;
                    continue;
                }

                if (c == ')')
                {
                    fields.Add(NormalizeToken(sb.ToString()));
                    sb.Clear();
                    i++;
                    break;
                }

                sb.Append(c);
                i++;
            }

            tuples.Add(fields);
        }
        else
        {
            i++;
        }
    }
    return tuples;
}

static string? NormalizeToken(string token)
{
    var trimmed = token.Trim();
    if (trimmed.Length == 0)
    {
        return null;
    }

    if (string.Equals(trimmed, "null", StringComparison.OrdinalIgnoreCase))
    {
        return null;
    }

    return trimmed;
}

static int ToInt(string? value)
{
    if (int.TryParse(value, out var result))
    {
        return result;
    }
    return 0;
}

static DateTimeOffset? ParseDate(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return null;
    }

    if (DateTimeOffset.TryParseExact(value.Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
    {
        return parsed;
    }

    if (DateTimeOffset.TryParse(value, out parsed))
    {
        return parsed;
    }

    return null;
}

static string? NormalizeOptional(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return null;
    }

    var trimmed = value.Trim();
    var normalized = NormalizeKey(trimmed);
    if (normalized == "NT" || normalized == "N T" || normalized == "SEM INFO")
    {
        return null;
    }

    return trimmed;
}

static string SafeTrim(string? value)
{
    return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
}

static bool HasMonitor(string? inventario, string? marca, string? polegadas)
{
    return !string.IsNullOrWhiteSpace(inventario)
        || !string.IsNullOrWhiteSpace(marca)
        || !string.IsNullOrWhiteSpace(polegadas);
}

static int ResolveId(Dictionary<string, int> map, string? value, int? fallback, List<string> warnings, string label, int id)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        if (fallback.HasValue)
        {
            warnings.Add($"{label} vazio. Id={id} => fallback {fallback.Value}");
            return fallback.Value;
        }
        return 0;
    }

    var key = NormalizeKey(value);
    if (key.Contains("NAO TEM", StringComparison.Ordinal)
        || key.Contains("NA O TEM", StringComparison.Ordinal)
        || key.Contains("N AO TEM", StringComparison.Ordinal)
        || key.Contains("N A O TEM", StringComparison.Ordinal)
        || key.Contains("N O TEM", StringComparison.Ordinal))
    {
        key = "NAO TEM";
    }
    if (map.TryGetValue(key, out var mapped))
    {
        return mapped;
    }

    if (fallback.HasValue)
    {
        warnings.Add($"{label} nao encontrado '{value}'. Id={id} => fallback {fallback.Value}");
        return fallback.Value;
    }

    warnings.Add($"{label} nao encontrado '{value}'. Id={id} => NULL");
    return 0;
}

static string NormalizeKey(string value)
{
    var normalized = value.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder();
    foreach (var c in normalized)
    {
        var uc = CharUnicodeInfo.GetUnicodeCategory(c);
        if (uc == UnicodeCategory.NonSpacingMark)
        {
            continue;
        }

        if (char.IsLetterOrDigit(c))
        {
            sb.Append(char.ToUpperInvariant(c));
        }
        else
        {
            sb.Append(' ');
        }
    }

    var collapsed = Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
    return collapsed;
}

static string SqlString(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return "NULL";
    }

    var escaped = value.Replace("'", "''");
    return $"'{escaped}'";
}

static string SqlDate(DateTimeOffset value)
{
    return $"'{value:yyyy-MM-dd HH:mm:ss}'";
}

static string SqlNullable(int? value)
{
    return value.HasValue && value.Value > 0 ? value.Value.ToString(CultureInfo.InvariantCulture) : "NULL";
}

static Dictionary<string, int> BuildSetorMap()
{
    return new Dictionary<string, int>
    {
        ["PRESIDENCIA"] = 1,
        ["SUPERINTENDENCIA"] = 2,
        ["ASSESSORIA TECNICA"] = 3,
        ["APAF"] = 4,
        ["APDI"] = 6,
        ["EAP"] = 7,
        ["PARQUE BARIGUI"] = 8,
        ["BARIGUI"] = 34,
        ["NIT"] = 15,
        ["ASSESSORIA DE COMUNICACAO"] = 16,
        ["ASSESSORIA JURIDICA"] = 18,
        ["APPA"] = 21,
        ["APPLI"] = 22,
        ["NEAD"] = 23,
        ["ESTAGIO"] = 24,
        ["GABINETE"] = 25,
        ["SEGURO"] = 27,
        ["BIBLIOTECA"] = 28,
        ["EXTERNO"] = 30,
        ["BACKUP"] = 31,
        ["RH"] = 32,
        ["BKP BARIGUI"] = 33,
        ["ESTUDIO"] = 35,
        ["WORKTIBA"] = 36
    };
}

static Dictionary<string, int> BuildSistemaOperacionalMap()
{
    return new Dictionary<string, int>
    {
        ["WINDOWS XP"] = 1,
        ["WINDOWS 7"] = 2,
        ["WINDOWS 10"] = 3,
        ["WINDOWS 11"] = 4,
        ["WINDOWS 8"] = 5,
        ["ARLEQUIM"] = 6,
        ["MACOS"] = 7
    };
}

static Dictionary<string, int> BuildOfficeMap()
{
    return new Dictionary<string, int>
    {
        ["2007"] = 1,
        ["2010"] = 2,
        ["2013"] = 3,
        ["2016"] = 4,
        ["2019"] = 6,
        ["NAO TEM"] = 7,
        ["N AO TEM"] = 7,
        ["N O TEM"] = 7,
        ["N A O TEM"] = 7,
        ["MACOS"] = 8
    };
}

static Dictionary<string, int> BuildAntivirusMap()
{
    return new Dictionary<string, int>
    {
        ["KASPERSKY"] = 1,
        ["NENHUM"] = 2
    };
}

static Dictionary<string, int> BuildConexaoMap()
{
    return new Dictionary<string, int>
    {
        ["CABO DE REDE"] = 1,
        ["WIFI"] = 2,
        ["NT"] = 3,
        ["N T"] = 3
    };
}
