using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Booking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class HomePageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string diemDi;

        [ObservableProperty]
        private string diemDen;

        [ObservableProperty]
        private DateTime? ngayDi;

        [ObservableProperty]
        private int soLuongGhe = 1;

        [ObservableProperty]
        private ObservableCollection<string> sanBayList = new();

        [ObservableProperty]
        private DateTime minBookingDate;

        public HomePageViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadSanBay();
            }
        }

        // Danh sách dùng để binding cho điểm đi (lọc bỏ điểm đến)
        public ObservableCollection<string> DiemDiList =>
            new(SanBayList.Where(s => s != DiemDen));

        // Danh sách dùng để binding cho điểm đến (lọc bỏ điểm đi)
        public ObservableCollection<string> DiemDenList =>
            new(SanBayList.Where(s => s != DiemDi));

        partial void OnDiemDiChanged(string value)
        {
            OnPropertyChanged(nameof(DiemDenList));
        }

        partial void OnDiemDenChanged(string value)
        {
            OnPropertyChanged(nameof(DiemDiList));
        }

        public void LoadSanBay()
        {
            using (var context = new AirTicketDbContext()) // Hoặc dùng SqlConnection nếu ADO.NET
            {
                var danhSach = context.Sanbays
                            .AsEnumerable() // chuyển sang LINQ to Objects
                            .Select(sb => $"{sb.ThanhPho} ({sb.MaSb}), {sb.QuocGia}")
                            .OrderBy(display => display)
                            .ToList();
                SanBayList = new ObservableCollection<string>(danhSach);
                var quiDinh = context.Quydinhs.FirstOrDefault();
                int tgDatVe = quiDinh?.TgdatVeChamNhat ?? 1;
                MinBookingDate = DateTime.Now.AddDays(tgDatVe);
            }
        }

        [RelayCommand]
        public void SearchFlight()
        {
            // Kiểm tra điều kiện đầu vào
            if (string.IsNullOrWhiteSpace(DiemDi) || string.IsNullOrWhiteSpace(DiemDen) || NgayDi == null)
            {
                // Hiển thị thông báo lỗi nếu cần
                return;
            }

            // Kiểm tra ngày đi không được nhỏ hơn ngày hiện tại
            if (NgayDi.Value.Date < DateTime.Now.Date)
            {
                // Hiển thị thông báo lỗi nếu cần
                return;
            }

            // Tạo đối tượng chứa thông tin tìm kiếm
            var searchParams = new SearchFlightParameters
            {
                DiemDi = DiemDi,
                DiemDen = DiemDen,
                NgayDi = NgayDi,
                SoLuongGhe = SoLuongGhe
            };

            // Chuyển sang FlightScheduleSearchView với thông tin đã nhập
            NavigationService.NavigateTo<FlightScheduleSearchViewModel>(searchParams);
        }

    }
}
