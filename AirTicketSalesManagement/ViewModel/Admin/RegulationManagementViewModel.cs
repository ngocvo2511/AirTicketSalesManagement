using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class RegulationManagementViewModel : BaseViewModel
    {
        // Quy định hiện tại
        [ObservableProperty]
        private int maxAirports;
        [ObservableProperty]
        private int minFlightTime;
        [ObservableProperty]
        private int maxStopover;
        [ObservableProperty]
        private int minStopTime;
        [ObservableProperty]
        private int maxStopTime;
        [ObservableProperty]
        private int bookingTime;
        [ObservableProperty]
        private int cancelTime;

        // Trạng thái chỉnh sửa từng trường
        [ObservableProperty]
        private bool isEditingMaxAirports;
        [ObservableProperty]
        private bool isEditingMinFlightTime;
        [ObservableProperty]
        private bool isEditingMaxStopover;
        [ObservableProperty]
        private bool isEditingMinStopTime;
        [ObservableProperty]
        private bool isEditingMaxStopTime;
        [ObservableProperty]
        private bool isEditingBookingTime;
        [ObservableProperty]
        private bool isEditingCancelTime;

        // Trường nhập liệu khi chỉnh sửa
        [ObservableProperty]
        private int editMaxAirports;
        [ObservableProperty]
        private int editMinFlightTime;
        [ObservableProperty]
        private int editMaxStopover;
        [ObservableProperty]
        private int editMinStopTime;
        [ObservableProperty]
        private int editMaxStopTime;
        [ObservableProperty]
        private int editBookingTime;
        [ObservableProperty]
        private int editCancelTime;

        public RegulationManagementViewModel()
        {
            // Giả lập dữ liệu, thực tế nên load từ DB
            loadData();
        }
        private void loadData()
        {
            MaxAirports = 10;
            MinFlightTime = 30;
            MaxStopover = 3;
            MinStopTime = 10;
            MaxStopTime = 30;
            BookingTime = 24;
            CancelTime = 12;
        }


        // --- MaxAirports ---
        [RelayCommand]
        private void _EditMaxAirports()
        {
            EditMaxAirports = MaxAirports;
            IsEditingMaxAirports = true;
        }
        [RelayCommand]
        private void SaveMaxAirports()
        {
            MaxAirports = EditMaxAirports;
            IsEditingMaxAirports = false;
            // TODO: Lưu vào DB
        }
        [RelayCommand]
        private void CancelMaxAirports()
        {
            IsEditingMaxAirports = false;
        }

        // --- MinFlightTime ---
        [RelayCommand]
        private void _EditMinFlightTime()
        {
            EditMinFlightTime = MinFlightTime;
            IsEditingMinFlightTime = true;
        }
        [RelayCommand]
        private void SaveMinFlightTime()
        {
            MinFlightTime = EditMinFlightTime;
            IsEditingMinFlightTime = false;
            // TODO: Lưu vào DB
        }
        [RelayCommand]
        private void CancelMinFlightTime()
        {
            IsEditingMinFlightTime = false;
        }

        // --- MaxStopover ---
        [RelayCommand]
        private void _EditMaxStopover()
        {
            EditMaxStopover = MaxStopover;
            IsEditingMaxStopover = true;
        }
        [RelayCommand]
        private void SaveMaxStopover()
        {
            MaxStopover = EditMaxStopover;
            IsEditingMaxStopover = false;
            // TODO: Lưu vào DB
        }
        [RelayCommand]
        private void CancelMaxStopover()
        {
            IsEditingMaxStopover = false;
        }

        // --- MinStopTime ---
        [RelayCommand]
        private void _EditMinStopTime()
        {
            EditMinStopTime = MinStopTime;
            IsEditingMinStopTime = true;
        }
        [RelayCommand]
        private void SaveMinStopTime()
        {
            MinStopTime = EditMinStopTime;
            IsEditingMinStopTime = false;
            // TODO: Lưu vào DB
        }
        [RelayCommand]
        private void CancelMinStopTime()
        {
            IsEditingMinStopTime = false;
        }

        // --- MaxStopTime ---
        [RelayCommand]
        private void _EditMaxStopTime()
        {
            EditMaxStopTime = MaxStopTime;
            IsEditingMaxStopTime = true;
        }
        [RelayCommand]
        private void SaveMaxStopTime()
        {
            MaxStopTime = EditMaxStopTime;
            IsEditingMaxStopTime = false;
            // TODO: Lưu vào DB
        }
        [RelayCommand]
        private void CancelMaxStopTime()
        {
            IsEditingMaxStopTime = false;
        }

        // --- BookingTime ---
        [RelayCommand]
        private void _EditBookingTime()
        {
            EditBookingTime = BookingTime;
            IsEditingBookingTime = true;
        }
        [RelayCommand]
        private void SaveBookingTime()
        {
            BookingTime = EditBookingTime;
            IsEditingBookingTime = false;
            // TODO: Lưu vào DB
        }
        [RelayCommand]
        private void CancelBookingTime()
        {
            IsEditingBookingTime = false;
        }

        // --- CancelTime ---
        [RelayCommand]
        private void _EditCancelTime()
        {
            EditCancelTime = CancelTime;
            IsEditingCancelTime = true;
        }
        [RelayCommand]
        private void SaveCancelTime()
        {
            CancelTime = EditCancelTime;
            IsEditingCancelTime = false;
            // TODO: Lưu vào DB
        }
        [RelayCommand]
        private void CancelCancelTime()
        {
            IsEditingCancelTime = false;
        }
    }
}