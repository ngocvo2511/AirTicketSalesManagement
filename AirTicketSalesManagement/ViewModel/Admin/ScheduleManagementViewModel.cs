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
        private int editID;
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
            var now = DateTime.Now;

            var danhSach = context.Lichbays
                .Include(lb => lb.SoHieuCbNavigation).ThenInclude(cb => cb.SbdiNavigation)
                .Include(lb => lb.SoHieuCbNavigation).ThenInclude(cb => cb.SbdenNavigation)
                .Include(lb => lb.Hangvetheolichbays)
                    .ThenInclude(hv => hv.MaHvNavigation)
                .ToList();

            bool hasChanged = false;

            foreach (var lb in danhSach)
            {
                // Đảm bảo không null
                lb.SoHieuCbNavigation ??= new Chuyenbay();
                lb.SoHieuCbNavigation.SbdiNavigation ??= new Sanbay();
                lb.SoHieuCbNavigation.SbdenNavigation ??= new Sanbay();

                // Cập nhật tình trạng
                if (lb.GioDi <= now && lb.GioDen > now)
                {
                    if (lb.TtlichBay != "Đã cất cánh")
                    {
                        lb.TtlichBay = "Đã cất cánh";
                        hasChanged = true;
                    }
                }
                else if (lb.GioDen <= now)
                {
                    if (lb.TtlichBay != "Hoàn thành")
                    {
                        lb.TtlichBay = "Hoàn thành";
                        hasChanged = true;
                    }
                }
            }

            // Chỉ SaveChanges nếu có thay đổi
            if (hasChanged)
                context.SaveChanges();

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
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(AddSoHieuCB) || AddNgayDi == null || AddNgayDen == null ||
                    string.IsNullOrWhiteSpace(AddGioDi) || string.IsNullOrWhiteSpace(AddGioDen) ||
                    string.IsNullOrWhiteSpace(AddLoaiMB) || string.IsNullOrWhiteSpace(AddSLVeKT) ||
                    string.IsNullOrWhiteSpace(AddGiaVe) || string.IsNullOrWhiteSpace(AddTTLichBay))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin lịch bay.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Ghép giờ đi và ngày đi thành DateTime
                if (!TimeSpan.TryParse(AddGioDi, out TimeSpan gioDi) || !TimeSpan.TryParse(AddGioDen, out TimeSpan gioDen))
                {
                    MessageBox.Show("Định dạng giờ không hợp lệ. Vui lòng nhập theo định dạng HH:mm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime ngayGioDi = AddNgayDi.Value.Date + gioDi;
                DateTime ngayGioDen = AddNgayDen.Value.Date + gioDen;

                using (var context = new AirTicketDbContext())
                {
                    var newSchedule = new Lichbay
                    {
                        SoHieuCb = AddSoHieuCB,
                        GioDi = ngayGioDi,
                        GioDen = ngayGioDen,
                        LoaiMb = AddLoaiMB,
                        SlveKt = int.Parse(AddSLVeKT),
                        GiaVe = decimal.Parse(AddGiaVe),
                        TtlichBay = AddTTLichBay
                    };

                    context.Lichbays.Add(newSchedule);
                    context.SaveChanges();

                    // Lấy MaLB sau khi lưu
                    int maLichBay = newSchedule.MaLb;

                    // Thêm các loại vé theo lịch bay
                    foreach (var hv in TicketClassForScheduleList)
                    {
                        if (string.IsNullOrWhiteSpace(hv.TenHangVe)) continue;

                        var maHV = context.Hangves
                            .FirstOrDefault(h => h.TenHv == hv.TenHangVe)?.MaHv;

                        if (maHV != null)
                        {
                            var hvLb = new Hangvetheolichbay
                            {
                                MaLb = maLichBay,
                                MaHv = maHV,
                                SlveToiDa = int.Parse(hv.SLVeToiDa),
                                SlveConLai = int.Parse(hv.SLVeConLai)
                            };
                            context.Hangvetheolichbays.Add(hvLb);
                        }
                    }

                    context.SaveChanges();

                    MessageBox.Show("Lịch bay đã được thêm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Đóng popup, reset dữ liệu, refresh lại danh sách
                    IsAddSchedulePopupOpen = false;
                    LoadFlightSchedule();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm lịch bay: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            EditID = selectedLichBay?.MaLb ?? 0;

            using (var context = new AirTicketDbContext())
            {
                var schedule = context.Lichbays
                    .Include(lb => lb.Datves)
                    .Include(lb => lb.Hangvetheolichbays)
                    .FirstOrDefault(lb => lb.MaLb == selectedLichBay.MaLb);

                if (schedule == null)
                {
                    MessageBox.Show("Không tìm thấy lịch bay trong cơ sở dữ liệu.",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (schedule.Datves.Any())
                {
                    MessageBox.Show("Không thể sửa lịch bay đã có người đặt vé.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (schedule.TtlichBay == "Đã cất cánh")
                {
                    MessageBox.Show("Không thể sửa lịch bay đã cất cánh.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (schedule.TtlichBay == "Hoàn thành")
                {
                    MessageBox.Show("Không thể sửa lịch bay đã hoàn thành.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            LoadSoHieuCB();
            ResetEditField(selectedLichBay);
            IsEditSchedulePopupOpen = true;
        }

        [RelayCommand]
        public void DeleteSchedule(Lichbay SelectedLichBay)
        {
            if (SelectedLichBay == null)
            {
                MessageBox.Show("Vui lòng chọn một lịch bay để xóa.",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa lịch bay {SelectedLichBay.SoHieuCb} (Mã: {SelectedLichBay.MaLb})?",
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var schedule = context.Lichbays
                        .Include(lb => lb.Datves)
                        .Include(lb => lb.Hangvetheolichbays)
                        .FirstOrDefault(lb => lb.MaLb == SelectedLichBay.MaLb);

                    if (schedule == null)
                    {
                        MessageBox.Show("Không tìm thấy lịch bay trong cơ sở dữ liệu.",
                                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (schedule.Datves.Any())
                    {
                        MessageBox.Show("Không thể xóa lịch bay đã có người đặt vé.",
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (schedule.TtlichBay == "Đã cất cánh")
                    {
                        MessageBox.Show("Không thể xóa lịch bay đã cất cánh.",
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (schedule.TtlichBay == "Hoàn thành")
                    {
                        MessageBox.Show("Không thể xóa lịch bay đã hoàn thành.",
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Xóa các hạng vé theo lịch bay trước
                    context.Hangvetheolichbays.RemoveRange(schedule.Hangvetheolichbays);

                    // Xóa lịch bay
                    context.Lichbays.Remove(schedule);
                    context.SaveChanges();

                    MessageBox.Show("Đã xóa lịch bay thành công!",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Làm mới danh sách
                    LoadFlightSchedule(); // 👈 thay bằng tên hàm bạn dùng để reload danh sách lịch bay
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi xóa lịch bay: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            LoadHangVe();
            TicketClassForScheduleList = new ObservableCollection<HangVeTheoLichBay>();
            foreach (var hangVe in selectedLichBay?.Hangvetheolichbays ?? Enumerable.Empty<Hangvetheolichbay>())
            {
                TicketClassForScheduleList.Add(new HangVeTheoLichBay
                {
                    STT = TicketClassForScheduleList.Count + 1,
                    TenHangVe = hangVe.MaHvNavigation.TenHv,
                    SLVeToiDa = hangVe.SlveToiDa.ToString(),
                    SLVeConLai = hangVe.SlveConLai.ToString(),
                    HangVeList = new ObservableCollection<string>(TicketClassList),
                    OnTenHangVeChangedCallback = UpdateTicketClassList
                });
            }
            UpdateTicketClassList();         
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
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(EditSoHieuCB) || EditNgayDi == null || EditNgayDen == null ||
                    string.IsNullOrWhiteSpace(EditGioDi) || string.IsNullOrWhiteSpace(EditGioDen) ||
                    string.IsNullOrWhiteSpace(EditLoaiMB) || string.IsNullOrWhiteSpace(EditSLVeKT) ||
                    string.IsNullOrWhiteSpace(EditGiaVe) || string.IsNullOrWhiteSpace(EditTTLichBay))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin lịch bay.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Ghép giờ đi và ngày đi thành DateTime
                if (!TimeSpan.TryParse(EditGioDi, out TimeSpan gioDi) || !TimeSpan.TryParse(EditGioDen, out TimeSpan gioDen))
                {
                    MessageBox.Show("Định dạng giờ không hợp lệ. Vui lòng nhập theo định dạng HH:mm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var context = new AirTicketDbContext())
                {
                    var schedule = context.Lichbays
               .Include(lb => lb.Hangvetheolichbays)
               .FirstOrDefault(lb => lb.MaLb == EditID);

                    if (schedule == null)
                    {
                        MessageBox.Show("Không tìm thấy lịch bay để chỉnh sửa.",
                                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật thông tin lịch bay
                    schedule.GioDi = EditNgayDi.Value.Date + TimeSpan.Parse(EditGioDi);
                    schedule.GioDen = EditNgayDen.Value.Date + TimeSpan.Parse(EditGioDen);
                    schedule.LoaiMb = EditLoaiMB;
                    schedule.SlveKt = int.Parse(EditSLVeKT);
                    schedule.GiaVe = decimal.Parse(EditGiaVe.Replace("VNĐ", "").Replace(",", "").Trim());
                    schedule.TtlichBay = EditTTLichBay;

                    // Cập nhật danh sách hạng vé
                    context.Hangvetheolichbays.RemoveRange(schedule.Hangvetheolichbays);

                    foreach (var hv in TicketClassForScheduleList)
                    {
                        if (!string.IsNullOrWhiteSpace(hv.TenHangVe))
                        {
                            var hangVe = context.Hangves.FirstOrDefault(h => h.TenHv == hv.TenHangVe);
                            if (hangVe != null)
                            {
                                var newHV = new Hangvetheolichbay
                                {
                                    MaLb = schedule.MaLb,
                                    MaHv = hangVe.MaHv,
                                    SlveToiDa = int.Parse(hv.SLVeToiDa),
                                    SlveConLai = int.Parse(hv.SLVeConLai)
                                };
                                context.Hangvetheolichbays.Add(newHV);
                            }
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Lịch bay đã được cập nhật thành công!",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Đóng popup và reload danh sách lịch bay
                    IsEditSchedulePopupOpen = false;
                    LoadFlightSchedule();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cập nhật lịch bay: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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