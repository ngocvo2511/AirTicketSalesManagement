﻿using AirTicketSalesManagement.Data;
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

namespace AirTicketSalesManagement.ViewModel.Booking
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

        // Constructor overload để nhận thông tin tìm kiếm từ HomePage
        public FlightScheduleSearchViewModel(SearchFlightParameters searchParams)
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadSanBay();
                
                // Điền thông tin từ HomePage
                if (searchParams != null)
                {
                    DiemDi = searchParams.DiemDi;
                    DiemDen = searchParams.DiemDen;
                    NgayDi = searchParams.NgayDi;
                    AdultCount = searchParams.SoLuongGhe;
                    
                    // Đảm bảo các property đã được cập nhật trước khi gọi SearchFlight
                    OnPropertyChanged(nameof(DiemDi));
                    OnPropertyChanged(nameof(DiemDen));
                    OnPropertyChanged(nameof(NgayDi));
                    OnPropertyChanged(nameof(AdultCount));
                    OnPropertyChanged(nameof(PassengerSummary));
                    
                    // Tự động gọi SearchFlight sau khi điền thông tin
                    // Sử dụng Task.Delay để đảm bảo UI đã được cập nhật
                    Task.Delay(100).ContinueWith(_ =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SearchFlight();
                        });
                    });
                }
            }
        }

        [ObservableProperty]
        private string diemDi;

        [ObservableProperty]
        private string diemDen;

        [ObservableProperty]
        private DateTime? ngayDi;


        [ObservableProperty]
        private Visibility resultVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private ObservableCollection<string> sanBayList = new();

        [ObservableProperty]
        private DateTime minBookingDate;

        [ObservableProperty]
        private int tuoiToiDaSoSinh = 2;
        [ObservableProperty]
        private int tuoiToiDaTreEm = 12;

        public string DoTuoiTreEmText { get; set; }


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
                var quyDinh = context.Quydinhs.FirstOrDefault();
                TuoiToiDaSoSinh = quyDinh.TuoiToiDaSoSinh ?? 2;
                TuoiToiDaTreEm = quyDinh.TuoiToiDaTreEm ?? 12;
                DoTuoiTreEmText = $"Từ {TuoiToiDaSoSinh} đến {TuoiToiDaTreEm} tuổi";
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
        private async Task SelectTicketClass(ThongTinChuyenBayDuocChon selection)
        {
            if (selection == null || selection.TicketClass == null || selection.Flight == null)
                return;
            int idDatVe;

            using (var context = new AirTicketDbContext())
            {
                var datVe = new Datve
                {
                    MaLb = selection.Flight.MaLichBay,
                    MaKh = UserSession.Current.CustomerId,
                    MaNv = UserSession.Current.StaffId,
                    Slve = TotalPassengers,
                    TongTienTt = TotalPassengers * selection.TicketClass.GiaVe,
                    TtdatVe = "Giữ chỗ",
                    ThoiGianDv = DateTime.Now
                };
                context.Datves.Add(datVe);
                context.SaveChanges();

                // Giảm vé
                var hangVe = context.Hangvetheolichbays.First(h => h.MaHvLb == selection.TicketClass.MaHangVe);
                hangVe.SlveConLai -= TotalPassengers;

                context.SaveChanges();
                idDatVe = datVe.MaDv;
                // Lưu lại datVe.MaDV vào session để sử dụng sau
            }
            await Task.Delay(500); // Giả lập thời gian xử lý
            // Tạo đối tượng chứa thông tin chuyến bay và hạng vé đã chọn
            var selectedFlightInfo = new ThongTinChuyenBayDuocChon
            {
                Id = idDatVe,
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

            IsPassengerSelectorOpen = !IsPassengerSelectorOpen;
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
            if (AdultCount + ChildCount < 9)
            {
                AdultCount++;
                OnPropertyChanged(nameof(AdultCount));
                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        // Giảm số lượng người lớn
        [RelayCommand]
        private void DecreaseAdult()
        {
            // Tối thiểu 1 người lớn và không ít hơn số em bé
            if (AdultCount > 1)
            {
                AdultCount--;
                OnPropertyChanged(nameof(AdultCount));


                // Nếu số em bé vượt quá số người lớn, điều chỉnh số em bé
                if (InfantCount > AdultCount)
                {
                    InfantCount = AdultCount;
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
            if (AdultCount + ChildCount < 9)
            {
                ChildCount++;
                OnPropertyChanged(nameof(ChildCount));
                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        // Giảm số lượng trẻ em (2-12 tuổi)
        [RelayCommand]
        private void DecreaseChild()
        {
            // Không được nhỏ hơn 0
            if (ChildCount > 0)
            {
                ChildCount--;
                OnPropertyChanged(nameof(ChildCount));
                OnPropertyChanged(nameof(PassengerSummary));

            }
        }

        // Tăng số lượng em bé (dưới 2 tuổi)
        [RelayCommand]
        private void IncreaseInfant()
        {
            // Số em bé không được vượt quá số người lớn
            if (InfantCount < AdultCount)
            {
                InfantCount++;
                OnPropertyChanged(nameof(InfantCount));
                OnPropertyChanged(nameof(PassengerSummary));
            }
        }

        // Giảm số lượng em bé (dưới 2 tuổi)
        [RelayCommand]
        private void DecreaseInfant()
        {
            // Không được nhỏ hơn 0
            if (InfantCount > 0)
            {
                InfantCount--;
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
            ClearExpiredHolds();

            SearchedAdultCount = AdultCount;
            SearchedChildCount = ChildCount;
            SearchedInfantCount = InfantCount;
            FlightResults.Clear();
            HasSearched = true; // Đánh dấu đã tìm kiếm

            // Kiểm tra điều kiện đầu vào
            if (string.IsNullOrWhiteSpace(DiemDi) || string.IsNullOrWhiteSpace(DiemDen) || NgayDi == null)
                return;

            using (var context = new AirTicketDbContext())
            {
                var quiDinh = context.Quydinhs.FirstOrDefault();
                int tgDatVe = quiDinh?.TgdatVeChamNhat ?? 1;
                // Truy vấn danh sách chuyến bay
                var flights = context.Lichbays
                    .Include(lb => lb.SoHieuCbNavigation) // Bao gồm thông tin chuyến bay
                        .ThenInclude(cb => cb.SbdiNavigation) // Bao gồm thông tin sân bay đi
                    .Include(lb => lb.SoHieuCbNavigation.SbdenNavigation) // Bao gồm thông tin sân bay đến
                    .Include(lb => lb.SoHieuCbNavigation.Sanbaytrunggians) // Bao gồm thông tin sân bay trung gian
                    .Where(lb =>
                        lb.SoHieuCbNavigation.SbdiNavigation.MaSb == ExtractMaSB(DiemDi) &&
                        lb.SoHieuCbNavigation.SbdenNavigation.MaSb == ExtractMaSB(DiemDen) &&
                        lb.GioDi.Value.Date == NgayDi.Value.Date && DateTime.Now <= lb.GioDi.Value.AddDays(-tgDatVe))
                    .ToList();

                foreach (var flight in flights)
                {
                    // Lấy danh sách hạng vé
                    var availableTicketClasses = context.Hangvetheolichbays
                        .Include(hvlb => hvlb.MaHvNavigation)
                        .Where(hvlb => hvlb.MaLb == flight.MaLb && (hvlb.SlveConLai ?? 0) >= TotalPassengers)
                        .ToList();

                    if (!availableTicketClasses.Any())
                        continue;

                    var ticketClassList = availableTicketClasses
                        .Select(hvlb => new HangVe
                        {
                            MaHangVe = hvlb.MaHvLb,
                            TenHangVe = hvlb.MaHvNavigation.TenHv,
                            GiaVe = flight.GiaVe.Value * (decimal)hvlb.MaHvNavigation.HeSoGia,
                            SoGheConLai = hvlb.SlveConLai ?? 0,
                            BackgroundColor = GetBackgroundColorForTicketClass(hvlb.MaHvNavigation.TenHv),
                            HeaderColor = GetHeaderColorForTicketClass(hvlb.MaHvNavigation.TenHv),
                            ButtonColor = GetButtonColorForTicketClass(hvlb.MaHvNavigation.TenHv)
                        })
                        .ToObservableCollection();

                    // Thêm kết quả chuyến bay
                    FlightResults.Add(new KQTraCuuChuyenBayMoRong
                    {
                        MaSBDi = flight.SoHieuCbNavigation.SbdiNavigation.MaSb,
                        MaSBDen = flight.SoHieuCbNavigation.SbdenNavigation.MaSb,
                        DiemDi = flight.SoHieuCbNavigation.SbdiNavigation.ThanhPho,
                        DiemDen = flight.SoHieuCbNavigation.SbdenNavigation.ThanhPho,
                        MaLichBay = flight.MaLb,
                        HangHangKhong = flight.SoHieuCbNavigation.HangHangKhong,
                        NgayDi = flight.GioDi.Value.Date,
                        GioDi = flight.GioDi.Value.TimeOfDay,
                        GioDen = flight.GioDen.Value.TimeOfDay,
                        ThoiGianBay = flight.GioDen.Value - flight.GioDi.Value,
                        MayBay = flight.LoaiMb,
                        GiaVe = flight.GiaVe.Value,
                        SoSanBayTrungGian = flight.SoHieuCbNavigation.Sanbaytrunggians.Count,
                        LogoUrl = GetAirlineLogo(flight.SoHieuCbNavigation.HangHangKhong), // Thay bằng logo thực tế nếu có
                        TicketClasses = ticketClassList
                    });
                }
            }

            // Hiển thị kết quả
            ResultVisibility = Visibility.Visible; // Luôn hiển thị kết quả sau khi tìm kiếm
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

        public void ClearExpiredHolds()
        {
            using (var context = new AirTicketDbContext())
            {
                var expiredDatVes = context.Datves
                    .Where(dv => (dv.TtdatVe == "Chưa thanh toán (Online)" || dv.TtdatVe == "Giữ chỗ") &&
                                 dv.ThoiGianDv < DateTime.Now.AddMinutes(-20))
                    .ToList();

                foreach (var datVe in expiredDatVes)
                {
                    var chiTiets = context.Ctdvs.Where(ct => ct.MaDv == datVe.MaDv).ToList();

                    var maHvLb = chiTiets.FirstOrDefault()?.MaHvLb;

                    if (maHvLb != null)
                    {
                        var hangVe = context.Hangvetheolichbays
                            .FirstOrDefault(h => h.MaHvLb == maHvLb);

                        if (hangVe != null)
                        {
                            hangVe.SlveConLai += chiTiets.Count;
                        }
                    }

                    context.Ctdvs.RemoveRange(chiTiets);
                    context.Datves.Remove(datVe);
                }

                context.SaveChanges();
            }
        }

        private string GetAirlineLogo(string airlineName)
        {
            if (string.IsNullOrWhiteSpace(airlineName))
                return "/Resources/Images/default.png";


            if (airlineName == "Vietnam Airlines")
                return "/Resources/Images/vietnamair.png";
            if (airlineName == "Vietjet Air")
                return "/Resources/Images/vietjet.png";
            if (airlineName == "Bamboo Airways")
                return "/Resources/Images/bamboo.jpg";
            if (airlineName == "Vietravel Airlines")
                return "/Resources/Images/vietravel.png";

            return "/Images/default.png";
        }

        [ObservableProperty]
        private bool isSearchExpanded = true;

        [ObservableProperty]
        private double searchContentHeight = double.NaN;

        [ObservableProperty]
        private bool hasSearched = false;

        [RelayCommand]
        private void ToggleSearch()
        {
            IsSearchExpanded = !IsSearchExpanded;
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