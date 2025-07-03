using AirTicketSalesManagement.Models.ReportModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class ReportViewModel : BaseViewModel
    {
        [ObservableProperty] private bool isYearlyReport = true;
        [ObservableProperty] private bool isMonthlyReport;
        [ObservableProperty] private IEnumerable<int> years;
        [ObservableProperty] private int selectedYear;
        [ObservableProperty] private IEnumerable<MonthItem> months;
        [ObservableProperty] private int selectedMonth;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private ReportSummaryModel? reportSummary;
        [ObservableProperty] private ObservableCollection<YearlyReportItem> yearlyReportData = new();
        [ObservableProperty] private ObservableCollection<MonthlyReportItem> monthlyReportData = new();

        public ReportViewModel()
        {
            // Khởi tạo dữ liệu mẫu
            Years = Enumerable.Range(2020, 6).ToList();
            SelectedYear = Years.FirstOrDefault();
            Months = Enumerable.Range(1, 12)
                .Select(i => new MonthItem { Name = $"Tháng {i}", Value = i }).ToList();
            SelectedMonth = 1;
            IsYearlyReport = true;
            IsMonthlyReport = false;
        }

        [RelayCommand]
        private void GenerateReport()
        {
            IsLoading = true;
            // TODO: Load dữ liệu thật từ DB
            if (IsYearlyReport)
            {
                // Giả lập dữ liệu
                YearlyReportData = new ObservableCollection<YearlyReportItem>
                {
                    new YearlyReportItem { MonthName = "Tháng 1", Revenue = 10000000, TotalFlights = 12, RevenueRate = 55.6f },
                    // ...
                };
                ReportSummary = new ReportSummaryModel
                {
                    TotalRevenue = YearlyReportData.Sum(x => x.Revenue),
                    TotalFlights = YearlyReportData.Sum(x => x.TotalFlights)
                };
            }
            else
            {
                MonthlyReportData = new ObservableCollection<MonthlyReportItem>
                {
                    new MonthlyReportItem
                    {
                        FlightNumber = "VN123",
                        Airline = "Vietnam Airlines",
                        DepartureTime = DateTime.Now,
                        TicketsSold = 150,
                        Revenue = 50000000,
                        RevenueRate = 77.8f
                    }
                    // ...
                };
                ReportSummary = new ReportSummaryModel
                {
                    TotalRevenue = MonthlyReportData.Sum(x => x.Revenue),
                    TotalFlights = MonthlyReportData.Count,
                    TotalTicketsSold = MonthlyReportData.Sum(x => x.TicketsSold)
                };
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
            // TODO: Xuất báo cáo ra file
        }
    }
}
