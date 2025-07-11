using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Interface;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.Services.EmailServices;
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
using System.Windows.Media;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class BookingHistoryViewModel : BaseViewModel
    {
        private readonly IEmailService _emailService;
        private readonly EmailTemplateService _templateService;
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

        [ObservableProperty]
        private NotificationViewModel notification = new();

        public BookingHistoryViewModel() { }
        public BookingHistoryViewModel(int? idCustomer, CustomerViewModel parent, IEmailService emailService, EmailTemplateService templateService)
        {
            this.parent = parent;
            this._emailService = emailService;
            this._templateService = templateService;
            _ = LoadData(UserSession.Current.CustomerId);
            ClearExpiredHolds();
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
                        "Chưa thanh toán (Tiền mặt)",
                        "Chưa thanh toán (Online)",
                        "Đã hủy"
                    };
                    BookingStatusFilter = "Tất cả";
                }
            }
            catch (Exception e)
            {

            }
        }

        public void ClearExpiredHolds()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var quiDinh = context.Quydinhs.FirstOrDefault();
                    int tgHuy = quiDinh?.TghuyDatVe ?? 0;
                    var expiredDatVes = context.Datves
                        .Where(dv => (dv.TtdatVe == "Chưa thanh toán (Online)" || dv.TtdatVe == "Giữ chỗ") &&
                                     (dv.ThoiGianDv < DateTime.Now.AddMinutes(-20) || (dv.MaLbNavigation != null && dv.MaLbNavigation.GioDi != null && DateTime.Now.Date >= dv.MaLbNavigation.GioDi.Value.AddDays(-tgHuy).Date)))
                        .ToList();
                    foreach (var datVe in expiredDatVes)
                    {
                        var chiTiets = context.Ctdvs.Where(ct => ct.MaDv == datVe.MaDv).ToList();

                        var maHvLb = chiTiets.FirstOrDefault()?.MaHvLb;

                        if (maHvLb != null)
                        {
                            var hangVe = context.Hangvetheolichbays
                                .FirstOrDefault(h => h.MaHvLb == maHvLb);

                            if (hangVe != null)
                            {
                                hangVe.SlveConLai += chiTiets.Count;
                            }
                        }

                        context.Ctdvs.RemoveRange(chiTiets);
                        context.Datves.Remove(datVe);
                    }
                    var datVeTienMat = context.Datves.Where(dv => dv.TtdatVe == "Chưa thanh toán (Tiền mặt)"
                        && dv.MaLbNavigation != null && dv.MaLbNavigation.GioDi != null
                        && DateTime.Now.Date >= dv.MaLbNavigation.GioDi.Value.AddDays(-tgHuy).Date)
                        .ToList();
                    foreach (var ve in datVeTienMat)
                    {
                        var chitiet = context.Ctdvs.Where(ct => ct.MaDv == ve.MaDv).ToList();
                        var maHvLb = chitiet.FirstOrDefault()?.MaHvLb;
                        if (maHvLb != null) 
                        {
                            var hangVe = context.Hangvetheolichbays
                                .FirstOrDefault(h => h.MaHvLb == maHvLb);
                            if (hangVe != null)
                            {
                                hangVe.SlveConLai += chitiet.Count;
                            }
                        }
                        ve.TtdatVe = "Đã hủy";
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        [RelayCommand]
        private void ShowDetailHistory(KQLichSuDatVe lichSuDatVe)
        {
            parent.CurrentViewModel = new BookingHistoryDetailViewModel(lichSuDatVe, parent, _emailService, _templateService);
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
            if (ve.CanCancel == false)
            {
                await notification.ShowNotificationAsync(
                    "Vé không thể hủy vì đã quá thời hạn hủy vé.",
                    NotificationType.Error);
                return;
            }
            if (ve.TrangThai == "Đã hủy")
            {
                await notification.ShowNotificationAsync(
                    "Vé đã được hủy trước đó.",
                    NotificationType.Warning);
                return;
            }

            bool confirm = await notification.ShowNotificationAsync(
                "Bạn có chắc chắn muốn hủy vé này không?",
                NotificationType.Information,
                true);

            if (confirm)
            {
                try
                {
                    using (var context = new AirTicketDbContext())
                    {
                        var booking = await context.Datves.Include(b => b.MaLbNavigation).FirstOrDefaultAsync(b => b.MaDv == ve.MaVe);
                        if (booking != null)
                        {
                            bool isPaid = booking.TtdatVe == "Đã thanh toán";
                            string soHieuCb = booking.MaLbNavigation?.SoHieuCb ?? "";
                            DateTime gioDi = booking.MaLbNavigation?.GioDi ?? DateTime.Now;
                            var chiTietVe = await context.Ctdvs.Where(ct => ct.MaDv == ve.MaVe).ToListAsync();
                            var maHvLb = chiTietVe.FirstOrDefault()?.MaHvLb;
                            if (maHvLb != null)
                            {
                                var hangVe = await context.Hangvetheolichbays
                                    .FirstOrDefaultAsync(h => h.MaHvLb == maHvLb);
                                if (hangVe != null)
                                {
                                    hangVe.SlveConLai += chiTietVe.Count;
                                }
                            }
                            booking.TtdatVe = "Đã hủy";
                            await context.SaveChangesAsync();
                            ve.TrangThai = "Đã hủy";
                            OnPropertyChanged(nameof(HistoryBooking));
                            await notification.ShowNotificationAsync(
                                "Hủy vé thành công.",
                                NotificationType.Information);
                            if (isPaid)
                            {
                                var emailBody = _templateService.BuildBookingCancel(soHieuCb, gioDi, DateTime.Now);
                                await _emailService.SendEmailAsync(
                                    booking.Email ?? UserSession.Current.Email,
                                    $"Huỷ vé chuyến bay {soHieuCb}",
                                    emailBody);
                            }
                            await LoadData(UserSession.Current.CustomerId);
                        }
                        else
                        {
                            await notification.ShowNotificationAsync(
                                "Không tìm thấy vé để hủy.",
                                NotificationType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await notification.ShowNotificationAsync(
                        $"Lỗi khi hủy vé: {ex.Message}",
                        NotificationType.Error);
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

        [ObservableProperty]
        private bool isSearchExpanded = true;

        [ObservableProperty]
        private double searchContentHeight = double.NaN;

        [RelayCommand]
        private void ToggleSearch()
        {
            IsSearchExpanded = !IsSearchExpanded;
        }
    }
}