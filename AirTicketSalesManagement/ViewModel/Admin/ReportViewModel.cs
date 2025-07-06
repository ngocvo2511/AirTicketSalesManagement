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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class ReportViewModel : BaseViewModel
    {
        [ObservableProperty] private bool isYearlyReport = true;
        [ObservableProperty] private bool isMonthlyReport;
        public SeriesCollection YearlySeries { get; set; }
        public SeriesCollection MonthlySeries { get; set; }

        public AxesCollection YearlyAxesX { get; set; }
        public AxesCollection MonthlyAxesX { get; set; }
        private double monthlyChartWidth;
        public double MonthlyChartWidth
        {
            get => monthlyChartWidth;
            set => SetProperty(ref monthlyChartWidth, value);
        }


        public string[] MonthsLabels { get; set; }
        public string[] FlightsLabels { get; set; }

        public Func<double, string> VndFormatter => v => v.ToString("N0") + " ₫";

        public NotificationViewModel Notification { get; } = new NotificationViewModel();
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
            Years = Enumerable.Range(DateTime.Now.Year - 10, 11).Reverse().ToList();
            SelectedYear = Years.FirstOrDefault();
            Months = Enumerable.Range(1, 12)
                .Select(i => new MonthItem { Name = $"Tháng {i}", Value = i }).ToList();
            SelectedMonth = 1;
            IsYearlyReport = true;
            IsMonthlyReport = false;
            YearlyAxesX = new AxesCollection();
            MonthlyAxesX = new AxesCollection();
            YearlySeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<double>()
                }
            };
            MonthsLabels = Array.Empty<string>();
            MonthlySeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<double>()
                }
            };           
            FlightsLabels = Array.Empty<string>();
            MonthlyChartWidth = 800;
        }

        [RelayCommand]
        private async void GenerateReportAsync()
        {
            Debug.Print("Generating report...");
            IsLoading = true;
            using (var context = new AirTicketDbContext())
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
                    UpdateYearlyChart();
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
                    UpdateMonthlyChart();
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

            YearlyAxesX.Clear();
            MonthlyAxesX.Clear();
            MonthlySeries.Clear();
            MonthlySeries.Add(new ColumnSeries
            {
                Title = "Doanh thu",
                Values = new ChartValues<double>()
            });

            FlightsLabels = Array.Empty<string>();
            OnPropertyChanged(nameof(FlightsLabels));

            YearlySeries.Clear();
            YearlySeries.Add(new ColumnSeries
            {
                Title = "Doanh thu",
                Values = new ChartValues<double>()
            });
            MonthsLabels = Array.Empty<string>();
            OnPropertyChanged(nameof(MonthsLabels));
            MonthlyChartWidth = 800;
            ReportSummary = null;
        }

        [RelayCommand]
        private async void ExportReport()
        {
            string filename;
            if (IsYearlyReport)
            {
                if (YearlyReportData.IsNullOrEmpty())
                {
                    await Notification.ShowNotificationAsync("Không có dữ liệu để xuất báo cáo năm!", NotificationType.Warning);
                    return;
                }
                filename = $"Báo cáo năm {YearlyReportData.First().Year}";
            }
            else
            {
                if (MonthlyReportData.IsNullOrEmpty())
                {
                    await Notification.ShowNotificationAsync("Không có dữ liệu để xuất báo cáo tháng!", NotificationType.Warning);
                    return;
                }
                filename = $"Báo cáo tháng {MonthlyReportData.First().Month} năm {MonthlyReportData.First().Year}";
            }

            var dialog = new SaveFileDialog
            {
                FileName = filename,
                DefaultExt = ".xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Chọn nơi lưu báo cáo"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string filePath = dialog.FileName;
                if (IsMonthlyReport)
                {
                    ExcelExporter.ExportMonthlyReportToExcel(MonthlyReportData, MonthlyReportData.First().Month, MonthlyReportData.First().Year, filePath);
                }
                else
                {
                    ExcelExporter.ExportYearlyReportToExcel(YearlyReportData, YearlyReportData.First().Year, filePath);
                }

                await Notification.ShowNotificationAsync("Xuất báo cáo thành công!", NotificationType.Information);
            }
        }
        // Add these methods to ReportViewModel

        private void UpdateYearlyChart()
        {
            MonthsLabels = Enumerable.Range(1, 12)
                             .Select(i => $"T{i}")
                             .ToArray();
            OnPropertyChanged(nameof(MonthsLabels));
            YearlySeries[0].Values = new ChartValues<double>(
                                         YearlyReportData.Select(r => (double)r.Revenue));
            YearlyAxesX.Clear();
            YearlyAxesX.Add(new Axis
            {
                Labels = MonthsLabels,
                Separator = new Separator { Step = 1 },
                LabelsRotation = 0
            });
        }

        private void UpdateMonthlyChart()
        {
            MonthlySeries[0].Values = new ChartValues<double>(MonthlyReportData.Select(r => (double)r.Revenue));

            FlightsLabels = MonthlyReportData.Select(r => r.FlightNumber).ToArray();
            OnPropertyChanged(nameof(FlightsLabels));
            int numFlights = MonthlyReportData.Count;
            int widthPerBar = 80;
            MonthlyChartWidth = Math.Max(800, numFlights * widthPerBar);

            MonthlyAxesX.Clear();
            MonthlyAxesX.Add(new Axis
            {
                Labels = FlightsLabels,
                Separator = new Separator
                {
                    Step = 1
                },
                LabelsRotation = 0
            });
        }

    }
}
