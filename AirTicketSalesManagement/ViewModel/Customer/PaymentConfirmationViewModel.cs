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

        public string AdultSummary { get; set; }
        public string ChildSummary { get; set; }
        public string InfantSummary { get; set; }
        public bool HasChildren { get; set; }
        public bool HasInfants { get; set; }
        public decimal AdultTotalPrice { get; set; }
        public decimal ChildTotalPrice { get; set; }
        public decimal InfantTotalPrice { get; set; }
        public decimal TaxAndFees { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentConfirmationViewModel()
        {
        }

        public PaymentConfirmationViewModel(ThongTinChuyenBayDuocChon thongTinChuyenBayDuocChon)
        {
            FlightCode = $"{thongTinChuyenBayDuocChon.Flight.MaSBDi} - {thongTinChuyenBayDuocChon.Flight.MaSBDen} ({thongTinChuyenBayDuocChon.Flight.HangHangKhong})";
            SelectedTicketClass = thongTinChuyenBayDuocChon.TicketClass;
            Flight = thongTinChuyenBayDuocChon.Flight;
            AdultSummary = $"{Flight.NumberAdults} Người lớn";
            ChildSummary = $"{Flight.NumberChildren} Trẻ em";
            InfantSummary = $"{Flight.NumberInfants} Em bé";
            HasChildren = Flight.NumberChildren > 0;
            HasInfants = Flight.NumberInfants > 0;
            AdultTotalPrice = Flight.NumberAdults * SelectedTicketClass.GiaVe;
            ChildTotalPrice = Flight.NumberChildren * SelectedTicketClass.GiaVe;
            InfantTotalPrice = Flight.NumberInfants * SelectedTicketClass.GiaVe;
            TaxAndFees = 0;
            TotalPrice = (Flight.NumberAdults + Flight.NumberChildren + Flight.NumberInfants) * SelectedTicketClass.GiaVe + TaxAndFees;
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
