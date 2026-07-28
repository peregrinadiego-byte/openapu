using System.Globalization;
using System.Text;
using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;

namespace OpenAPU.Application.Exports;

public static class CsvExportService
{
    public static byte[] ExportApus(
        IReadOnlyCollection<ApuResult> apus)
    {
        var builder = new StringBuilder();

        builder.AppendLine(
            "Clave APU,Nombre APU,Unidad,Costo directo");

        foreach (var apu in apus)
        {
            AppendRow(
                builder,
                apu.Key,
                apu.Name,
                apu.Unit,
                Decimal(apu.DirectCost));
        }

        return WithUtf8Bom(builder.ToString());
    }

    public static byte[] ExportBudgets(
        IReadOnlyCollection<BudgetResult> budgets)
    {
        var builder = new StringBuilder();

        builder.AppendLine(
            "Clave presupuesto,Nombre presupuesto,Clave concepto,Nombre concepto,Cantidad,Precio unitario,Importe partida,Total presupuesto");

        foreach (var budget in budgets)
        {
            if (budget.Items.Count == 0)
            {
                AppendRow(
                    builder,
                    budget.Key,
                    budget.Name,
                    "",
                    "",
                    "",
                    "",
                    "",
                    Decimal(budget.Total));

                continue;
            }

            foreach (var item in budget.Items)
            {
                AppendRow(
                    builder,
                    budget.Key,
                    budget.Name,
                    item.ConceptKey,
                    item.ConceptName,
                    Decimal(item.Quantity),
                    Decimal(item.UnitPrice),
                    Decimal(item.Total),
                    Decimal(budget.Total));
            }
        }

        return WithUtf8Bom(builder.ToString());
    }

    private static string Decimal(decimal value)
    {
        return value.ToString(
            "0.####",
            CultureInfo.InvariantCulture);
    }

    private static void AppendRow(
        StringBuilder builder,
        params string[] values)
    {
        builder.AppendLine(
            string.Join(",", values.Select(Escape)));
    }

    private static string Escape(string value)
    {
        var safe = value ?? "";

        if (safe.Contains('"'))
        {
            safe = safe.Replace("\"", "\"\"");
        }

        return safe.IndexOfAny([',', '"', '\r', '\n']) >= 0
            ? $"\"{safe}\""
            : safe;
    }

    private static byte[] WithUtf8Bom(string content)
    {
        var encoding = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: true);

        var preamble = encoding.GetPreamble();
        var body = encoding.GetBytes(content);
        var result = new byte[preamble.Length + body.Length];

        Buffer.BlockCopy(
            preamble,
            0,
            result,
            0,
            preamble.Length);

        Buffer.BlockCopy(
            body,
            0,
            result,
            preamble.Length,
            body.Length);

        return result;
    }
}
