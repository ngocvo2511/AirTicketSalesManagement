using AirTicketSalesManagement.Models.ReportModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;

public static class ExcelExporter
{
    public static void ExportYearlyReportToExcel(IReadOnlyList<YearlyReportItem> items, int year, string filePath)
    {
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet($"Báo cáo {year}");

        var styles = CreateStyles(workbook);

        //title
        var titleRow = sheet.CreateRow(0);
        titleRow.CreateCell(0).SetCellValue($"BÁO CÁO DOANH THU THÁNG NĂM {year}");
        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));

        var titleFont = workbook.CreateFont();
        titleFont.IsBold = true;
        titleFont.FontHeightInPoints = 14;

        var titleStyle = workbook.CreateCellStyle();
        titleStyle.SetFont(titleFont);
        titleStyle.Alignment = HorizontalAlignment.Center;
        titleStyle.VerticalAlignment = VerticalAlignment.Center;

        titleRow.GetCell(0).CellStyle = titleStyle;

        // Header
        var header = sheet.CreateRow(1);
        header.CreateCell(0).SetCellValue("Tháng");
        header.CreateCell(1).SetCellValue("Số chuyến");
        header.CreateCell(2).SetCellValue("Doanh thu");
        header.CreateCell(3).SetCellValue("% Doanh Thu");

        for (int i = 0; i < 4; i++)
            header.GetCell(i).CellStyle = styles.Header;

        // Data
        decimal totalRevenue = items.Sum(x => x.Revenue);
        int totalFlights = items.Sum(x => x.TotalFlights);
        for (int i = 0; i < items.Count; i++)
        {
            var row = sheet.CreateRow(i + 2);
            var item = items[i];

            row.CreateCell(0).SetCellValue(item.MonthName);
            row.CreateCell(1).SetCellValue(item.TotalFlights);
            row.CreateCell(2).SetCellValue((double)item.Revenue);
            row.CreateCell(3).SetCellValue((double)item.RevenueRate);

            row.GetCell(1).CellStyle = styles.Number;
            row.GetCell(2).CellStyle = styles.Money;
            row.GetCell(3).CellStyle = styles.Percent;
        }

        // Total row
        var totalRow = sheet.CreateRow(items.Count + 2);
        totalRow.CreateCell(0).SetCellValue("Tổng");
        totalRow.CreateCell(1).SetCellValue(totalFlights);
        totalRow.CreateCell(2).SetCellValue((double)totalRevenue);
        totalRow.GetCell(0).CellStyle = styles.Header;
        totalRow.GetCell(2).CellStyle = styles.Money;

        for (int i = 0; i < 4; i++) sheet.AutoSizeColumn(i);

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        workbook.Write(fs);
    }

    public static void ExportMonthlyReportToExcel(IReadOnlyList<MonthlyReportItem> items, int month, int year, string filePath)
    {
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet($"Tháng {month} năm {year}");

        var styles = CreateStyles(workbook);
        var dateStyle = workbook.CreateCellStyle();
        dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy HH:mm");

        // Title
        var titleRow = sheet.CreateRow(0);
        titleRow.CreateCell(0).SetCellValue($"BÁO CÁO DOANH THU THÁNG {month} NĂM {year}");
        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 5)); // Gộp 6 ô A1 -> F1

        var titleFont = workbook.CreateFont();
        titleFont.IsBold = true;
        titleFont.FontHeightInPoints = 14;

        var titleStyle = workbook.CreateCellStyle();
        titleStyle.SetFont(titleFont);
        titleStyle.Alignment = HorizontalAlignment.Center;
        titleStyle.VerticalAlignment = VerticalAlignment.Center;

        titleRow.GetCell(0).CellStyle = titleStyle;

        // Header
        var header = sheet.CreateRow(1);
        header.CreateCell(0).SetCellValue("Số hiệu chuyến bay");
        header.CreateCell(1).SetCellValue("Hãng");
        header.CreateCell(2).SetCellValue("Ngày bay");
        header.CreateCell(3).SetCellValue("Số vé bán");
        header.CreateCell(4).SetCellValue("Doanh thu");
        header.CreateCell(5).SetCellValue("Tỉ lệ doanh thu");

        for (int i = 0; i < 6; i++)
            header.GetCell(i).CellStyle = styles.Header;

        // Data
        decimal totalRevenue = 0;
        int totalTicketSold = items.Sum(x => x.TicketsSold);
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var row = sheet.CreateRow(i + 2);

            row.CreateCell(0).SetCellValue(item.FlightNumber);
            row.CreateCell(1).SetCellValue(item.Airline);
            Debug.WriteLine(item.DepartureTime.ToString(CultureInfo.InvariantCulture));
            row.CreateCell(2).SetCellValue(item.DepartureTime);
            row.CreateCell(3).SetCellValue(item.TicketsSold);
            row.CreateCell(4).SetCellValue((double)item.Revenue);
            row.CreateCell(5).SetCellValue((double)item.RevenueRate);

            row.GetCell(2).CellStyle = dateStyle;
            row.GetCell(3).CellStyle = styles.Number;
            row.GetCell(4).CellStyle = styles.Money;
            row.GetCell(5).CellStyle = styles.Percent;

            totalRevenue += item.Revenue;
        }

        var totalRow = sheet.CreateRow(items.Count + 2);
        totalRow.CreateCell(0).SetCellValue("Tổng");
        totalRow.CreateCell(3).SetCellValue(totalTicketSold);
        totalRow.CreateCell(4).SetCellValue((double)totalRevenue);
        totalRow.GetCell(0).CellStyle = styles.Header;
        totalRow.GetCell(4).CellStyle = styles.Money;

        for (int i = 0; i < 6; i++) sheet.AutoSizeColumn(i);

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        workbook.Write(fs);
    }

    private static (ICellStyle Header, ICellStyle Number, ICellStyle Money, ICellStyle Percent) CreateStyles(IWorkbook wb)
    {
        var boldFont = wb.CreateFont();
        boldFont.IsBold = true;

        var headerStyle = wb.CreateCellStyle();
        headerStyle.SetFont(boldFont);
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.BorderBottom = BorderStyle.Thin;
        headerStyle.BorderTop = BorderStyle.Thin;
        headerStyle.BorderLeft = BorderStyle.Thin;
        headerStyle.BorderRight = BorderStyle.Thin;

        var numberStyle = wb.CreateCellStyle();
        numberStyle.DataFormat = wb.CreateDataFormat().GetFormat("0");

        var moneyStyle = wb.CreateCellStyle();
        moneyStyle.DataFormat = wb.CreateDataFormat().GetFormat("#,##0 \"₫\"");

        var percentStyle = wb.CreateCellStyle();
        percentStyle.DataFormat = wb.CreateDataFormat().GetFormat("0.00");

        return (headerStyle, numberStyle, moneyStyle, percentStyle);
    }
}
