using AirTicketSalesManagement.Models.ReportModel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.PDF
{
    public sealed class MonthlyReportDocument : BaseReportDocument
    {
        private readonly IReadOnlyList<MonthlyReportItem> _items;
        private readonly int _year;
        private readonly int _month;

        public MonthlyReportDocument(IReadOnlyList<MonthlyReportItem> items, int year, int month, string companyName = "ABC Aviation JSC", string? logoPath = null, string filePath = "BaoCaoThang.pdf")
            : base(filePath, companyName, logoPath)
        {
            _items = items;
            _year = year;
            _month = month;
            Generate(filePath);
        }

        protected override string GetTitle() => $"BÁO CÁO CHUYẾN BAY THÁNG {_month}/{_year}";

        protected override void ComposeContent(PageDescriptor page)
        {
            page.Content().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(90);
                    c.ConstantColumn(90);
                    c.ConstantColumn(120);
                    c.ConstantColumn(70);
                    c.ConstantColumn(100);
                    c.ConstantColumn(80);
                });

                table.Header(h =>
                {
                    h.Cell().Element(HeaderCell).Text("Số hiệu");
                    h.Cell().Element(HeaderCell).Text("Hãng bay");
                    h.Cell().Element(HeaderCell).Text("Khởi hành");
                    h.Cell().Element(HeaderCell).Text("Số vé");
                    h.Cell().Element(HeaderCell).Text("Doanh thu");
                    h.Cell().Element(HeaderCell).Text("% DT");

                    static IContainer HeaderCell(IContainer container) => container
                        .Padding(5)
                        .Border(1)
                        .BorderColor(Colors.Black)
                        .Background(Colors.Grey.Lighten2)
                        .AlignCenter()
                        .DefaultTextStyle(x => x.SemiBold());
                });

                foreach (var item in _items)
                {
                    table.Cell().Element(CellStyle).Text(item.FlightNumber);
                    table.Cell().Element(CellStyle).Text(item.Airline);
                    table.Cell().Element(CellStyle).Text(item.DepartureTime.ToString("dd/MM/yyyy HH:mm"));
                    table.Cell().Element(CellStyle).Text(item.TicketsSold.ToString());
                    table.Cell().Element(CellStyle).Text($"{item.Revenue:N0} ₫");
                    table.Cell().Element(CellStyle).Text(item.RevenueRate.ToString("N2"));
                }

                static IContainer CellStyle(IContainer container) => container
                    .Padding(5)
                    .Border(1)
                    .BorderColor(Colors.Black);
            });
        }
    }
}
