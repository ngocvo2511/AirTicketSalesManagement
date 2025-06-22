using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.ViewModel.Customer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public partial class TicketManagementViewModel : BaseViewModel
    {
        private readonly StaffViewModel parent;
        private ObservableCollection<KQLichSuDatVe> rootHistoryBooking;
        [ObservableProperty]
        private DateTime? ngayDatFilter;
        [ObservableProperty]
        private string noiDiFilter;
        [ObservableProperty]
        private string noiDenFilter;
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

        partial void OnNoiDiFilterChanged(string value)
        {
            OnPropertyChanged(nameof(DiemDenList));
        }

        partial void OnNoiDenFilterChanged(string value)
        {
            OnPropertyChanged(nameof(DiemDiList));
        }
        [ObservableProperty]
        private ObservableCollection<KQLichSuDatVe>? historyBooking = new();
        [ObservableProperty]
        private bool isEmpty;
        //private ObservableCollection<string>
        public TicketManagementViewModel() { }
        public TicketManagementViewModel(StaffViewModel parent)
        {
            this.parent = parent;
            //LoadData(UserSession.Current.CustomerId);
            rootHistoryBooking = new ObservableCollection<KQLichSuDatVe>
                {
                    new KQLichSuDatVe
                    {
                        MaVe = 1,
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
                        MaVe = 2,
                        MaDiemDi = "DAT",
                MaDiemDen = "HAN",
                DiemDi = "Đà Nẵng(DAT), Việt Nam",
                DiemDen = "Hà Nội(HAN), Việt Nam",
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
                MaVe = 3,
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
            HistoryBooking = new ObservableCollection<KQLichSuDatVe>(rootHistoryBooking);
            IsEmpty = HistoryBooking.Count == 0;
        }
        public async Task LoadData()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var result = await (from datve in context.Datves                                       
                                        join lichbay in context.Lichbays on datve.MaLb equals lichbay.MaLb
                                        join chuyenbay in context.Chuyenbays on lichbay.SoHieuCb equals chuyenbay.SoHieuCb
                                        join sbDi in context.Sanbays on chuyenbay.Sbdi equals sbDi.MaSb
                                        join sbDen in context.Sanbays on chuyenbay.Sbden equals sbDen.MaSb
                                        select new KQLichSuDatVe
                                        {
                                            MaVe = datve.MaDv,
                                            MaDiemDi = chuyenbay.Sbdi,
                                            MaDiemDen = chuyenbay.Sbden,
                                            DiemDi = sbDi.ThanhPho + " (" + sbDi.MaSb + "), " + sbDi.QuocGia,
                                            DiemDen = sbDen.ThanhPho + " (" + sbDen.MaSb + "), " + sbDen.QuocGia,
                                            HangHangKhong = chuyenbay.HangHangKhong,
                                            GioDi = lichbay.GioDi,
                                            GioDen = lichbay.GioDen,
                                            LoaiMayBay = lichbay.LoaiMb,
                                            NgayDat = datve.ThoiGianDv,
                                            TrangThai = datve.TtdatVe,
                                            SoLuongKhach = datve.Ctdvs.Count
                                        }).ToListAsync();
                    rootHistoryBooking = new ObservableCollection<KQLichSuDatVe>(result);
                    HistoryBooking = new ObservableCollection<KQLichSuDatVe>(result);
                    IsEmpty = HistoryBooking.Count == 0;
                    var airlineName = await context.Chuyenbays
                                    .Select(v => v.HangHangKhong)
                                    .Distinct()
                                    .ToListAsync();
                    HangHangKhongList = new ObservableCollection<string>([.. airlineName]);
                    var danhSach = context.Sanbays
                    .AsEnumerable()
                    .Select(sb => $"{sb.ThanhPho} ({sb.MaSb}), {sb.QuocGia}")
                    .OrderBy(display => display)
                    .ToList();
                    SanBayList = new ObservableCollection<string>(danhSach);
                }
            }
            catch (Exception e)
            {

            }
        }
        //[RelayCommand]
        //private void ShowDetailHistory(KQLichSuDatVe lichSuDatVe)
        //{
        //    parent.CurrentViewModel = new BookingHistoryDetailViewModel(lichSuDatVe, parent);
        //}
        [RelayCommand]
        private void SearchHistory()
        {
            if (rootHistoryBooking == null) return;
            if (rootHistoryBooking.Count != 0)
            {
                var filter = rootHistoryBooking.AsEnumerable();
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
        [RelayCommand]
        private void DeleteDate()
        {
            NgayDatFilter = null;
        }

        partial void OnSanBayListChanged(ObservableCollection<string> value)
        {
            OnPropertyChanged(nameof(DiemDiList));
            OnPropertyChanged(nameof(DiemDenList));
        }
    }
}

