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

        //// Lưu giá trị tạm thời khi đang chọn
        //private int tempAdultCount;
        //private int tempChildCount;
        //private int tempInfantCount;

        // Tổng số hành khách
        public int TotalPassengers => AdultCount + ChildCount + InfantCount;

        // Chuỗi hiển thị tóm tắt số lượng hành khách
        public string PassengerSummary
        {
            get
            {
                string summary = $"{AdultCount} người lớn";
                if (ChildCount > 0)
                    summary += $", {ChildCount} trẻ em";
                if (InfantCount > 0)
                    summary += $", {InfantCount} em bé";
                return summary;
            }
        }

        // Mở popup chọn hành khách
        [RelayCommand]
        private void OpenPassengerSelector()
        {
            //// Lưu giá trị tạm thời để người dùng có thể hủy việc thay đổi
            //tempAdultCount = AdultCount;
            //tempChildCount = ChildCount;
            //tempInfantCount = InfantCount;

            IsPassengerSelectorOpen = true;
        }

        // Áp dụng lựa chọn hành khách
        [RelayCommand]
        private void ApplyPassengerSelection()
        {
            // Áp dụng các giá trị đã chọn
            //AdultCount = tempAdultCount;
            //ChildCount = tempChildCount;
            //InfantCount = tempInfantCount;

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

            FlightResults.Add(new KQTraCuuChuyenBay
            {
                MaSBDi = "SGN",
                MaSBDen = "HAN",
                HangHangKhong = "Vietnam Airlines",
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