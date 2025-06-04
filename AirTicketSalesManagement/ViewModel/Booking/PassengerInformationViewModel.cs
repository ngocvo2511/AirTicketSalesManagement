using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;

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

        public ThongTinChuyenBayDuocChon ThongTinChuyenBayDuocChon { get; set; }


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
            // Khởi tạo danh sách hành khách dựa trên số lượng người lớn, trẻ em, em bé
            InitializePassengerList(selectedFlightInfo.Flight.NumberAdults, selectedFlightInfo.Flight.NumberChildren, selectedFlightInfo.Flight.NumberInfants); // Thay bằng dữ liệu thực tế nếu cần
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
        private void Continue()
        {
            // Validate all required fields are filled
            if (string.IsNullOrWhiteSpace(ContactEmail) || string.IsNullOrWhiteSpace(ContactPhone))
                return;
            List<HanhKhach> passengerList = new List<HanhKhach>();

            foreach (var passenger in PassengerList)
            {
                if (string.IsNullOrWhiteSpace(passenger.FullName) || passenger.Gender == null ||
                    passenger.DateOfBirth == null)
                    return;

                // Additional validation for adults
                if (passenger.PassengerType == PassengerType.Adult &&
                    string.IsNullOrWhiteSpace(passenger.IdentityNumber))
                    return;

                // Additional validation for infants
                if (passenger.PassengerType == PassengerType.Infant &&
                    passenger.AccompanyingAdult == null)
                    return;
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
                return PassengerType switch
                {
                    PassengerType.Adult => "Từ 12 tuổi trở lên",
                    PassengerType.Child => "Từ 2-12 tuổi",
                    PassengerType.Infant => "Dưới 2 tuổi",
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
