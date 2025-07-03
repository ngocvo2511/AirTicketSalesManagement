using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.PDF
{
    public abstract class BaseReportDocument : IReportDocument, IDocument
    {
        private readonly string _filePath;
        protected readonly string CompanyName;
        protected readonly string? LogoPath;

        protected BaseReportDocument(string filePath, string companyName, string? logoPath)
        {
            _filePath = filePath;
            CompanyName = companyName;
            LogoPath = logoPath;
        }

        public void Generate(string filePath) => Document.Create(container => Compose(container)).GeneratePdf(filePath);

        DocumentMetadata IDocument.GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.DefaultTextStyle(TextStyle.Default.FontFamily("Times"));

                ComposeHeader(page);
                ComposeContent(page);
                ComposeFooter(page);
            });
        }

        protected virtual void ComposeHeader(PageDescriptor page)
        {
            page.Header().Column(column =>
            {
                column.Item().Row(row =>
                {
                    if (!string.IsNullOrEmpty(LogoPath) && File.Exists(LogoPath))
                    {
                        row.RelativeColumn(1).Height(50).AlignLeft().Image(LogoPath!, ImageScaling.FitHeight);
                    }
                    else
                    {
                        row.RelativeColumn(1);
                    }

                    row.RelativeColumn(5).AlignCenter().Column(inner =>
                    {
                        inner.Item().Text(CompanyName).Bold().FontSize(16).FontColor(Colors.Blue.Medium);
                        inner.Item().Text(GetTitle()).SemiBold().FontSize(14);
                    });
                });

                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });
        }

        protected abstract void ComposeContent(PageDescriptor page);

        protected virtual void ComposeFooter(PageDescriptor page)
        {
            page.Footer().AlignCenter().Text(txt =>
            {
                txt.Span("Xuất ngày: ").SemiBold();
                txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            });
        }

        protected abstract string GetTitle();
    }
}
