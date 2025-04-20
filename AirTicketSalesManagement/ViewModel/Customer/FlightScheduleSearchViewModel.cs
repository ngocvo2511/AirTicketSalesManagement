using AirTicketSalesManagement.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class FlightScheduleSearchViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string diemDi;
        [ObservableProperty]
        private string diemDen;
        [ObservableProperty]
        private DateTime? ngayDi;
        [ObservableProperty]
        private DateTime? _ngayVe;
        [ObservableProperty]
        private int soLuongGhe = 1;
        [ObservableProperty]
        private bool isRoundTrip;
        [ObservableProperty]
        private Visibility resultVisibility = Visibility.Collapsed;
        [ObservableProperty]
        private ObservableCollection<KQTraCuuChuyenBay> flightResults = new();
        [RelayCommand]
        private void SearchFlight()
        {
            // TODO: Thay bằng truy vấn DB thực tế
            FlightResults.Clear();

            if (string.IsNullOrWhiteSpace(DiemDi) || string.IsNullOrWhiteSpace(DiemDen) || NgayDi == null)
                return;

            FlightResults.Add(new KQTraCuuChuyenBay
            {
                MaSBDi = "SGN",
                MaSBDen = "HAN",
                HangHangKhong = "Vietnam Airlinesss",
                GioDi = new TimeSpan(8, 0, 0), // 08:00 AM
                GioDen = new TimeSpan(9, 30, 0), // 09:30 AM
                ThoiGianBay = new TimeSpan(1, 30, 0), // 1 hour 30 minutes
                MayBay = "Airbus A321",
                GiaVe = 1290000
            });

            ResultVisibility = Visibility.Visible;
        }    
    }
}
