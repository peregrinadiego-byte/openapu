using System.Globalization;
using System.Net;
using System.Text;
using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;

namespace OpenAPU.Application.Reports;

public static class HtmlReportService
{
    public static string CreateApuSummary(
        IReadOnlyCollection<ApuResult> apus)
    {
        var rows = new StringBuilder();

        foreach (var apu in apus)
        {
            rows.AppendLine(
                $"""
                <tr>
                    <td>{Encode(apu.Key)}</td>
                    <td>{Encode(apu.Name)}</td>
                    <td>{Encode(apu.Unit)}</td>
                    <td class="number">{Money(apu.DirectCost)}</td>
                </tr>
                """);
        }

        return Document(
            "Resumen de APU",
            $"""
            <header>
                <h1>Resumen de análisis de precios unitarios</h1>
                <p>Generado por OpenAPU · {DateTimeOffset.Now:dd/MM/yyyy HH:mm}</p>
            </header>

            <table>
                <thead>
                    <tr>
                        <th>Clave</th>
                        <th>Nombre</th>
                        <th>Unidad</th>
                        <th>Costo directo</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>

            <footer>
                Total de APU: {apus.Count}
            </footer>
            """);
    }

    public static string CreateBudgetDetail(
        BudgetResult budget)
    {
        var rows = new StringBuilder();

        foreach (var item in budget.Items)
        {
            rows.AppendLine(
                $"""
                <tr>
                    <td>{Encode(item.ConceptKey)}</td>
                    <td>{Encode(item.ConceptName)}</td>
                    <td class="number">{Decimal(item.Quantity)}</td>
                    <td class="number">{Money(item.UnitPrice)}</td>
                    <td class="number">{Money(item.Total)}</td>
                </tr>
                """);
        }

        return Document(
            $"Presupuesto {budget.Key}",
            $"""
            <header>
                <h1>{Encode(budget.Name)}</h1>
                <p>Clave: <strong>{Encode(budget.Key)}</strong></p>
                <p>Generado por OpenAPU · {DateTimeOffset.Now:dd/MM/yyyy HH:mm}</p>
            </header>

            <table>
                <thead>
                    <tr>
                        <th>Clave</th>
                        <th>Concepto</th>
                        <th>Cantidad</th>
                        <th>Precio unitario</th>
                        <th>Importe</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
                <tfoot>
                    <tr>
                        <th colspan="4">Total</th>
                        <th class="number">{Money(budget.Total)}</th>
                    </tr>
                </tfoot>
            </table>

            <footer>
                Total de partidas: {budget.Items.Count}
            </footer>
            """);
    }

    private static string Document(
        string title,
        string body)
    {
        return
            $$"""
            <!doctype html>
            <html lang="es">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>{{Encode(title)}}</title>
                <style>
                    :root {
                        font-family: Arial, Helvetica, sans-serif;
                        color: #111;
                    }

                    body {
                        margin: 32px auto;
                        max-width: 1100px;
                        padding: 0 24px;
                    }

                    header {
                        margin-bottom: 28px;
                    }

                    h1 {
                        margin: 0 0 8px;
                        font-size: 26px;
                    }

                    p {
                        margin: 4px 0;
                    }

                    table {
                        width: 100%;
                        border-collapse: collapse;
                    }

                    th,
                    td {
                        padding: 10px 8px;
                        border: 1px solid #bbb;
                        text-align: left;
                    }

                    th {
                        background: #eee;
                    }

                    .number {
                        text-align: right;
                        white-space: nowrap;
                    }

                    footer {
                        margin-top: 20px;
                        font-size: 13px;
                    }

                    .toolbar {
                        display: flex;
                        justify-content: flex-end;
                        margin-bottom: 18px;
                    }

                    button {
                        padding: 9px 14px;
                        border: 1px solid #333;
                        background: #fff;
                        cursor: pointer;
                    }

                    @media print {
                        body {
                            margin: 0;
                            max-width: none;
                            padding: 0;
                        }

                        .toolbar {
                            display: none;
                        }

                        thead {
                            display: table-header-group;
                        }

                        tr {
                            break-inside: avoid;
                        }
                    }
                </style>
            </head>
            <body>
                <div class="toolbar">
                    <button onclick="window.print()">Imprimir</button>
                </div>
                {{body}}
            </body>
            </html>
            """;
    }

    private static string Encode(string value)
    {
        return WebUtility.HtmlEncode(value);
    }

    private static string Money(decimal value)
    {
        return value.ToString(
            "C2",
            CultureInfo.GetCultureInfo("es-MX"));
    }

    private static string Decimal(decimal value)
    {
        return value.ToString(
            "0.####",
            CultureInfo.GetCultureInfo("es-MX"));
    }
}
