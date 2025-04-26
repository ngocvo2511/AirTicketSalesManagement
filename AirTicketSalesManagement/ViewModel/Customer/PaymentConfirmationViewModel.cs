using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class PaymentConfirmationViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string flightCode;

        public HangVe SelectedTicketClass { get; set; }

        public KQTraCuuChuyenBayMoRong Flight { get; set; }
        public PaymentConfirmationViewModel()
        {
        }

        public PaymentConfirmationViewModel(ThongTinChuyenBayDuocChon thongTinChuyenBayDuocChon)
        {
            FlightCode = $"{thongTinChuyenBayDuocChon.Flight.MaSBDi} - {thongTinChuyenBayDuocChon.Flight.MaSBDen} ({thongTinChuyenBayDuocChon.Flight.HangHangKhong})";
            SelectedTicketClass = thongTinChuyenBayDuocChon.TicketClass;
            Flight = thongTinChuyenBayDuocChon.Flight;
        }

        [RelayCommand]
        private void Back()
        {
            NavigationService.NavigateBack();
        }

        [RelayCommand]
        private void ProcessPayment()
        {
            // Logic to confirm payment
        }
    }
}
