using AirTicketSalesManagement.Models.ReportModel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using NPOI.XSSF.UserModel;

namespace AirTicketSalesManagement.PDF
{
    public sealed class YearlyReportDocument : BaseReportDocument
    {
        private readonly IReadOnlyList<YearlyReportItem> _items;
        private readonly int _year;

        public YearlyReportDocument(IReadOnlyList<YearlyReportItem> items, int year, string companyName = "ABC Aviation JSC", string? logoPath = null, string filePath = "BaoCaoNam.pdf")
            : base(filePath, companyName, logoPath)
        {
            _items = items;
            _year = year;
            Generate(filePath);
        }

        protected override string GetTitle() => $"BÁO CÁO DOANH THU NĂM {_year}";

        protected override void ComposeContent(PageDescriptor page)
        {
            page.Content()
                .Border(1) // Khung ngoài toàn bảng
                .Table(table =>
                {
                    // 1. Định nghĩa các cột
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Tháng
                        columns.RelativeColumn(2); // Số chuyến
                        columns.RelativeColumn(3); // Doanh thu
                        columns.RelativeColumn(3); // % doanh thu
                    });

                    // 2. Header
                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Tháng");
                        header.Cell().Element(HeaderCell).Text("Số chuyến");
                        header.Cell().Element(HeaderCell).Text("Doanh thu");
                        header.Cell().Element(HeaderCell).Text("% Doanh Thu");
                    });

                    // 3. Dữ liệu từng dòng
                    for (int i = 0; i < _items.Count; i++)
                    {
                        var item = _items[i];
                        var bgColor = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                        table.Cell().Element(c => DataCell(c, bgColor, HorizontalAlignment.Left)).Text(item.MonthName).AlignLeft();
                        table.Cell().Element(c => DataCell(c, bgColor, HorizontalAlignment.Right)).Text(item.TotalFlights.ToString()).AlignRight();
                        table.Cell().Element(c => DataCell(c, bgColor, HorizontalAlignment.Right)).Text($"{item.Revenue:N0} ₫").AlignRight();
                        table.Cell().Element(c => DataCell(c, bgColor, HorizontalAlignment.Right)).Text(item.RevenueRate.ToString("N2")).AlignRight();
                    }

                    // 4. Tổng
                    table.Cell().ColumnSpan(2).Element(TotalCell).AlignRight().Text("Tổng");
                    table.Cell().Element(TotalCell).AlignRight().Text($"{_items.Sum(x => x.Revenue):N0} ₫");
                    table.Cell().Element(TotalCell); // % doanh thu không tính
                });

            // ================================
            // Các hàm phụ (local function)
            // ================================

            static IContainer HeaderCell(IContainer container) => container
                .Padding(5)
                .BorderRight(1).BorderBottom(1)
                .BorderColor(Colors.Black)
                .Background(Colors.Grey.Lighten2)
                .AlignCenter()
                .DefaultTextStyle(x => x.SemiBold());

            static IContainer DataCell(IContainer container, string bgColor, HorizontalAlignment align) => container
                .Padding(5)
                .BorderRight(1).BorderBottom(1)
                .BorderColor(Colors.Black)
                .Background(bgColor);

            static IContainer TotalCell(IContainer container) => container
                .Padding(5)
                .BorderRight(1).BorderBottom(1)
                .BorderColor(Colors.Black)
                .Background(Colors.Grey.Lighten3)
                .DefaultTextStyle(x => x.SemiBold());
        }
    }
}