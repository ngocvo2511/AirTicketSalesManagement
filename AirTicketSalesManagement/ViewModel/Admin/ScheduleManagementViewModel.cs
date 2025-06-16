// ScheduleManagementViewModel.cs - Tối ưu hoá và fix LicenseContext conflict
using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using AirTicketSalesManagement.Models.UIModels;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class ScheduleManagementViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string diemDi;
        [ObservableProperty]
        private string diemDen;
        [ObservableProperty]
        private string soHieuCB;
        [ObservableProperty]
        private string tinhTrangLichBay;
        [ObservableProperty]
        private DateTime? ngayDi;
        [ObservableProperty]
        private ObservableCollection<Lichbay> flightSchedule = new();
        [ObservableProperty]
        private ObservableCollection<string> sanBayList = new();

        [ObservableProperty]
        private bool isAddSchedulePopupOpen = false;
        [ObservableProperty]
        private bool isEditSchedulePopupOpen = false;
        [ObservableProperty]
        private Lichbay selectedLichBay;

        //Add Schedule
        [ObservableProperty]
        private string addSoHieuCB;
        [ObservableProperty]
        private DateTime? addNgayDi;
        [ObservableProperty]
        private DateTime? addNgayDen;
        [ObservableProperty]
        private string addGioDi = "";

        [ObservableProperty]
        private string addGioDen = "";
        [ObservableProperty]
        private string addLoaiMB;
        [ObservableProperty]
        private string addSLVeKT;
        [ObservableProperty]
        private string addGiaVe;
        [ObservableProperty]
        private string addTTLichBay;
        [ObservableProperty]
        private ObservableCollection<string> flightNumberList;

        [ObservableProperty]
        private ObservableCollection<HangVeTheoLichBay> ticketClassForScheduleList;

        [ObservableProperty]
        private ObservableCollection<string> ticketClassList;

        //Edit Schedule
        [ObservableProperty]
        private string editSoHieuCB;
        [ObservableProperty]
        private DateTime? editNgayDi;
        [ObservableProperty]
        private DateTime? editNgayDen;
        [ObservableProperty]
        private string editGioDi = "";

        [ObservableProperty]
        private string editGioDen = "";
        [ObservableProperty]
        private string editLoaiMB;
        [ObservableProperty]
        private string editSLVeKT;
        [ObservableProperty]
        private string editGiaVe;
        [ObservableProperty]
        private string editTTLichBay;



        public ObservableCollection<string> DiemDiList => new(SanBayList.Where(s => s != DiemDen));
        public ObservableCollection<string> DiemDenList => new(SanBayList.Where(s => s != DiemDi));

        partial void OnDiemDiChanged(string value) => OnPropertyChanged(nameof(DiemDenList));
        partial void OnDiemDenChanged(string value) => OnPropertyChanged(nameof(DiemDiList));

        public ScheduleManagementViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadSanBay();
                LoadFlightSchedule();
            }
        }

        public void LoadSanBay()
        {
            using var context = new AirTicketDbContext();
            var danhSach = context.Sanbays
                .AsEnumerable()
                .Select(sb => $"{sb.ThanhPho} ({sb.MaSb}), {sb.QuocGia}")
                .OrderBy(display => display)
                .ToList();
            SanBayList = new ObservableCollection<string>(danhSach);
        }

        public void LoadSoHieuCB()
        {
            using (var context = new AirTicketDbContext())
            {
                var list = context.Chuyenbays
                                  .Select(cb => cb.SoHieuCb)
                                  .ToList();
                // Nếu null, khởi tạo 1 lần duy nhất
                if (FlightNumberList == null)
                    FlightNumberList = new ObservableCollection<string>();

                FlightNumberList.Clear();
                foreach (var item in list)
                    FlightNumberList.Add(item);
            }
        }

        public void LoadHangVe()
        {
            using (var context = new AirTicketDbContext())
            {
                var list = context.Hangves
                                  .Select(cb => cb.TenHv)
                                  .ToList();
                TicketClassList = new ObservableCollection<string>(list);
            }
        }

        public void LoadFlightSchedule()
        {
            using var context = new AirTicketDbContext();
            var danhSach = context.Lichbays
                .Include(lb => lb.SoHieuCbNavigation).ThenInclude(cb => cb.SbdiNavigation)
                .Include(lb => lb.SoHieuCbNavigation).ThenInclude(cb => cb.SbdenNavigation)
                .Include(lb => lb.Hangvetheolichbays)
                    .ThenInclude(hv => hv.MaHvNavigation)
                .ToList();

            foreach (var lb in danhSach)
            {
                lb.SoHieuCbNavigation ??= new Chuyenbay();
                lb.SoHieuCbNavigation.SbdiNavigation ??= new Sanbay();
                lb.SoHieuCbNavigation.SbdenNavigation ??= new Sanbay();
            }
            FlightSchedule = new ObservableCollection<Lichbay>(danhSach);
        }

        [RelayCommand] public void Refresh() => LoadFlightSchedule();

        [RelayCommand]
        public void ClearSearch()
        {
            DiemDi = string.Empty; DiemDen = string.Empty;
            SoHieuCB = string.Empty; TinhTrangLichBay = string.Empty; NgayDi = null;
            LoadFlightSchedule();
        }

        [RelayCommand]
        public void Search()
        {
            FlightSchedule.Clear();
            using var context = new AirTicketDbContext();
            var query = context.Lichbays.Include(lb => lb.SoHieuCbNavigation)
                .ThenInclude(cb => cb.SbdiNavigation)
                .Include(lb => lb.SoHieuCbNavigation).ThenInclude(cb => cb.SbdenNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(DiemDi))
                query = query.Where(lb => lb.SoHieuCbNavigation.SbdiNavigation.MaSb == ExtractMaSB(DiemDi));
            if (!string.IsNullOrWhiteSpace(DiemDen))
                query = query.Where(lb => lb.SoHieuCbNavigation.SbdenNavigation.MaSb == ExtractMaSB(DiemDen));
            if (!string.IsNullOrWhiteSpace(SoHieuCB))
                query = query.Where(lb => lb.SoHieuCbNavigation.SoHieuCb.Contains(SoHieuCB));
            if (!string.IsNullOrWhiteSpace(TinhTrangLichBay) && TinhTrangLichBay != "Tất cả")
                query = query.Where(lb => lb.TtlichBay == TinhTrangLichBay);
            if (NgayDi.HasValue)
                query = query.Where(lb => lb.GioDi.Value.Date == NgayDi.Value.Date);

            var danhSach = query.ToList();
            foreach (var lb in danhSach)
            {
                lb.SoHieuCbNavigation ??= new Chuyenbay();
                lb.SoHieuCbNavigation.SbdiNavigation ??= new Sanbay();
                lb.SoHieuCbNavigation.SbdenNavigation ??= new Sanbay();
            }
            FlightSchedule = new ObservableCollection<Lichbay>(danhSach);
        }

        private string ExtractMaSB(string displayString)
        {
            if (string.IsNullOrWhiteSpace(displayString)) return "";
            int start = displayString.IndexOf('(');
            int end = displayString.IndexOf(')');
            return (start >= 0 && end > start) ? displayString.Substring(start + 1, end - start - 1) : displayString;
        }

        [RelayCommand]
        public void ImportFromExcel()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Chọn file Excel lịch bay"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using var package = new ExcelPackage(new FileInfo(openFileDialog.FileName));
                    var worksheet = package.Workbook.Worksheets[0];
                    using var context = new AirTicketDbContext();

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        string soHieuCB = worksheet.Cells[row, 1].Text;
                        DateTime gioDi = DateTime.Parse(worksheet.Cells[row, 2].Text);
                        DateTime gioDen = DateTime.Parse(worksheet.Cells[row, 3].Text);
                        string loaiMB = worksheet.Cells[row, 4].Text;
                        int slVe = int.Parse(worksheet.Cells[row, 5].Text);
                        decimal giaVe = decimal.Parse(worksheet.Cells[row, 6].Text);
                        string tinhTrang = worksheet.Cells[row, 7].Text;

                        var lichBay = new Lichbay
                        {
                            SoHieuCb = soHieuCB,
                            GioDi = gioDi,
                            GioDen = gioDen,
                            LoaiMb = loaiMB,
                            SlveKt = slVe,
                            GiaVe = giaVe,
                            TtlichBay = tinhTrang
                        };
                        context.Lichbays.Add(lichBay);
                    }
                    context.SaveChanges();
                    MessageBox.Show("Nhập lịch bay từ Excel thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadFlightSchedule();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void AddSchedule()
        {
            ResetAddField();
            LoadSoHieuCB();
            IsAddSchedulePopupOpen = true;
        }

        private void ResetAddField()
        {
            AddSoHieuCB = string.Empty;
            AddNgayDi = null;
            AddNgayDen = null;
            AddGioDi = string.Empty;
            AddGioDen = string.Empty;
            AddLoaiMB = string.Empty;
            AddSLVeKT = string.Empty;
            AddGiaVe = string.Empty;
            AddTTLichBay = string.Empty;
            TicketClassForScheduleList = new ObservableCollection<HangVeTheoLichBay>();
        }

        [RelayCommand]
        public void CancelAddSchedule()
        {
            IsAddSchedulePopupOpen = false;
        }

        [RelayCommand]
        public void CloseAddSchedule()
        {
            IsAddSchedulePopupOpen = false;
        }

        [RelayCommand]
        public void SaveAddSchedule()
        {

        }

        [RelayCommand]
        public void AddTicketClass()
        {
            LoadHangVe();
            try
            {
                // Tạo sân bay trung gian mới với STT tự động tăng
                var hangVeTheoLichBay = new HangVeTheoLichBay()
                {
                    STT = TicketClassForScheduleList.Count + 1, // Tự động tăng STT
                    TenHangVe = string.Empty, // Mã sân bay trung gian sẽ được nhập sau
                    SLVeToiDa = string.Empty,
                    SLVeConLai = string.Empty,
                    HangVeList = new ObservableCollection<string>(TicketClassList),
                    OnTenHangVeChangedCallback = UpdateTicketClassList
                };

                // Thêm vào collection
                TicketClassForScheduleList.Add(hangVeTheoLichBay);
                UpdateTicketClassList();
                // Log hoặc thông báo thành công (tùy chọn)
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show($"Lỗi khi thêm sân bay trung gian: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTicketClassList()
        {
            if (TicketClassForScheduleList == null || TicketClassForScheduleList.Count == 0)
            {
                return;
            }
            // Danh sách cơ bản loại bỏ điểm đi và điểm đến
            var danhSachCoBan = TicketClassList
                .ToList();

            // Lấy danh sách mã sân bay đã được chọn ở các item khác
            foreach (var item in TicketClassForScheduleList)
            {
                var daChon = TicketClassForScheduleList
                    .Where(x => x != item && !string.IsNullOrWhiteSpace(x.TenHangVe))
                    .Select(x => x.TenHangVe)
                    .ToList();

                var danhSachLoc = danhSachCoBan
                    .Where(x => !daChon.Contains(x))
                    .ToList();

                item.HangVeList = new ObservableCollection<string>(danhSachLoc);
            }
        }

        [RelayCommand]
        public void RemoveAddTicketClass(HangVeTheoLichBay ticketClass)
        {
            try
            {
                if (ticketClass == null)
                {
                    MessageBox.Show("Không tìm thấy hạng ghế để xóa!",
                                  "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa hạng ghế?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Lưu STT của sân bay bị xóa
                    int removedSTT = ticketClass.STT;

                    // Xóa khỏi collection
                    TicketClassForScheduleList.Remove(ticketClass);

                    // Cập nhật lại STT cho các sân bay sau sân bay bị xóa
                    UpdateSTTAfterRemoval(removedSTT);


                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                MessageBox.Show($"Lỗi khi xóa sân bay trung gian: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSTTAfterRemoval(int removedSTT)
        {
            try
            {
                // Cập nhật STT cho các sân bay có STT lớn hơn sân bay bị xóa
                foreach (var ticketClass in TicketClassForScheduleList.Where(a => a.STT > removedSTT))
                {
                    ticketClass.STT--;
                }

                // Sắp xếp lại collection theo STT để đảm bảo thứ tự
                var sortedList = TicketClassForScheduleList.OrderBy(a => a.STT).ToList();
                TicketClassForScheduleList.Clear();

                foreach (var ticketClass in sortedList)
                {
                    TicketClassForScheduleList.Add(ticketClass);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật STT: {ex.Message}");
            }
        }

        [RelayCommand]
        public void EditSchedule(Lichbay selectedLichBay)
        {
            LoadSoHieuCB();
            ResetEditField(selectedLichBay);
            IsEditSchedulePopupOpen = true;
        }

        [RelayCommand]
        public void DeleteSchedule()
        {

        }

        private void ResetEditField(Lichbay selectedLichBay)
        {
            EditSoHieuCB = selectedLichBay?.SoHieuCbNavigation?.SoHieuCb ?? string.Empty;
            EditNgayDi = selectedLichBay?.GioDi?.Date;
            EditNgayDen = selectedLichBay?.GioDen?.Date;
            EditGioDi = selectedLichBay?.GioDi?.ToString("HH:mm") ?? string.Empty;
            EditGioDen = selectedLichBay?.GioDen?.ToString("HH:mm") ?? string.Empty;
            EditLoaiMB = selectedLichBay?.LoaiMb ?? string.Empty;
            EditSLVeKT = selectedLichBay?.SlveKt?.ToString() ?? string.Empty;
            EditTTLichBay = selectedLichBay?.TtlichBay?.ToString() ?? string.Empty;
            EditGiaVe = selectedLichBay?.GiaVe?.ToString("#,##0") + " VNĐ" ?? string.Empty;
            //foreach (var hangVe in selectedLichBay?.Hangvetheolichbays ?? Enumerable.Empty<Hangvetheolichbay>())
            //{
            //    TicketClassForScheduleList.Add(new HangVeTheoLichBay
            //    {
            //        STT = TicketClassForScheduleList.Count + 1,
            //        TenHangVe = hangVe.TenHv,
            //        SLVeToiDa = hangVe.SlveToida.ToString(),
            //        SLVeConLai = hangVe.SlveConlai.ToString(),
            //        HangVeList = new ObservableCollection<string>(TicketClassList),
            //        OnTenHangVeChangedCallback = UpdateTicketClassList
            //    });
            //}
            //EditDiemDi = SelectedLichBay?.SbdiNavigation != null
            //? $"{SelectedFlight.SbdiNavigation.ThanhPho} ({SelectedFlight.SbdiNavigation.MaSb}), {SelectedFlight.SbdiNavigation.QuocGia}"
            //: string.Empty;
            //EditDiemDen = SelectedFlight?.SbdenNavigation != null
            //? $"{SelectedFlight.SbdenNavigation.ThanhPho} ({SelectedFlight.SbdenNavigation.MaSb}), {SelectedFlight.SbdenNavigation.QuocGia}"
            //: string.Empty;
            //EditHangHangKhong = SelectedFlight?.HangHangKhong ?? string.Empty;
            //EditSoHieuCB = SelectedFlight?.SoHieuCb ?? string.Empty;
            //EditTTKhaiThac = SelectedFlight?.TtkhaiThac ?? string.Empty;
            //EditThoiGianBay = SelectedFlight?.SoGioBay.ToString() ?? string.Empty;
            //foreach (var sbtg in SelectedFlight?.Sanbaytrunggians ?? Enumerable.Empty<Sanbaytrunggian>())
            //{
            //    DanhSachEditSBTG.Add(new SBTG
            //    {
            //        STT = sbtg.Stt,
            //        MaSBTG = $"{sbtg.MaSbtgNavigation.ThanhPho} ({sbtg.MaSbtgNavigation.MaSb}), {sbtg.MaSbtgNavigation.QuocGia}",
            //        ThoiGianDung = sbtg.ThoiGianDung.Value,
            //        GhiChu = sbtg.GhiChu,
            //        SbtgList = new ObservableCollection<string>(EditSBTGList),
            //        OnMaSBTGChangedCallback = CapNhatSBTGList
            //    });
            //}
        }

        [RelayCommand]
        public void CancelEditSchedule()
        {
            IsEditSchedulePopupOpen = false;
        }

        [RelayCommand]
        public void CloseEditSchedule()
        {
            IsEditSchedulePopupOpen = false;
        }

        [RelayCommand]
        public void SaveEditSchedule()
        {

        }

        [RelayCommand]
        public void EditTicketClass()
        {

        }

        [RelayCommand]
        public void RemoveEditTicketClass()
        {

        }

    }
}