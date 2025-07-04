using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models.ReportModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class ReportViewModel : BaseViewModel
    {
        [ObservableProperty] private bool isYearlyReport = true;
        [ObservableProperty] private bool isMonthlyReport;
        partial void OnIsYearlyReportChanged(bool oldValue, bool newValue)
        {
            if (newValue)         
            {
                IsMonthlyReport = false;   
                Refresh();         
            }
        }

        // Khi IsMonthlyReport đổi
        partial void OnIsMonthlyReportChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                IsYearlyReport = false;
                Refresh();
            }
        }

        [ObservableProperty] private IEnumerable<int> years;
        [ObservableProperty] private int selectedYear;
        [ObservableProperty] private int selectedMonth;
        [ObservableProperty] private IEnumerable<MonthItem> months;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private ReportSummaryModel? reportSummary;
        [ObservableProperty] private ObservableCollection<YearlyReportItem> yearlyReportData = new();
        [ObservableProperty] private ObservableCollection<MonthlyReportItem> monthlyReportData = new();

        public ReportViewModel()
        {
            Years = Enumerable.Range(DateTime.Now.Year-10, 11).Reverse().ToList();
            SelectedYear = Years.FirstOrDefault();
            Months = Enumerable.Range(1, 12)
                .Select(i => new MonthItem { Name = $"Tháng {i}", Value = i }).ToList();
            SelectedMonth = 1;
            IsYearlyReport = true;
            IsMonthlyReport = false;
        }

        [RelayCommand]
        private async void GenerateReportAsync()
        {
            Debug.Print("Generating report...");
            IsLoading = true;
            using(var context = new AirTicketDbContext())
            {
                CultureInfo culture = CultureInfo.GetCultureInfo("vi-VN");
                if (IsYearlyReport)
                {
                    var revenueQuery =
                       from dv in context.Datves
                       where dv.TtdatVe == "Đã thanh toán"
                             && dv.MaLbNavigation != null
                             && dv.MaLbNavigation.GioDi.Value.Year == SelectedYear
                       group dv by dv.MaLbNavigation.GioDi.Value.Month into g
                       select new
                       {
                           Month = g.Key,                     
                           Revenue = g.Sum(x => x.TongTienTt ?? 0)
                       };

                    var flightQuery =
                       from lb in context.Lichbays
                       where lb.GioDi.Value.Year == selectedYear
                       group lb by lb.GioDi.Value.Month into g
                       select new
                       {
                           Month = g.Key,
                           Flights = g.Count()
                       };
                    var revenue = await revenueQuery.ToListAsync();
                    var flights = await flightQuery.ToListAsync();

                    var report = Enumerable.Range(1, 12)
                        .Select(m =>
                        {
                            var r = revenue.FirstOrDefault(x => x.Month == m);
                            var fl = flights.FirstOrDefault(x => x.Month == m);

                            return new YearlyReportItem
                            {
                                Year = SelectedYear,
                                MonthName = culture.DateTimeFormat.GetMonthName(m),
                                Revenue = r?.Revenue ?? 0,
                                TotalFlights = fl?.Flights ?? 0
                            };
                        })
                        .ToList();
                    YearlyReportData = new ObservableCollection<YearlyReportItem>(report);
                    var totalRevenue = report.Sum(r => r.Revenue);
                    var totalFlights = report.Sum(r => r.TotalFlights);
                    foreach (var item in report)
                    {
                        item.RevenueRate = totalRevenue == 0 ? 0 : Math.Round(item.Revenue / totalRevenue * 100, 2);
                    }
                    ReportSummary = new ReportSummaryModel
                    {
                        TotalRevenue = totalRevenue,
                        TotalFlights = totalFlights
                    };
                }
                else
                {
                    var query =
                    from lb in context.Lichbays
                    where lb.GioDi.Value.Year == SelectedYear && lb.GioDi.Value.Month == SelectedMonth
                    join cb in context.Chuyenbays on lb.SoHieuCb equals cb.SoHieuCb
                    join dv in context.Datves on lb.MaLb equals dv.MaLb into datvesGroup
                    select new
                    {
                        FlightNumber = lb.SoHieuCb ?? "",
                        Airline = lb.SoHieuCbNavigation.HangHangKhong ?? "",
                        DepartureTime = lb.GioDi,
                        TicketsSold = datvesGroup
                            .Where(dv => dv.TtdatVe == "Đã thanh toán")
                            .Count(),
                        Revenue = datvesGroup
                            .Where(dv => dv.TtdatVe == "Đã thanh toán")
                            .Sum(dv => dv.TongTienTt ?? 0)
                    };

                    var rawResult = await query.ToListAsync();
                    decimal totalRevenue = rawResult.Sum(r => r.Revenue);
                    var report = rawResult.Select(r => new MonthlyReportItem
                    {
                        Month = SelectedMonth,
                        Year = selectedYear,
                        FlightNumber = r.FlightNumber,
                        Airline = r.Airline,
                        DepartureTime = r.DepartureTime.Value,
                        TicketsSold = r.TicketsSold,
                        Revenue = r.Revenue,
                        RevenueRate = totalRevenue == 0 ? 0 : Math.Round(r.Revenue / totalRevenue * 100, 2)
                    }).ToList();

                    MonthlyReportData = new ObservableCollection<MonthlyReportItem>(report);
                    ReportSummary = new ReportSummaryModel
                    {
                        TotalRevenue = totalRevenue,
                        TotalFlights = report.Count,
                        TotalTicketsSold = report.Sum(x => x.TicketsSold)
                    };
                }
            }           
            IsLoading = false;
        }

        [RelayCommand]
        private void Refresh()
        {
            YearlyReportData.Clear();
            MonthlyReportData.Clear();
            ReportSummary = null;
        }

        [RelayCommand]
        private void ExportReport()
        {
            string filename;
            if (IsYearlyReport)
            {
                if(YearlyReportData.IsNullOrEmpty())
                {
                    return;
                }
                filename = $"Báo cáo năm {YearlyReportData.First().Year}";
                
            }
            else
            {
                if(MonthlyReportData.IsNullOrEmpty())
                {
                    return;
                }
                filename = $"Báo cáo tháng {MonthlyReportData.First().Month} năm {MonthlyReportData.First().Year}";
                
            }
            var dialog = new SaveFileDialog
            {
                FileName = filename,
                DefaultExt = ".pdf",
                Filter = "PDF files (*.xlsx)|*.xlsx",
                Title = "Chọn nơi lưu báo cáo"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string filePath = dialog.FileName;
                if(IsMonthlyReport)
                {
                    ExcelExporter.ExportMonthlyReportToExcel(MonthlyReportData, MonthlyReportData.First().Month, MonthlyReportData.First().Year, filePath);
                }
                else
                {
                    ExcelExporter.ExportYearlyReportToExcel(YearlyReportData, YearlyReportData.First().Year, filePath);
                }
            }
        }
    }
}
