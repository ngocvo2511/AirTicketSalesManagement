using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class BookingHistoryViewModel : BaseViewModel
    {
        private CustomerViewModel parent;
        [ObservableProperty]
        private ObservableCollection<KQLichSuDatVe>? historyBooking;
        [ObservableProperty]
        private bool isEmpty;
        public BookingHistoryViewModel() { }
        public BookingHistoryViewModel(string idCustomer, CustomerViewModel parent)
        {     
            this.parent = parent;
            //LoadData(idCustomer);
            HistoryBooking = new ObservableCollection<KQLichSuDatVe>
        {
            new KQLichSuDatVe
            {
                DiemDi = "HAN",
        DiemDen = "SGN",
        HangHangKhong = "Vietnam Airlines",
        GioDi = new DateTime(2025, 5, 10, 8, 30, 0),
        GioDen = new DateTime(2025, 5, 10, 10, 45, 0),
        LoaiMayBay = "Airbus A321",
        NgayDat = new DateTime(2025, 4, 20),
        TrangThai = "Đã thanh toán",
        SoLuongKhach = 5
            },
            new KQLichSuDatVe
            {
                DiemDi = "DAT",
        DiemDen = "HAN",
        HangHangKhong = "Bamboo Airways",
        GioDi = new DateTime(2025, 5, 12, 14, 0, 0),
        GioDen = new DateTime(2025, 5, 12, 15, 30, 0),
        LoaiMayBay = "Embraer E190",
        NgayDat = new DateTime(2025, 4, 21),
        TrangThai = "Chờ thanh toán",
        SoLuongKhach = 4
            },
            new KQLichSuDatVe
    {
        DiemDi = "CAN",
        DiemDen = "DAL",
        HangHangKhong = "VietJet Air",
        GioDi = new DateTime(2025, 5, 15, 6, 45, 0),
        GioDen = new DateTime(2025, 5, 15, 7, 50, 0),
        LoaiMayBay = "Airbus A320",
        NgayDat = new DateTime(2025, 4, 22),
        TrangThai = "Đã hủy",
        SoLuongKhach = 3
    },
        };

            IsEmpty = HistoryBooking.Count == 0;
        }
        public void LoadData(string idCustomer)
        {
            using(var context = new AirTicketDbContext())
            {
                var result = (from datve in context.Datves
                              where datve.MaKh == idCustomer
                              join lichbay in context.Lichbays on datve.MaLb equals lichbay.MaLb
                              join chuyenbay in context.Chuyenbays on lichbay.SoHieuCb equals chuyenbay.SoHieuCb
                              join ctdv in context.Ctdvs on datve.MaDv equals ctdv.MaDv
                              select new KQLichSuDatVe
                              {
                                  DiemDi = chuyenbay.Sbdi,
                                  DiemDen = chuyenbay.Sbden,
                                  HangHangKhong = chuyenbay.HangHangKhong,
                                  GioDi = lichbay.GioDi,
                                  GioDen = lichbay.GioDen,
                                  LoaiMayBay = chuyenbay.MayBay,
                                  NgayDat = datve.ThoiGianDv,
                                  TrangThai = datve.TtdatVe,
                                  SoLuongKhach = datve.Ctdvs.Count
                              }).ToList();

                HistoryBooking = new ObservableCollection<KQLichSuDatVe>(result);
                IsEmpty = HistoryBooking.Count == 0;
            }
        }
        [RelayCommand]
        private void ShowDetailHistory()
        {
            parent.CurrentViewModel = new BookingHistoryDetailViewModel();
        } 
        
    }
}
