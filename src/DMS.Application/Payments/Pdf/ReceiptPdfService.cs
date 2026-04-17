using Abp.Dependency;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DMS.Payments.Pdf;

public class ReceiptPdfService : IReceiptPdfService, ITransientDependency
{
    public byte[] Generate(ReceiptPdfModel model)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(header => ComposeHeader(header, model));
                page.Content().Element(content => ComposeContent(content, model));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Thank you for your payment — ");
                    text.Span(model.ReceiptNumber).Bold();
                });
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, ReceiptPdfModel model)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                if (!string.IsNullOrEmpty(model.CompanyLogoPath) && System.IO.File.Exists(model.CompanyLogoPath))
                    col.Item().MaxHeight(60).Image(model.CompanyLogoPath);
                col.Item().Text(model.CompanyName).FontSize(14).Bold();
                col.Item().Text(model.CompanyAddress).FontColor(Colors.Grey.Darken1);
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text("PAYMENT RECEIPT").FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                col.Item().Text($"Receipt #: {model.ReceiptNumber}").Bold();
                col.Item().Text($"Date: {model.PaymentDate}");
            });
        });
    }

    private static void ComposeContent(IContainer container, ReceiptPdfModel model)
    {
        container.Column(col =>
        {
            col.Spacing(12);

            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("BILLED TO").Bold().FontColor(Colors.Grey.Darken2);
                    c.Item().Text(model.CustomerName);
                    c.Item().Text(model.CustomerAddress).FontColor(Colors.Grey.Darken1);
                });

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("INVOICE REF").Bold().FontColor(Colors.Grey.Darken2);
                    c.Item().Text($"Invoice #: {model.InvoiceNumber}");
                    c.Item().Text($"Invoice Total: {model.InvoiceTotal:N2} EGP");
                    c.Item().Text($"Amount Due Before: {(model.InvoiceTotal - model.PaidBeforeThis):N2} EGP");
                });
            });

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Method").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Reference").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Amount").Bold();
                });

                foreach (var line in model.Lines)
                {
                    table.Cell().Padding(4).Text(line.MethodName);
                    table.Cell().Padding(4).Text(line.Reference ?? "—");
                    table.Cell().Padding(4).AlignRight().Text($"{line.Amount:N2} EGP");
                }
            });

            col.Item().AlignRight().Column(c =>
            {
                c.Item().Row(r =>
                {
                    r.RelativeItem().Text("TOTAL PAID").Bold();
                    r.ConstantItem(120).AlignRight().Text($"{model.TotalPaid:N2} EGP").Bold();
                });
                c.Item().Row(r =>
                {
                    r.RelativeItem().Text("REMAINING BALANCE");
                    r.ConstantItem(120).AlignRight().Text($"{model.RemainingBalance:N2} EGP");
                });
                c.Item().Text($"Status: {model.InvoiceStatus}").Bold()
                    .FontColor(model.RemainingBalance == 0 ? Colors.Green.Darken2 : Colors.Orange.Darken2);
            });
        });
    }
}
