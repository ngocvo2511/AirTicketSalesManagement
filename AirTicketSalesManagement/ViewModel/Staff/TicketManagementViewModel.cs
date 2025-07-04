using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using AirTicketSalesManagement.Models.UIModels;
using AirTicketSalesManagement.Services;
using AirTicketSalesManagement.ViewModel.Admin;
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
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Staff
{
    public partial class TicketManagementViewModel : BaseViewModel
    {
        private readonly BaseViewModel parent;
        private ObservableCollection<QuanLiDatVe> rootHistoryBooking = new();
        [ObservableProperty]
        private DateTime? ngayDatFilter;
        [ObservableProperty]
        private string noiDiFilter;
        [ObservableProperty]
        private string noiDenFilter;
        [ObservableProperty]
        private string? hangHangKhongFilter;
        [ObservableProperty]
        private string emailFilter;
        [ObservableProperty]
        private string bookingStatusFilter;
        [ObservableProperty]
        private ObservableCollection<string> bookingStatusList = new();
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
        private ObservableCollection<QuanLiDatVe>? historyBooking = new();
        [ObservableProperty]
        private bool isEmpty;
        //private ObservableCollection<string>
        public TicketManagementViewModel() { }
        public TicketManagementViewModel(BaseViewModel parent)
        {
            this.parent = parent;
            _ = LoadData();
        }
        public async Task LoadData()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var quiDinh = await context.Quydinhs.FirstOrDefaultAsync();
                    var query = from datve in context.Datves
                                 join lichbay in context.Lichbays on datve.MaLb equals lichbay.MaLb
                                 join chuyenbay in context.Chuyenbays on lichbay.SoHieuCb equals chuyenbay.SoHieuCb
                                 join sbDi in context.Sanbays on chuyenbay.Sbdi equals sbDi.MaSb
                                 join sbDen in context.Sanbays on chuyenbay.Sbden equals sbDen.MaSb
                                 join khachhang in context.Khachhangs on datve.MaKh equals khachhang.MaKh into khGroup
                                 from kh in khGroup.DefaultIfEmpty()
                                 join taikhoanKH in context.Taikhoans on kh.MaKh equals taikhoanKH.MaKh into tkKhGroup
                                 from tkKh in tkKhGroup.DefaultIfEmpty()
                                 join nhanvien in context.Nhanviens on datve.MaNv equals nhanvien.MaNv into nvGroup
                                 from nv in nvGroup.DefaultIfEmpty()
                                 join taikhoanNV in context.Taikhoans on nv.MaNv equals taikhoanNV.MaNv into tkNvGroup
                                 from tkNv in tkNvGroup.DefaultIfEmpty()
                                 select new QuanLiDatVe
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
                                     HoTenNguoiDat = kh != null ? kh.HoTenKh : (nv != null ? nv.HoTenNv : "Không rõ"),
                                     EmailNguoiDat = tkKh != null ? tkKh.Email : (tkNv != null ? tkNv.Email : ""),
                                     NgayDat = datve.ThoiGianDv,
                                     TrangThai = datve.TtdatVe,
                                     SoLuongKhach = datve.Ctdvs.Count,
                                     QdDatVe = (quiDinh != null) ? quiDinh.TgdatVeChamNhat : null,
                                     QdHuyVe = (quiDinh != null) ? quiDinh.TghuyDatVe : null
                                 };
                    var result = await query.OrderByDescending(x=>x.NgayDat).ToListAsync();
                    rootHistoryBooking = new ObservableCollection<QuanLiDatVe>(result);
                    HistoryBooking = new ObservableCollection<QuanLiDatVe>(result);
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
                        "Giữ chỗ",
                        "Đã hủy"
                    };
                    BookingStatusFilter = "Tất cả";
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi kết nối cơ sở dữ liệu",e);
            }
        }
        [RelayCommand]
        private void ShowDetailHistory(QuanLiDatVe chiTietVe)
        {
            if (parent is StaffViewModel staffViewModel)
            {
                staffViewModel.CurrentViewModel = new TicketManagementDetailViewModel(chiTietVe, parent);
            }
            else if(parent is AdminViewModel adminViewModel)
            {
                adminViewModel.CurrentViewModel = new TicketManagementDetailViewModel(chiTietVe, parent);
            }
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
                if (!string.IsNullOrWhiteSpace(EmailFilter))
                {
                    filter = filter.Where(v => v.EmailNguoiDat != null && v.EmailNguoiDat.Contains(EmailFilter, StringComparison.OrdinalIgnoreCase));
                }
                HistoryBooking = new ObservableCollection<QuanLiDatVe>(filter);
                IsEmpty = HistoryBooking.Count == 0;
            }
        }
        [RelayCommand]
        private async Task CancelTicket(QuanLiDatVe ve)
        {
            if (ve == null) return;
            if (ve.CanCancel == false)
            {
                MessageBox.Show("Không thể hủy vé này do đã quá thời hạn hủy.");
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
                            await LoadData();
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
        private async Task ConfirmPayment(QuanLiDatVe ve)
        {
            if (ve == null) return;
            if (ve.CanConfirm == false)
            {
                MessageBox.Show("Không thể xác nhận thanh toán vé này do đã quá thời hạn đặt vé.");
                return;
            }
            if (ve.TrangThai != "Chưa thanh toán (tiền mặt)")
            {
                MessageBox.Show("Không thể xác nhận thanh toán.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xác nhận thanh toán vé này không?", "Xác nhận thanh toán", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AirTicketDbContext())
                    {
                        var booking = await context.Datves.FirstOrDefaultAsync(b => b.MaDv == ve.MaVe);
                        if (booking != null)
                        {
                            booking.TtdatVe = "Đã thanh toán";
                            await context.SaveChangesAsync();
                            ve.TrangThai = "Đã thanh toán";
                            OnPropertyChanged(nameof(HistoryBooking));
                            MessageBox.Show("Xác nhận thanh toán thành công.");
                            await LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy vé để xác nhận thanh toán.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xác nhận thanh toán: {ex.Message}");
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
            EmailFilter = null;
            BookingStatusFilter = "Tất cả";
            HistoryBooking = new ObservableCollection<QuanLiDatVe>(rootHistoryBooking);
            IsEmpty = HistoryBooking.Count == 0;
        }

        partial void OnSanBayListChanged(ObservableCollection<string> value)
        {
            OnPropertyChanged(nameof(DiemDiList));
            OnPropertyChanged(nameof(DiemDenList));
        }
    }
}

