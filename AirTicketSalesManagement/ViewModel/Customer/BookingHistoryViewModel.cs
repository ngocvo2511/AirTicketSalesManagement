using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class BookingHistoryViewModel : BaseViewModel
    {       
        private readonly CustomerViewModel parent;
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
        private string bookingStatusFilter;
        [ObservableProperty]
        private ObservableCollection<string> hangHangKhongList = new();
        [ObservableProperty]
        private ObservableCollection<string> sanBayList = new();
        [ObservableProperty]
        private ObservableCollection<string> bookingStatusList;
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
        public BookingHistoryViewModel() { }
        public BookingHistoryViewModel(int? idCustomer, CustomerViewModel parent)
        {
            this.parent = parent;
            LoadData(UserSession.Current.CustomerId);           
        }
        public async Task LoadData(int? idCustomer)
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var HuyVe = await context.Quydinhs.FirstOrDefaultAsync();
                    var query = (from datve in context.Datves
                                 where datve.MaKh == idCustomer
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
                                     SoLuongKhach = datve.Ctdvs.Count,
                                     QdHuyVe = (HuyVe != null) ? HuyVe.TghuyDatVe : null
                                 });
                    var result = await query.OrderByDescending(x => x.NgayDat).ToListAsync();
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
                    BookingStatusList = new ObservableCollection<string>
                    {
                        "Tất cả",
                        "Đã thanh toán",
                        "Chờ thanh toán",
                        "Đã hủy"
                    };
                    BookingStatusFilter = "Tất cả";
                }                
            }
            catch(Exception e)
            {

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
                if (!string.IsNullOrWhiteSpace(BookingStatusFilter))
                {
                    filter = filter.Where(v => v.TrangThai == BookingStatusFilter || BookingStatusFilter == "Tất cả");
                }
                HistoryBooking = new ObservableCollection<KQLichSuDatVe>(filter);
                IsEmpty = HistoryBooking.Count == 0;
            }
        }
        [RelayCommand]
        private async Task CancelTicket(KQLichSuDatVe ve)
        {
            if (ve == null) return;
            if(ve.CanCancel == false)
            {
                MessageBox.Show("Vé không thể hủy vì đã quá thời hạn hủy vé.");
                return;
            }
            if (ve.TrangThai == "Đã hủy")
            {
                MessageBox.Show("Vé đã được hủy trước đó.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn hủy vé này không?", "Xác nhận hủy vé", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AirTicketDbContext())
                    {
                        var booking = await context.Datves.FirstOrDefaultAsync(b => b.MaDv == ve.MaVe);
                        if (booking != null)
                        {
                            booking.TtdatVe = "Đã hủy";
                            await context.SaveChangesAsync();
                            ve.TrangThai = "Đã hủy";
                            OnPropertyChanged(nameof(HistoryBooking));
                            MessageBox.Show("Hủy vé thành công.");
                            await LoadData(UserSession.Current.CustomerId);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy vé để hủy.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi hủy vé: {ex.Message}");
                }
            }
        }


        [RelayCommand]
        private void ClearFilter()
        {
            NgayDatFilter = null;
            NoiDiFilter = null;
            NoiDenFilter = null;
            HangHangKhongFilter = null;
            HistoryBooking = new ObservableCollection<KQLichSuDatVe>(rootHistoryBooking);
            IsEmpty = HistoryBooking.Count == 0;
        }

        partial void OnSanBayListChanged(ObservableCollection<string> value)
        {
            OnPropertyChanged(nameof(DiemDiList));
            OnPropertyChanged(nameof(DiemDenList));
        }
    }
}
