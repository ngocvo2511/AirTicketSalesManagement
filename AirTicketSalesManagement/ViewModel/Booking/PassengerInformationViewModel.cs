﻿using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using AirTicketSalesManagement.Data;

namespace AirTicketSalesManagement.ViewModel.Booking
{
    public partial class PassengerInformationViewModel : BaseViewModel
    {
        public string FlightCode { get; set; }

        [ObservableProperty]
        private ObservableCollection<PassengerInfoModel> passengerList;

        [ObservableProperty]
        private string contactEmail;

        [ObservableProperty]
        private string contactPhone;

        public HangVe SelectedTicketClass { get; set; }

        [ObservableProperty]
        public string diemDi;

        [ObservableProperty]
        public string diemDen;
        [ObservableProperty]
        public DateTime thoiGian;
        [ObservableProperty]
        public string flightSummary;
        [ObservableProperty]
        public DateTime ngayBatDauNguoiLon;
        [ObservableProperty]
        public DateTime ngayKetThucNguoiLon;
        [ObservableProperty]
        public DateTime ngayBatDauTreEm;
        [ObservableProperty]
        public DateTime ngayKetThucTreEm;
        [ObservableProperty]
        public DateTime ngayBatDauEmBe;
        [ObservableProperty]
        public DateTime ngayKetThucEmBe;

        public ThongTinChuyenBayDuocChon ThongTinChuyenBayDuocChon { get; set; }

        public NotificationViewModel Notification { get; set; } = new NotificationViewModel();

        public PassengerInformationViewModel()
        {
        }

        public PassengerInformationViewModel(ThongTinChuyenBayDuocChon selectedFlightInfo)
        {
            if (selectedFlightInfo == null)
                throw new ArgumentNullException(nameof(selectedFlightInfo));

            ThongTinChuyenBayDuocChon = selectedFlightInfo;
            // Lưu thông tin chuyến bay và hạng vé
            FlightCode = $"{selectedFlightInfo.Flight.MaSBDi} - {selectedFlightInfo.Flight.MaSBDen} ({selectedFlightInfo.Flight.HangHangKhong})";
            SelectedTicketClass = selectedFlightInfo.TicketClass;
            DiemDi = selectedFlightInfo.Flight.DiemDi;
            DiemDen = selectedFlightInfo.Flight.DiemDen;
            thoiGian = selectedFlightInfo.Flight.NgayDi;
            FlightSummary = $"{DiemDi} đến {DiemDen} - {ThoiGian.ToString("dddd, dd 'tháng' MM, yyyy", new CultureInfo("vi-VN"))}";
            // Khởi tạo danh sách hành khách dựa trên số lượng người lớn, trẻ em, em bé
            InitializePassengerList(selectedFlightInfo.Flight.NumberAdults, selectedFlightInfo.Flight.NumberChildren, selectedFlightInfo.Flight.NumberInfants); // Thay bằng dữ liệu thực tế nếu cần

            using (var context = new AirTicketDbContext())
            {
                var quyDinh = context.Quydinhs.FirstOrDefault();
                if (quyDinh != null)
                {
                    var today = DateTime.Today;

                    NgayBatDauEmBe = today.AddYears(-quyDinh.TuoiToiDaSoSinh.Value); // Ví dụ: sơ sinh <= 2 tuổi → từ hôm nay lùi 2 năm
                    NgayKetThucTreEm = NgayBatDauEmBe.AddDays(-1);               // Trẻ em bắt đầu từ sau em bé

                    NgayBatDauTreEm = today.AddYears(-quyDinh.TuoiToiDaTreEm.Value); // Ví dụ: trẻ em <= 12 tuổi → lùi 12 năm
                    NgayKetThucNguoiLon = NgayBatDauTreEm.AddDays(-1);           // Người lớn từ sau trẻ em

                    NgayBatDauNguoiLon = today.AddYears(-100);                 // Giới hạn tối đa 100 tuổi
                    NgayKetThucEmBe = today;                                     // Sơ sinh: từ hiện tại trở về trước
                }
            }
        }

        public PassengerInformationViewModel(ThongTinHanhKhachVaChuyenBay thongTinHanhKhachVaChuyenBay) : this(thongTinHanhKhachVaChuyenBay.FlightInfo)
        {
            AddExistingInformation(thongTinHanhKhachVaChuyenBay);
        }

        private void AddExistingInformation(ThongTinHanhKhachVaChuyenBay thongTinHanhKhachVaChuyenBay)
        {
            if (thongTinHanhKhachVaChuyenBay?.PassengerList == null || PassengerList == null)
                return;

            for (int i = 0; i < thongTinHanhKhachVaChuyenBay.PassengerList.Count && i < PassengerList.Count; i++)
            {
                var source = thongTinHanhKhachVaChuyenBay.PassengerList[i];
                var target = PassengerList[i];

                target.FullName = source.HoTen;
                target.Gender = source.GioiTinh;
                target.DateOfBirth = source.NgaySinh;
                target.IdentityNumber = source.CCCD;

                // Nếu là Infant thì kiểm tra thông tin người đi kèm
                if (target.PassengerType == PassengerType.Infant && !string.IsNullOrEmpty(source.HoTenNguoiGiamHo))
                {
                    var matchingAdult = PassengerList
                        .FirstOrDefault(p => p.PassengerType == PassengerType.Adult && p.FullName == source.HoTenNguoiGiamHo);

                    target.AccompanyingAdult = matchingAdult;
                }
            }

            ContactEmail = thongTinHanhKhachVaChuyenBay.ContactEmail;
            ContactPhone = thongTinHanhKhachVaChuyenBay.ContactPhone;
        }

        private void InitializePassengerList(int adultCount, int childCount, int infantCount)
        {
            PassengerList = new ObservableCollection<PassengerInfoModel>();

            // Add adults
            for (int i = 0; i < adultCount; i++)
            {
                PassengerList.Add(new PassengerInfoModel
                {
                    PassengerType = PassengerType.Adult,
                    Index = i + 1,
                    GenderOptions = new List<string> { "Nam", "Nữ" }
                });
            }

            // Add children
            for (int i = 0; i < childCount; i++)
            {
                PassengerList.Add(new PassengerInfoModel
                {
                    PassengerType = PassengerType.Child,
                    Index = i + 1,
                    GenderOptions = new List<string> { "Nam", "Nữ" }
                });
            }

            // Add infants
            for (int i = 0; i < infantCount; i++)
            {
                var infant = new PassengerInfoModel
                {
                    PassengerType = PassengerType.Infant,
                    Index = i + 1,
                    GenderOptions = new List<string> { "Nam", "Nữ" }
                };

                // Set list of adults for accompanying dropdown
                infant.AdultPassengers = PassengerList.Where(p => p.PassengerType == PassengerType.Adult).ToList();

                PassengerList.Add(infant);
            }
        }

        [RelayCommand]
        private void Back()
        {
            NavigationService.NavigateBack();
        }

        [RelayCommand]
        private async Task Continue()
        {
            // Validate all required fields are filled
            if (string.IsNullOrWhiteSpace(ContactEmail) || string.IsNullOrWhiteSpace(ContactPhone))
            {
                await Notification.ShowNotificationAsync("Vui lòng nhập đầy đủ thông tin", NotificationType.Warning);
                return;
            }

            if (!IsValidEmail(ContactEmail))
            {
                await Notification.ShowNotificationAsync("Email không hợp lệ!", NotificationType.Warning);
                return;
            }

            if (!IsValidPhone(ContactPhone))
            {
                await Notification.ShowNotificationAsync("Số điện thoại không hợp lệ!", NotificationType.Warning);
                return;
            }

            List<HanhKhach> passengerList = new List<HanhKhach>();

            foreach (var passenger in PassengerList)
            {
                if (string.IsNullOrWhiteSpace(passenger.FullName) || passenger.Gender == null ||
                    passenger.DateOfBirth == null)
                {
                    await Notification.ShowNotificationAsync("Vui lòng nhập đầy đủ thông tin", NotificationType.Warning);
                    return;
                }

                if (!IsValidPassengerDateOfBirth(passenger))
                {
                    await Notification.ShowNotificationAsync($"Ngày sinh của hành khách {passenger.FullName} không hợp lệ với độ tuổi loại {passenger.PassengerTypeText}.", NotificationType.Warning);
                    return;
                }

                // Additional validation for adults
                if (passenger.PassengerType == PassengerType.Adult)
                {
                    if (string.IsNullOrWhiteSpace(passenger.IdentityNumber))
                    {
                        await Notification.ShowNotificationAsync("Vui lòng nhập đầy đủ thông tin", NotificationType.Warning);
                        return;
                    }
                    else if (passenger.IdentityNumber.Length != 12 || !passenger.IdentityNumber.All(char.IsDigit))
                    {
                        await Notification.ShowNotificationAsync("Số căn cước không hợp lệ!", NotificationType.Warning);
                        return;
                    }
                }



                // Additional validation for infants
                if (passenger.PassengerType == PassengerType.Infant &&
                    passenger.AccompanyingAdult == null)
                {
                    await Notification.ShowNotificationAsync("Vui lòng nhập đầy đủ thông tin", NotificationType.Warning);
                    return;
                }
                passengerList.Add(new HanhKhach(passenger.FullName, passenger.DateOfBirth.Value, passenger.Gender, passenger.IdentityNumber, passenger.AccompanyingAdult?.FullName));
            }

            ThongTinHanhKhachVaChuyenBay thongTinHanhKhachVaChuyenBay = new ThongTinHanhKhachVaChuyenBay
            {
                FlightInfo = ThongTinChuyenBayDuocChon,
                PassengerList = passengerList,
                ContactEmail = ContactEmail,
                ContactPhone = ContactPhone
            };

            NavigationService.NavigateTo<PaymentConfirmationViewModel>(thongTinHanhKhachVaChuyenBay);
        }

        public bool IsValidPassengerDateOfBirth(PassengerInfoModel passenger)
        {
            if (passenger.DateOfBirth == null)
                return false;

            var dob = passenger.DateOfBirth.Value.Date;

            return passenger.PassengerType switch
            {
                PassengerType.Adult => dob >= NgayBatDauNguoiLon && dob <= NgayKetThucNguoiLon,
                PassengerType.Child => dob >= NgayBatDauTreEm && dob <= NgayKetThucTreEm,
                PassengerType.Infant => dob >= NgayBatDauEmBe && dob <= NgayKetThucEmBe,
                _ => false
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9}$");
        }
    }

    public partial class PassengerInfoModel : ObservableObject
    {
        public int Index { get; set; }
        public PassengerType PassengerType { get; set; }

        public bool IsAdult => PassengerType == PassengerType.Adult;
        public bool IsChild => PassengerType == PassengerType.Child;
        public bool IsInfant => PassengerType == PassengerType.Infant;

        public string PassengerTypeText
        {
            get
            {
                return PassengerType switch
                {
                    PassengerType.Adult => "Người lớn",
                    PassengerType.Child => "Trẻ em",
                    PassengerType.Infant => "Em bé",
                    _ => string.Empty
                };
            }
        }

        public string PassengerTypeDescription
        {
            get
            {
                int tuoiToiDaSoSinh = 2;
                int tuoiToiDaTreEm = 12;
                using (var context = new AirTicketDbContext()) // Hoặc dùng SqlConnection nếu ADO.NET
                {
                    var quyDinh = context.Quydinhs.FirstOrDefault();
                    tuoiToiDaSoSinh = quyDinh.TuoiToiDaSoSinh ?? 2;
                    tuoiToiDaTreEm = quyDinh.TuoiToiDaTreEm ?? 12;
                }
                return PassengerType switch
                {
                    PassengerType.Adult => $"Từ {tuoiToiDaTreEm} tuổi trở lên",
                    PassengerType.Child => $"Từ {tuoiToiDaSoSinh}-{tuoiToiDaTreEm} tuổi",
                    PassengerType.Infant => $"Dưới {tuoiToiDaSoSinh} tuổi",
                    _ => string.Empty
                };
            }
        }

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private string gender;

        public List<string> GenderOptions { get; set; }

        [ObservableProperty]
        private DateTime? dateOfBirth;

        [ObservableProperty]
        private string identityNumber;


        // For infants only
        public List<PassengerInfoModel> AdultPassengers { get; set; }

        [ObservableProperty]
        private PassengerInfoModel accompanyingAdult;
    }

    public enum PassengerType
    {
        Adult,
        Child,
        Infant
    }
}
