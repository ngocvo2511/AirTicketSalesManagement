using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class BookingHistoryViewModel : BaseViewModel
    {       
        private readonly CustomerViewModel parent;
        [ObservableProperty]
        private DateTime? ngayDatFilter;
        [ObservableProperty]
        private string? noiDiFilter;
        [ObservableProperty]
        private string? noiDenFilter;
        [ObservableProperty]
        private string? hangHangKhongFilter;
        [ObservableProperty]
        private ObservableCollection<string> hangHangKhongList = new();
        [ObservableProperty]
        private ObservableCollection<string> sanBayList = new();

        public ObservableCollection<string> DiemDiList =>
            new(SanBayList.Where(s => s != NoiDenFilter));

        public ObservableCollection<string> DiemDenList =>
            new(SanBayList.Where(s => s != NoiDiFilter));
        [ObservableProperty]
        private ObservableCollection<KQLichSuDatVe>? historyBooking = new();
        [ObservableProperty]
        private bool isEmpty;

        //private ObservableCollection<string>
        public BookingHistoryViewModel() { }
        public BookingHistoryViewModel(string idCustomer, CustomerViewModel parent)
        {
            this.parent = parent;
            //LoadData(idCustomer);
            HistoryBooking = new ObservableCollection<KQLichSuDatVe>
                {
                    new KQLichSuDatVe
                    {
                        MaVe = "MV01",
                        MaDiemDi = "HAN",
                        MaDiemDen = "SGN",
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
                        MaVe = "MV02",
                        MaDiemDi = "DAT",
                MaDiemDen = "HAN",
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
                MaVe = "MV03",
                MaDiemDi = "CAN",
                MaDiemDen = "DAL",
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
        public async Task LoadData(string idCustomer)
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
                                  MaVe = datve.MaDv,
                                  MaDiemDi = chuyenbay.Sbdi,
                                  MaDiemDen = chuyenbay.Sbden,                                
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
                //var airlineName = await context.Datves
                //                .Where(datv => datv.MaKh == idCustomer)
                //                .Select(v => v.MaLbNavigation.SoHieuCbNavigation.HangHangKhong)
                //                .Distinct()
                //                .ToListAsync();
                //HangHangKhongList = new ObservableCollection<string>([.. airlineName]);
                //var danhSach = context.Sanbays
                //            .AsEnumerable()
                //            .Select(sb => $"{sb.ThanhPho} ({sb.MaSb}), {sb.QuocGia}")
                //            .OrderBy(display => display)
                //            .ToList();
                //SanBayList = new ObservableCollection<string>(danhSach);
            }
        }
        [RelayCommand]
        private void ShowDetailHistory(KQLichSuDatVe lichSuDatVe)
        {
            parent.CurrentViewModel = new BookingHistoryDetailViewModel(lichSuDatVe, parent);
        }
        [RelayCommand]
        private void SearchHistory()
        {
            if (HistoryBooking == null) return;
            if (HistoryBooking.Count != 0)
            {
                var filter = HistoryBooking.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(NoiDiFilter))
                {
                    filter = filter.Where(v => v.DiemDi == NoiDiFilter);
                }
                if (!string.IsNullOrWhiteSpace(NoiDenFilter))
                {
                    filter = filter.Where(v => v.DiemDen == NoiDenFilter);
                }
                if (!string.IsNullOrWhiteSpace(HangHangKhongFilter))
                {
                    filter = filter.Where(v => v.HangHangKhong == HangHangKhongFilter);
                }
                if (NgayDatFilter.HasValue)
                {
                    filter = filter.Where(v => v.NgayDat?.Date == NgayDatFilter.Value.Date);
                }
                HistoryBooking = new ObservableCollection<KQLichSuDatVe>(filter);
                IsEmpty = HistoryBooking.Count == 0;
            }
        }
    }
}
