using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
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

        public FlightScheduleSearchViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadSanBay();
            }
        }

        [ObservableProperty]
        private string diemDi;

        [ObservableProperty]
        private string diemDen;

        [ObservableProperty]
        private DateTime? ngayDi;

        [ObservableProperty]
        private DateTime? ngayVe;

        [ObservableProperty]
        private bool isRoundTrip;

        [ObservableProperty]
        private Visibility resultVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private ObservableCollection<string> sanBayList = new();


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
            }
        }

        [ObservableProperty]
        private ObservableCollection<KQTraCuuChuyenBay> flightResults = new();

        // Hành khách properties
        [ObservableProperty]
        private int adultCount = 1;

        [ObservableProperty]
        private int childCount = 0;

        [ObservableProperty]
        private int infantCount = 0;

        [ObservableProperty]
        private bool isPassengerSelectorOpen = false;

        // Tổng số hành khách
        public int TotalPassengers => AdultCount + ChildCount + InfantCount;

        // Chuỗi hiển thị tóm tắt số lượng hành khách
        public string PassengerSummary
        {
            get
            {
                string summary = $"{TotalPassengers} hành khách";
                return summary;
            }

            set
            {
                // Không cần thiết phải làm gì ở đây
            }
        }

        // Mở popup chọn hành khách
        [RelayCommand]
        private void OpenPassengerSelector()
        {

            IsPassengerSelectorOpen = true;
        }

        // Áp dụng lựa chọn hành khách
        [RelayCommand]
        private void ApplyPassengerSelection()
        {
            // Đóng popup
            IsPassengerSelectorOpen = false;

            // Cập nhật các thuộc tính phụ thuộc
            OnPropertyChanged(nameof(TotalPassengers));
            OnPropertyChanged(nameof(PassengerSummary));
        }

        // Tăng số lượng người lớn
        [RelayCommand]
        private void IncreaseAdult()
        {
            // Tối đa 9 người lớn + trẻ em
            if (adultCount + childCount < 9)
            {
                adultCount++;
                OnPropertyChanged(nameof(AdultCount));
                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        // Giảm số lượng người lớn
        [RelayCommand]
        private void DecreaseAdult()
        {
            // Tối thiểu 1 người lớn và không ít hơn số em bé
            if (adultCount > 1 )
            {
                adultCount--;
                OnPropertyChanged(nameof(AdultCount));


                // Nếu số em bé vượt quá số người lớn, điều chỉnh số em bé
                if (infantCount > adultCount)
                {
                    infantCount = adultCount;
                    OnPropertyChanged(nameof(InfantCount));

                }

                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        // Tăng số lượng trẻ em (2-12 tuổi)
        [RelayCommand]
        private void IncreaseChild()
        {
            // Tối đa 9 hành khách người lớn + trẻ em
            if (adultCount + childCount < 9)
            {
                childCount++;
                OnPropertyChanged(nameof(ChildCount));
                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        // Giảm số lượng trẻ em (2-12 tuổi)
        [RelayCommand]
        private void DecreaseChild()
        {
            // Không được nhỏ hơn 0
            if (childCount > 0)
            {
                childCount--;
                OnPropertyChanged(nameof(ChildCount));
                OnPropertyChanged(nameof(PassengerSummary));

            }
        }

        // Tăng số lượng em bé (dưới 2 tuổi)
        [RelayCommand]
        private void IncreaseInfant()
        {
            // Số em bé không được vượt quá số người lớn
            if (infantCount < adultCount)
            {
                infantCount++;
                OnPropertyChanged(nameof(InfantCount));
                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        // Giảm số lượng em bé (dưới 2 tuổi)
        [RelayCommand]
        private void DecreaseInfant()
        {
            // Không được nhỏ hơn 0
            if (infantCount > 0)
            {
                infantCount--;
                OnPropertyChanged(nameof(InfantCount));
                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        [RelayCommand]
        private void SearchFlight()
        {
            // TODO: Thay bằng truy vấn DB thực tế
            FlightResults.Clear();
            if (string.IsNullOrWhiteSpace(DiemDi) || string.IsNullOrWhiteSpace(DiemDen) || NgayDi == null)
                return;

            // Thêm chuyến bay thẳng
            FlightResults.Add(new KQTraCuuChuyenBay
            {
                MaSBDi = "SGN",
                MaSBDen = "HAN",
                HangHangKhong = "Vietnam Airlines",
                GioDi = new TimeSpan(8, 0, 0), // 08:00 AM
                GioDen = new TimeSpan(9, 30, 0), // 09:30 AM
                ThoiGianBay = new TimeSpan(1, 30, 0), // 1 hour 30 minutes
                MayBay = "Airbus A321",
                GiaVe = 1290000,
                SoSanBayTrungGian = 0 // Bay thẳng
            });

            // Thêm chuyến bay có 1 điểm dừng
            FlightResults.Add(new KQTraCuuChuyenBay
            {
                MaSBDi = "SGN",
                MaSBDen = "HAN",
                HangHangKhong = "VietJet Air",
                GioDi = new TimeSpan(9, 0, 0),
                GioDen = new TimeSpan(12, 30, 0),
                ThoiGianBay = new TimeSpan(3, 30, 0),
                MayBay = "Airbus A320",
                GiaVe = 950000,
                SoSanBayTrungGian = 1 // 1 điểm dừng
            });

            // Thêm chuyến bay có 2 điểm dừng
            FlightResults.Add(new KQTraCuuChuyenBay
            {
                MaSBDi = "SGN",
                MaSBDen = "HAN",
                HangHangKhong = "Bamboo Airways",
                GioDi = new TimeSpan(7, 30, 0),
                GioDen = new TimeSpan(13, 0, 0),
                ThoiGianBay = new TimeSpan(5, 30, 0),
                MayBay = "Boeing 787",
                GiaVe = 890000,
                SoSanBayTrungGian = 2 // 2 điểm dừng
            });

            ResultVisibility = Visibility.Visible;
        }
    }
}