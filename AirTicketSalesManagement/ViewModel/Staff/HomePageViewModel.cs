using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public partial class HomePageViewModel : BaseViewModel
    {
        public Func<double, string> YFormatter { get; }
        [ObservableProperty] private int ticketsSoldToday;
        [ObservableProperty] private int ticketsCanceledToday;
        [ObservableProperty] private int flightsDepartedToday;
        [ObservableProperty] private string greeting;

        public SeriesCollection RevenueSeries { get; } = new();
        public Axis RevenueAxisX { get; }
        public string[] RevenueLabels { get; private set; } = Array.Empty<string>();
        

        public HomePageViewModel()
        {            
            YFormatter = v => v.ToString("N0") + " ₫";
            RevenueAxisX = new Axis
            {
                Title = "Ngày",
                Labels = new List<string>(),
                Separator = new Separator { Step = 1 }
            };
            Greeting = $"Chào {UserSession.Current?.CustomerName ?? "bạn"}! Hôm nay là {DateTime.Now:dd/MM/yyyy}";
            LoadTodayStats();
            LoadRevenue7Days();
        }

        private async void LoadTodayStats()
        {
            try
            {
                using(var context = new AirTicketDbContext())
                {
                    var today = DateOnly.FromDateTime(DateTime.Now);
                    var start = today.ToDateTime(TimeOnly.MinValue);
                    var end = start.AddDays(1);

                    var sold = await context.Ctdvs
                        .Where(c =>
                            c.MaDvNavigation != null &&                     
                            c.MaDvNavigation.TtdatVe == "Đã thanh toán" && 
                            c.MaDvNavigation.ThoiGianDv >= start &&
                            c.MaDvNavigation.ThoiGianDv < end)
                        .CountAsync();

                    var canceled = await context.Ctdvs
                        .Where(c =>
                            c.MaDvNavigation != null &&
                            c.MaDvNavigation.TtdatVe == "Đã huỷ" &&
                            c.MaDvNavigation.ThoiGianDv >= start &&
                            c.MaDvNavigation.ThoiGianDv < end)
                        .CountAsync();
                    var flights = await context.Lichbays
                        .Where(lb => lb.GioDi >= start && lb.GioDi < end)
                        .CountAsync();
                    TicketsSoldToday = sold;
                    TicketsCanceledToday = canceled;
                    FlightsDepartedToday = flights;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading today's stats: {ex.Message}");
            }
        }
        private async void LoadRevenue7Days()
        {
            try
            {
                using var ctx = new AirTicketDbContext();

                var today = DateOnly.FromDateTime(DateTime.Now);
                var start = today.AddDays(-6);
                var end = today.AddDays(1);

                var revDict = await ctx.Datves
                    .Where(d => d.TtdatVe == "Đã thanh toán"
                             && d.ThoiGianDv >= start.ToDateTime(TimeOnly.MinValue)
                             && d.ThoiGianDv < end.ToDateTime(TimeOnly.MinValue))
                    .GroupBy(d => DateOnly.FromDateTime(d.ThoiGianDv!.Value))
                    .Select(g => new { g.Key, Sum = g.Sum(d => d.TongTienTt ?? 0) })
                    .ToDictionaryAsync(x => x.Key, x => x.Sum);

                var labels = new List<string>();
                var values = new ChartValues<double>();

                for (int i = 0; i < 7; i++)
                {
                    var day = start.AddDays(i);
                    labels.Add(day.ToString("dd/MM"));
                    values.Add((double)Math.Round(revDict.TryGetValue(day, out var v) ? v : 0, 2));
                }

                // ==== CẬP NHẬT TRÊN UI‑THREAD ====
                App.Current.Dispatcher.Invoke(() =>
                {
                    // 1. Update labels - KHÔNG tạo List mới
                    RevenueAxisX.Labels.Clear();
                    foreach (var label in labels)
                    {
                        RevenueAxisX.Labels.Add(label);
                    }

                    // 2. Update series
                    RevenueSeries.Clear();
                    RevenueSeries.Add(new ColumnSeries
                    {
                        Title = "Doanh thu",
                        Values = values,
                        Fill = new SolidColorBrush(Color.FromRgb(33, 149, 242)),
                        MaxColumnWidth = 60
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Revenue error: {ex.Message}");
            }
        }
    }
}
