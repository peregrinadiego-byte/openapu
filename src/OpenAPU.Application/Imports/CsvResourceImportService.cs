using System.Globalization;
using System.Text;
using OpenAPU.Application.Resources;

namespace OpenAPU.Application.Imports;

public sealed record ResourceImportError(
    int Row,
    string Message);

public sealed record ResourceImportResult(
    int Imported,
    int Rejected,
    IReadOnlyCollection<ResourceImportError> Errors);

public sealed class CsvResourceImportService
{
    private readonly CreateResourceHandler _handler;

    public CsvResourceImportService(
        CreateResourceHandler handler)
    {
        _handler = handler
            ?? throw new ArgumentNullException(nameof(handler));
    }

    public async Task<ResourceImportResult> ImportAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            leaveOpen: true);

        var text = await reader.ReadToEndAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(text))
        {
            return new ResourceImportResult(
                0,
                1,
                [new ResourceImportError(1, "El archivo está vacío.")]);
        }

        var rows = ParseRows(text);

        if (rows.Count < 2)
        {
            return new ResourceImportResult(
                0,
                1,
                [new ResourceImportError(1, "El archivo no contiene datos.")]);
        }

        ValidateHeader(rows[0]);

        var imported = 0;
        var errors = new List<ResourceImportError>();

        for (var index = 1; index < rows.Count; index++)
        {
            var rowNumber = index + 1;
            var values = rows[index];

            if (values.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            try
            {
                if (values.Count != 7)
                {
                    throw new FormatException(
                        "La fila debe contener 7 columnas.");
                }

                var type = ParseType(values[2]);

                if (!decimal.TryParse(
                    values[6],
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var price))
                {
                    var normalized = values[6].Replace(',', '.');

                    if (!decimal.TryParse(
                        normalized,
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out price))
                    {
                        throw new FormatException(
                            "El precio no es válido.");
                    }
                }

                await _handler.HandleAsync(
                    new CreateResourceCommand(
                        values[0].Trim(),
                        values[1].Trim(),
                        type,
                        values[3].Trim(),
                        values[4].Trim(),
                        values[5].Trim(),
                        price),
                    cancellationToken);

                imported++;
            }
            catch (Exception exception) when (
                exception is FormatException or
                OpenAPU.Application.ApplicationException or
                OpenAPU.Domain.DomainException)
            {
                errors.Add(
                    new ResourceImportError(
                        rowNumber,
                        exception.Message));
            }
        }

        return new ResourceImportResult(
            imported,
            errors.Count,
            errors);
    }

    public static byte[] CreateTemplate()
    {
        const string content =
            "Clave,Nombre,Tipo,CodigoUnidad,SimboloUnidad,NombreUnidad,Precio\r\n" +
            "MAT-001,Cemento,Material,KG,kg,Kilogramo,4.50\r\n";

        var encoding = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: true);

        return encoding.GetPreamble()
            .Concat(encoding.GetBytes(content))
            .ToArray();
    }

    private static ResourceTypeDto ParseType(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "material" => ResourceTypeDto.Material,
            "labor" or "mano de obra" or "mano_de_obra" =>
                ResourceTypeDto.Labor,
            "equipment" or "equipo" =>
                ResourceTypeDto.Equipment,
            "auxiliary" or "auxiliar" =>
                ResourceTypeDto.Auxiliary,
            _ => throw new FormatException(
                $"Tipo de recurso no válido: '{value}'.")
        };
    }

    private static void ValidateHeader(
        IReadOnlyList<string> header)
    {
        var expected = new[]
        {
            "Clave",
            "Nombre",
            "Tipo",
            "CodigoUnidad",
            "SimboloUnidad",
            "NombreUnidad",
            "Precio"
        };

        if (header.Count != expected.Length)
        {
            throw new FormatException(
                "El encabezado CSV no tiene 7 columnas.");
        }

        for (var index = 0; index < expected.Length; index++)
        {
            if (!string.Equals(
                Normalize(header[index]),
                Normalize(expected[index]),
                StringComparison.Ordinal))
            {
                throw new FormatException(
                    $"Encabezado no válido en la columna {index + 1}. Se esperaba '{expected[index]}'.");
            }
        }
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace("ó", "o", StringComparison.OrdinalIgnoreCase)
            .Replace("í", "i", StringComparison.OrdinalIgnoreCase)
            .ToLowerInvariant();
    }

    private static List<List<string>> ParseRows(string text)
    {
        var separator = DetectSeparator(text);
        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new StringBuilder();
        var quoted = false;

        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];

            if (character == '"')
            {
                if (quoted &&
                    index + 1 < text.Length &&
                    text[index + 1] == '"')
                {
                    field.Append('"');
                    index++;
                }
                else
                {
                    quoted = !quoted;
                }

                continue;
            }

            if (!quoted && character == separator)
            {
                row.Add(field.ToString());
                field.Clear();
                continue;
            }

            if (!quoted && (character == '\r' || character == '\n'))
            {
                if (character == '\r' &&
                    index + 1 < text.Length &&
                    text[index + 1] == '\n')
                {
                    index++;
                }

                row.Add(field.ToString());
                field.Clear();

                if (row.Any(value => value.Length > 0))
                {
                    rows.Add(row);
                }

                row = [];
                continue;
            }

            field.Append(character);
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }

        return rows;
    }

    private static char DetectSeparator(string text)
    {
        var firstLine = text
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? "";

        return firstLine.Count(character => character == ';') >
               firstLine.Count(character => character == ',')
            ? ';'
            : ',';
    }
}
