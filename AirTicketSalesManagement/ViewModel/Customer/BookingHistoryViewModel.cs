using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Helper;
using AirTicketSalesManagement.Interface;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.Services.EmailServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class BookingHistoryViewModel : BaseViewModel
    {
        private readonly IEmailService _emailService;
        private readonly EmailTemplateService _templateService;
        private readonly VnpayPayment vnpayPayment;

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
            vnpayPayment = new VnpayPayment();
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
                                     QdHuyVe = (HuyVe != null) ? HuyVe.TghuyDatVe : null,
                                     TongTienTT = datve.TongTienTt.Value
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
        public async Task ProcessPayment(KQLichSuDatVe ve)
        {
            try
            {
                // Debug thông tin user session
                PaymentDebugHelper.LogUserSession();
                PaymentDebugHelper.ValidateUserSessionForPayment();
                UserSession.Current.idVe = ve.MaVe.Value;
                string orderInfo = $"Thanhtoanvemaybay{ve.MaVe}";

                // Tạo URL thanh toán VNPay
                string paymentUrl = vnpayPayment.CreatePaymentUrl((double)ve.TongTienTT, orderInfo, ve.MaVe.Value);

                if (!string.IsNullOrEmpty(paymentUrl))
                {
                    // Lưu thông tin đặt vé tạm thời với trạng thái "Chờ thanh toán"
                    WeakReferenceMessenger.Default.Send(new PaymentRequestedMessage(paymentUrl));
                    Debug.WriteLine($"[ProcessVNPayPayment] Payment URL created successfully: {paymentUrl}");
                }
                else
                {
                    Debug.WriteLine("[ProcessVNPayPayment] Failed to create payment URL");
                    await Notification.ShowNotificationAsync(
                        "Không thể tạo URL thanh toán VNPay",
                        NotificationType.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProcessVNPayPayment] Error: {ex.Message}");
                Debug.WriteLine($"[ProcessVNPayPayment] Stack trace: {ex.StackTrace}");
                await Notification.ShowNotificationAsync(
                    $"Lỗi xử lý thanh toán VNPay: {ex.Message}",
                    NotificationType.Error);
            }
        }

        public void HandlePaymentSuccess()
        {
            try
            {
                PaymentDebugHelper.LogUserSession();
                PaymentDebugHelper.ValidateUserSessionForPayment();

                using (var context = new AirTicketDbContext())
                {
                    Datve datVe;
                    datVe = context.Datves.Include(dv => dv.MaLbNavigation)
                            .Where(dv => dv.MaDv == UserSession.Current.idVe)
                            .OrderByDescending(dv => dv.ThoiGianDv)
                            .FirstOrDefault();
                    
                    if (datVe != null)
                    {
                        if (datVe.TtdatVe == "Chưa thanh toán (Online)")
                        {
                            datVe.TtdatVe = "Đã thanh toán";
                            context.SaveChanges();
                            Debug.WriteLine($"[HandlePaymentSuccess] Successfully updated booking {datVe.MaDv} to 'Đã thanh toán'");
                            string soHieuCb = datVe.MaLbNavigation?.SoHieuCb ?? "";
                            var emailContent = _templateService.BuildBookingSuccess(
                                soHieuCb,
                                datVe.MaLbNavigation?.GioDi ?? DateTime.Now,
                                DateTime.Now,
                                datVe.TongTienTt ?? 0);
                            _emailService.SendEmailAsync(datVe.Email ?? UserSession.Current.Email, $"Thanh toán vé chuyến bay {soHieuCb}", emailContent);
                            _ = LoadData(UserSession.Current.CustomerId);
                        }
                        else
                        {
                            Debug.WriteLine($"[HandlePaymentSuccess] Booking {datVe.MaDv} status is not 'Chưa thanh toán (Online)'. Current status: {datVe.TtdatVe}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[HandlePaymentSuccess] No valid booking found in the last 20 minutes");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HandlePaymentSuccess] Lỗi cập nhật thanh toán: {ex.Message}");
                Debug.WriteLine($"[HandlePaymentSuccess] Stack trace: {ex.StackTrace}");
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