using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
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



        // Hành khách properties
        [ObservableProperty]
        private int adultCount = 1;

        [ObservableProperty]
        private int childCount = 0;

        [ObservableProperty]
        private int infantCount = 0;

        public int SearchedAdultCount { get; private set; }
        public int SearchedChildCount { get; private set; }
        public int SearchedInfantCount { get; private set; }

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

        [RelayCommand]
        private void SelectTicketClass(ThongTinChuyenBayDuocChon selection)
        {
            if (selection == null || selection.TicketClass == null || selection.Flight == null)
                return;

            // Tạo đối tượng chứa thông tin chuyến bay và hạng vé đã chọn
            var selectedFlightInfo = new ThongTinChuyenBayDuocChon
            {
                Flight = selection.Flight,
                TicketClass = selection.TicketClass,
            };

            // Chuyển sang PassengerInformationView và truyền thông tin chuyến bay
            NavigationService.NavigateTo<PassengerInformationViewModel>(selectedFlightInfo);
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
            if (adultCount > 1)
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

        // In FlightScheduleSearchViewModel
        [ObservableProperty]
        private ObservableCollection<KQTraCuuChuyenBayMoRong> flightResults = new();

        [RelayCommand]
        private void ToggleTicketClasses(KQTraCuuChuyenBayMoRong flight)
        {
            flight.IsTicketClassesExpanded = !flight.IsTicketClassesExpanded;
        }

        [RelayCommand]
        private void SearchFlight()
        {
            SearchedAdultCount = AdultCount;
            SearchedChildCount = ChildCount;
            SearchedInfantCount = InfantCount;
            FlightResults.Clear();

            // Kiểm tra điều kiện đầu vào
            if (string.IsNullOrWhiteSpace(DiemDi) || string.IsNullOrWhiteSpace(DiemDen) || NgayDi == null)
                return;

            using (var context = new AirTicketDbContext())
            {
                // Truy vấn danh sách chuyến bay
                var flights = context.Lichbays
                    .Include(lb => lb.SoHieuCbNavigation) // Bao gồm thông tin chuyến bay
                    .ThenInclude(cb => cb.SbdiNavigation) // Bao gồm thông tin sân bay đi
                    .Include(lb => lb.SoHieuCbNavigation.SbdenNavigation) // Bao gồm thông tin sân bay đến
                    .Include(lb => lb.SoHieuCbNavigation.Sanbaytrunggians) // Bao gồm thông tin sân bay trung gian
                    .Where(lb =>
                        lb.SoHieuCbNavigation.SbdiNavigation.MaSb == ExtractMaSB(DiemDi) &&
                        lb.SoHieuCbNavigation.SbdenNavigation.MaSb == ExtractMaSB(DiemDen) &&
                        lb.GioDi.Value.Date == NgayDi.Value.Date)
                    .ToList();

                foreach (var flight in flights)
                {
                    // Lấy danh sách hạng vé
                    var ticketClasses = context.Loaives
                        .Where(lv => lv.MaLb == flight.MaLb)
                        .Select(lv => new HangVe
                        {
                            TenHangVe = lv.HangGhe,
                            GiaVe = flight.GiaVe.Value * (decimal)lv.HeSoGia,
                            SoGheConLai = lv.SlveConLai ?? 0,
                            BackgroundColor = GetBackgroundColorForTicketClass(lv.HangGhe),
                            HeaderColor = GetHeaderColorForTicketClass(lv.HangGhe),
                            ButtonColor = GetButtonColorForTicketClass(lv.HangGhe)
                        })
                        .ToObservableCollection();

                    // Thêm kết quả chuyến bay
                    FlightResults.Add(new KQTraCuuChuyenBayMoRong
                    {
                        MaSBDi = flight.SoHieuCbNavigation.SbdiNavigation.MaSb,
                        MaSBDen = flight.SoHieuCbNavigation.SbdenNavigation.MaSb,
                        HangHangKhong = flight.SoHieuCbNavigation.HangHangKhong,
                        GioDi = flight.GioDi.Value.TimeOfDay,
                        GioDen = flight.GioDen.Value.TimeOfDay,
                        ThoiGianBay = flight.GioDen.Value - flight.GioDi.Value,
                        MayBay = flight.LoaiMb,
                        GiaVe = flight.GiaVe.Value,
                        SoSanBayTrungGian = flight.SoHieuCbNavigation.Sanbaytrunggians.Count,
                        LogoUrl = "/Images/DefaultAirlineLogo.png", // Thay bằng logo thực tế nếu có
                        TicketClasses = ticketClasses
                    });
                }
            }

            // Hiển thị kết quả
            ResultVisibility = FlightResults.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        private string ExtractMaSB(string displayString)
        {
            if (string.IsNullOrWhiteSpace(displayString)) return "";
            int start = displayString.IndexOf('(');
            int end = displayString.IndexOf(')');
            if (start >= 0 && end > start)
                return displayString.Substring(start + 1, end - start - 1);
            return displayString;
        }

        // Phương thức để lấy màu nền dựa trên tên hạng vé
        private static string GetBackgroundColorForTicketClass(string ticketClass)
        {
            return ticketClass.ToUpper() switch
            {
                "PHỔ THÔNG" => "#E0E0E0",
                "PHỔ THÔNG ĐẶC BIỆT" => "#C8E6C9",
                "THƯƠNG GIA" => "#FFD700",
                "ECO" => "#E0E0E0",
                "ECO PLUS" => "#FFCDD2",
                "BAMBOO ECO" => "#E0E0E0",
                "BAMBOO PLUS" => "#C8E6C9",
                "BAMBOO BUSINESS" => "#B2DFDB",
                "BAMBOO PREMIUM" => "#BBDEFB",
                "BAMBOO FIRST" => "#FFD700",
                _ => "#FFFFFF" // Màu mặc định nếu không khớp
            };
        }

        // Phương thức để lấy màu tiêu đề dựa trên tên hạng vé
        private static string GetHeaderColorForTicketClass(string ticketClass)
        {
            return ticketClass.ToUpper() switch
            {
                "PHỔ THÔNG" => "#333333",
                "PHỔ THÔNG ĐẶC BIỆT" => "#2E7D32",
                "THƯƠNG GIA" => "#B8860B",
                "ECO" => "#333333",
                "ECO PLUS" => "#C62828",
                "BAMBOO ECO" => "#333333",
                "BAMBOO PLUS" => "#2E7D32",
                "BAMBOO BUSINESS" => "#00796B",
                "BAMBOO PREMIUM" => "#1565C0",
                "BAMBOO FIRST" => "#B8860B",
                _ => "#000000" // Màu mặc định nếu không khớp
            };
        }

        // Phương thức để lấy màu nút dựa trên tên hạng vé
        private static string GetButtonColorForTicketClass(string ticketClass)
        {
            return ticketClass.ToUpper() switch
            {
                "PHỔ THÔNG" => "#388FF4",
                "PHỔ THÔNG ĐẶC BIỆT" => "#2E7D32",
                "THƯƠNG GIA" => "#B8860B",
                "ECO" => "#F44336",
                "ECO PLUS" => "#C62828",
                "BAMBOO ECO" => "#4CAF50",
                "BAMBOO PLUS" => "#2E7D32",
                "BAMBOO BUSINESS" => "#00796B",
                "BAMBOO PREMIUM" => "#1565C0",
                "BAMBOO FIRST" => "#B8860B",
                _ => "#388FF4" // Màu mặc định nếu không khớp
            };
        }


    }

    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
    }


}