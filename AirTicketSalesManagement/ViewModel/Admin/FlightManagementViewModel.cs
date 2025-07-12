using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class FlightManagementViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string diemDi;

        [ObservableProperty]
        private string diemDen;

        [ObservableProperty]
        private string soHieuCB;

        [ObservableProperty]
        private string trangThai;

        [ObservableProperty]
        private string hangHangKhong;

        [ObservableProperty]
        private ObservableCollection<Chuyenbay> flights = new();

        [ObservableProperty]
        private Chuyenbay selectedFlight;

        [ObservableProperty]
        private ObservableCollection<string> sanBayList = new();

        // Danh sách dùng để binding cho điểm đi (lọc bỏ điểm đến)
        public ObservableCollection<string> DiemDiList =>
            new(SanBayList.Where(s => s != DiemDen));

        // Danh sách dùng để binding cho điểm đến (lọc bỏ điểm đi)
        public ObservableCollection<string> DiemDenList =>
            new(SanBayList.Where(s => s != DiemDi));

        partial void OnDiemDiChanged(string value)
        {
            OnPropertyChanged(nameof(DiemDenList));
        }

        partial void OnDiemDenChanged(string value)
        {
            OnPropertyChanged(nameof(DiemDiList));
        }

        // Them chuyen bay
        [ObservableProperty]
        private string addDiemDi;
        [ObservableProperty]
        private string addDiemDen;
        [ObservableProperty]
        private string addSoHieuCB;
        [ObservableProperty]
        private string addHangHangKhong;
        [ObservableProperty]
        private string addTTKhaiThac;
        [ObservableProperty]
        private ObservableCollection<SBTG> danhSachSBTG;

        public ObservableCollection<string> AddDiemDiList =>
            new(SanBayList.Where(s => s != AddDiemDen));

        public ObservableCollection<string> AddDiemDenList =>
            new(SanBayList.Where(s => s != AddDiemDi));

        public ObservableCollection<string> SBTGList =>
            new(SanBayList.Where(s => s != AddDiemDi && s != AddDiemDen));

        partial void OnAddDiemDiChanged(string value)
        {
            OnPropertyChanged(nameof(AddDiemDenList));
            OnPropertyChanged(nameof(SBTGList));
            CapNhatSBTGList();
        }

        partial void OnAddDiemDenChanged(string value)
        {
            OnPropertyChanged(nameof(AddDiemDiList));
            OnPropertyChanged(nameof(SBTGList));
            CapNhatSBTGList();
        }

        [ObservableProperty]
        private bool isAddPopupOpen = false;

        //Sua chuyen bay
        [ObservableProperty]
        private string editDiemDi;
        [ObservableProperty]
        private string editDiemDen;
        [ObservableProperty]
        private string editSoHieuCB;
        [ObservableProperty]
        private string editHangHangKhong;
        [ObservableProperty]
        private string editTTKhaiThac;
        [ObservableProperty]
        private bool isEditPopupOpen = false;

        public ObservableCollection<string> EditDiemDiList =>
            new(SanBayList.Where(s => s != EditDiemDen));

        public ObservableCollection<string> EditDiemDenList =>
            new(SanBayList.Where(s => s != EditDiemDi));

        public ObservableCollection<string> EditSBTGList =>
            new(SanBayList.Where(s => s != EditDiemDen && s != EditDiemDi));

        partial void OnEditDiemDiChanged(string value)
        {
            OnPropertyChanged(nameof(AddDiemDenList));
            OnPropertyChanged(nameof(SBTGList));
            CapNhatSBTGList();
        }

        partial void OnEditDiemDenChanged(string value)
        {
            OnPropertyChanged(nameof(AddDiemDiList));
            OnPropertyChanged(nameof(SBTGList));
            CapNhatSBTGList();
        }

        // Notification
        public NotificationViewModel Notification { get; set; } = new NotificationViewModel();

        public void LoadSanBay()
        {
            using (var context = new AirTicketDbContext()) // Hoặc dùng SqlConnection nếu ADO.NET
            {
                var danhSach = context.Sanbays
                            .AsEnumerable() // chuyển sang LINQ to Objects
                            .Select(sb => $"{sb.ThanhPho} ({sb.MaSb}), {sb.QuocGia}")
                            .OrderBy(display => display)
                            .ToList();
                SanBayList = new ObservableCollection<string>(danhSach);
            }
        }

        public FlightManagementViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadSanBay();
                LoadFlights();
            }
        }

        public void LoadFlights()
        {
            using var context = new AirTicketDbContext();
            var danhSach = context.Chuyenbays
                .Include(cb => cb.SbdiNavigation)
                .Include(cb => cb.SbdenNavigation)
                .Include(cb => cb.Sanbaytrunggians)
                    .ThenInclude(sbtg => sbtg.MaSbtgNavigation)
                .AsEnumerable() // chuyển sang LINQ to Objects
                .Select(cb =>
                {
                    cb.SbdiNavigation ??= new Sanbay();
                    cb.SbdenNavigation ??= new Sanbay();
                    return cb;
                })
                .ToList();

            Flights = new ObservableCollection<Chuyenbay>(danhSach);
        }

        [RelayCommand]
        public void Refresh()
        {
            LoadFlights();
        }

        [RelayCommand]
        public void ClearSearch()
        {
            DiemDi = string.Empty;
            DiemDen = string.Empty;
            SoHieuCB = string.Empty;
            TrangThai = string.Empty;
            HangHangKhong = string.Empty;
            LoadFlights();
        }

        [RelayCommand]
        public void Search()
        {
            Flights.Clear();

            using (var context = new AirTicketDbContext())
            {
                // Truy vấn chuyến bay, bao gồm liên kết sân bay đi, đến
                var query = context.Chuyenbays
                    .Include(cb => cb.SbdiNavigation)
                    .Include(cb => cb.SbdenNavigation)
                    .AsQueryable();

                // Lọc theo điểm đi
                if (!string.IsNullOrWhiteSpace(DiemDi))
                {
                    var maSBDi = ExtractMaSB(DiemDi);
                    query = query.Where(cb => cb.SbdiNavigation.MaSb == maSBDi);
                }

                // Lọc theo điểm đến
                if (!string.IsNullOrWhiteSpace(DiemDen))
                {
                    var maSBDen = ExtractMaSB(DiemDen);
                    query = query.Where(cb => cb.SbdenNavigation.MaSb == maSBDen);
                }

                // Lọc theo số hiệu chuyến bay
                if (!string.IsNullOrWhiteSpace(SoHieuCB))
                {
                    query = query.Where(cb => cb.SoHieuCb.Contains(SoHieuCB));
                }

                // Lọc theo trạng thái
                if (!string.IsNullOrWhiteSpace(TrangThai) && TrangThai != "Tất cả")
                {
                    query = query.Where(cb => cb.TtkhaiThac == TrangThai);
                }

                // Lọc theo hãng hàng không
                if (!string.IsNullOrWhiteSpace(HangHangKhong) && HangHangKhong != "Tất cả")
                {
                    query = query.Where(cb => cb.HangHangKhong == HangHangKhong);
                }

                // Lấy kết quả và đưa vào ObservableCollection
                foreach (var cb in query.ToList())
                {
                    Flights.Add(cb);
                }
            }
        }

        private string ExtractMaSB(string displayString)
        {
            if (string.IsNullOrWhiteSpace(displayString)) return "";
            int start = displayString.IndexOf('(');
            int end = displayString.IndexOf(')');
            if (start >= 0 && end > start)
                return displayString.Substring(start + 1, end - start - 1);
            return displayString;
        }

        [RelayCommand]
        public void AddFlight()
        {
            ResetAddField();
            IsAddPopupOpen = true;
        }

        private void ResetAddField()
        {
            AddDiemDen = string.Empty;
            AddDiemDi = string.Empty;
            AddSoHieuCB = string.Empty;
            AddHangHangKhong = string.Empty;
            AddTTKhaiThac = string.Empty;
            DanhSachSBTG = new ObservableCollection<SBTG>();
        }

        [RelayCommand]
        public void CancelAddFlight()
        {
            IsAddPopupOpen = false;
        }

        [RelayCommand]
        public void CloseAdd()
        {
            IsAddPopupOpen = false;
        }

        [RelayCommand]
        public async void AddIntermediateAirport()
        {
            using (var context = new AirTicketDbContext())
            {
                int soSBTG = 0;
                var quyDinh = context.Quydinhs.FirstOrDefault();

                if (quyDinh != null)
                {
                    soSBTG = quyDinh.SoSanBayTgtoiDa.Value;
                }
                if (DanhSachSBTG.Count >= soSBTG)
                {
                    await Notification.ShowNotificationAsync($"Số sân bay trung gian tối đa là: {soSBTG}", NotificationType.Warning);
                    return;
                }
            }
            try
            {
                // Tạo sân bay trung gian mới với STT tự động tăng
                var sbtg = new SBTG()
                {
                    STT = DanhSachSBTG.Count + 1, // Tự động tăng STT
                    MaSBTG = string.Empty, // Mã sân bay trung gian sẽ được nhập sau
                    ThoiGianDung = 0, // Thời gian dừng mặc định là 0
                    GhiChu = string.Empty, // Ghi chú mặc định là rỗng
                    SbtgList = new ObservableCollection<string>(SBTGList),
                    OnMaSBTGChangedCallback = CapNhatSBTGList
                };

                // Thêm vào collection
                DanhSachSBTG.Add(sbtg);
                CapNhatSBTGList();
                System.Diagnostics.Debug.WriteLine($"Đã thêm sân bay trung gian thứ {sbtg.STT}");
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync($"Lỗi khi thêm sân bay trung gian: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        public async void RemoveIntermediateAirport(SBTG addSBTG)
        {
            try
            {
                if (addSBTG == null)
                {
                    await Notification.ShowNotificationAsync("Không tìm thấy sân bay trung gian để xóa!", NotificationType.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận
                bool confirmed = await Notification.ShowNotificationAsync(
                    $"Bạn có chắc chắn muốn xóa sân bay trung gian thứ {addSBTG.STT}?",
                    NotificationType.Warning,
                    isConfirmation: true);

                if (confirmed)
                {
                    // Lưu STT của sân bay bị xóa
                    int removedSTT = addSBTG.STT;

                    // Xóa khỏi collection
                    DanhSachSBTG.Remove(addSBTG);

                    // Cập nhật lại STT cho các sân bay sau sân bay bị xóa
                    UpdateSTTAfterRemoval(removedSTT);

                    // Log hoặc thông báo thành công
                    System.Diagnostics.Debug.WriteLine($"Đã xóa sân bay trung gian thứ {removedSTT}");
                }
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync($"Lỗi khi xóa sân bay trung gian: {ex.Message}", NotificationType.Error);
            }
        }

        private void UpdateSTTAfterRemoval(int removedSTT)
        {
            try
            {
                // Cập nhật STT cho các sân bay có STT lớn hơn sân bay bị xóa
                foreach (var airport in DanhSachSBTG.Where(a => a.STT > removedSTT))
                {
                    airport.STT--;
                }

                // Sắp xếp lại collection theo STT để đảm bảo thứ tự
                var sortedList = DanhSachSBTG.OrderBy(a => a.STT).ToList();
                DanhSachSBTG.Clear();

                foreach (var airport in sortedList)
                {
                    DanhSachSBTG.Add(airport);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật STT: {ex.Message}");
            }
        }

        private void CapNhatSBTGList()
        {
            if (DanhSachSBTG == null || DanhSachSBTG.Count == 0)
            {
                return;
            }
            // Danh sách cơ bản loại bỏ điểm đi và điểm đến
            var danhSachCoBan = SanBayList
                .Where(s => s != AddDiemDi && s != AddDiemDen)
                .ToList();

            // Lấy danh sách mã sân bay đã được chọn ở các item khác
            foreach (var item in DanhSachSBTG)
            {
                var daChon = DanhSachSBTG
                    .Where(x => x != item && !string.IsNullOrWhiteSpace(x.MaSBTG))
                    .Select(x => x.MaSBTG)
                    .ToList();

                var danhSachLoc = danhSachCoBan
                    .Where(sbtg => !daChon.Contains(sbtg))
                    .ToList();

                item.SbtgList = new ObservableCollection<string>(danhSachLoc);
            }
        }

        [RelayCommand]
        public async void SaveFlight()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AddDiemDi) || string.IsNullOrWhiteSpace(AddDiemDen) ||
                    string.IsNullOrWhiteSpace(AddSoHieuCB) || string.IsNullOrWhiteSpace(AddHangHangKhong) ||
                    string.IsNullOrWhiteSpace(AddTTKhaiThac))
                {
                    await Notification.ShowNotificationAsync("Vui lòng điền đầy đủ thông tin chuyến bay.", NotificationType.Warning);
                    return;
                }
                using (var context = new AirTicketDbContext())
                {
                    // Kiểm tra trùng số hiệu chuyến bay
                    bool isDuplicate = context.Chuyenbays.Any(cb => cb.SoHieuCb == AddSoHieuCB);
                    if (isDuplicate)
                    {
                        await Notification.ShowNotificationAsync("Số hiệu chuyến bay đã tồn tại. Vui lòng nhập số hiệu khác.", NotificationType.Warning);
                        return;
                    }

                    int thoiGianDungMin = 0;
                    int thoiGianDungMax = int.MaxValue;
                    var quyDinh = context.Quydinhs.FirstOrDefault();

                    if (quyDinh != null)
                    {
                        thoiGianDungMin = quyDinh.TgdungMin.Value;
                        thoiGianDungMax = quyDinh.TgdungMax.Value;
                    }

                    foreach (var sbtg in DanhSachSBTG)
                    {
                        if (!string.IsNullOrWhiteSpace(sbtg.MaSBTG))
                        {
                            if (thoiGianDungMin > sbtg.ThoiGianDung)
                            {
                                await Notification.ShowNotificationAsync($"Thời gian dừng tối thiểu là: {thoiGianDungMin} phút", NotificationType.Warning);
                                return;
                            }
                            else if (thoiGianDungMax < sbtg.ThoiGianDung)
                            {
                                await Notification.ShowNotificationAsync($"Thời gian dừng tối đa là: {thoiGianDungMax} phút", NotificationType.Warning);
                                return;
                            }
                        }
                    }

                    // Tạo chuyến bay mới
                    var newFlight = new Chuyenbay
                    {
                        SoHieuCb = AddSoHieuCB,
                        SbdiNavigation = context.Sanbays.FirstOrDefault(sb => sb.MaSb == ExtractMaSB(AddDiemDi)),
                        SbdenNavigation = context.Sanbays.FirstOrDefault(sb => sb.MaSb == ExtractMaSB(AddDiemDen)),
                        HangHangKhong = AddHangHangKhong,
                        TtkhaiThac = AddTTKhaiThac,
                    };

                    context.Chuyenbays.Add(newFlight);
                    context.SaveChanges();
                    // Thêm các sân bay trung gian
                    foreach (var sbtg in DanhSachSBTG)
                    {
                        if (!string.IsNullOrWhiteSpace(sbtg.MaSBTG))
                        {
                            var sb = context.Sanbays.FirstOrDefault(s => s.MaSb == ExtractMaSB(sbtg.MaSBTG));
                            if (sb != null)
                            {
                                var sbtg1 = new Sanbaytrunggian
                                {
                                    Stt = sbtg.STT,
                                    SoHieuCb = newFlight.SoHieuCb,
                                    MaSbtg = sb.MaSb,
                                    ThoiGianDung = sbtg.ThoiGianDung,
                                    GhiChu = sbtg.GhiChu
                                };
                                context.Sanbaytrunggians.Add(sbtg1); // 👈 Bắt buộc để EF thực sự lưu nó
                            }
                        }
                    }
                    // Lưu vào cơ sở dữ liệu
                    context.SaveChanges();
                    await Notification.ShowNotificationAsync("Chuyến bay đã được thêm thành công!", NotificationType.Information);
                    // Đóng popup và làm mới danh sách chuyến bay
                    IsAddPopupOpen = false;
                    LoadFlights();
                }
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Đã xảy ra lỗi khi thêm chuyến bay: " + ex.Message, NotificationType.Error);
            }
        }

        [RelayCommand]
        public void EditFlight()
        {
            DanhSachSBTG = new ObservableCollection<SBTG>();
            ResetEditField();
            IsEditPopupOpen = true;
        }

        private void ResetEditField()
        {
            EditDiemDi = SelectedFlight?.SbdiNavigation != null
            ? $"{SelectedFlight.SbdiNavigation.ThanhPho} ({SelectedFlight.SbdiNavigation.MaSb}), {SelectedFlight.SbdiNavigation.QuocGia}"
            : string.Empty;
            EditDiemDen = SelectedFlight?.SbdenNavigation != null
            ? $"{SelectedFlight.SbdenNavigation.ThanhPho} ({SelectedFlight.SbdenNavigation.MaSb}), {SelectedFlight.SbdenNavigation.QuocGia}"
            : string.Empty;
            EditHangHangKhong = SelectedFlight?.HangHangKhong ?? string.Empty;
            EditSoHieuCB = SelectedFlight?.SoHieuCb ?? string.Empty;
            EditTTKhaiThac = SelectedFlight?.TtkhaiThac ?? string.Empty;
            foreach (var sbtg in SelectedFlight?.Sanbaytrunggians ?? Enumerable.Empty<Sanbaytrunggian>())
            {
                DanhSachSBTG.Add(new SBTG
                {
                    STT = sbtg.Stt,
                    MaSBTG = $"{sbtg.MaSbtgNavigation.ThanhPho} ({sbtg.MaSbtgNavigation.MaSb}), {sbtg.MaSbtgNavigation.QuocGia}",
                    ThoiGianDung = sbtg.ThoiGianDung.Value,
                    GhiChu = sbtg.GhiChu,
                    SbtgList = new ObservableCollection<string>(EditSBTGList),
                    OnMaSBTGChangedCallback = CapNhatSBTGList
                });
            }
            CapNhatSBTGList();
        }

        [RelayCommand]
        public async void DeleteFlight()
        {
            if (SelectedFlight == null)
            {
                await Notification.ShowNotificationAsync("Vui lòng chọn một chuyến bay để xóa.", NotificationType.Warning);
                return;
            }

            bool confirmed = await Notification.ShowNotificationAsync(
                $"Bạn có chắc chắn muốn xóa chuyến bay {SelectedFlight.SoHieuCb}?",
                NotificationType.Warning,
                isConfirmation: true);

            if (!confirmed)
                return;

            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var flight = context.Chuyenbays
                        .Include(cb => cb.Lichbays)
                        .Include(cb => cb.Sanbaytrunggians)
                        .FirstOrDefault(cb => cb.SoHieuCb == SelectedFlight.SoHieuCb);

                    if (flight == null)
                    {
                        await Notification.ShowNotificationAsync("Không tìm thấy chuyến bay trong hệ thống.", NotificationType.Error);
                        return;
                    }

                    if (flight.Lichbays.Any())
                    {
                        await Notification.ShowNotificationAsync("Không thể xóa chuyến bay đã có lịch bay.", NotificationType.Warning);
                        return;
                    }

                    // Xóa các sân bay trung gian liên quan trước (nếu có)
                    context.Sanbaytrunggians.RemoveRange(flight.Sanbaytrunggians);

                    // Sau đó xóa chuyến bay
                    context.Chuyenbays.Remove(flight);
                    context.SaveChanges();

                    await Notification.ShowNotificationAsync("Đã xóa chuyến bay thành công!", NotificationType.Information);

                    // Làm mới danh sách
                    LoadFlights();
                }
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Đã xảy ra lỗi khi xóa chuyến bay: " + ex.Message, NotificationType.Error);
            }
        }

        [RelayCommand]
        public void CancelEditFlight()
        {
            IsEditPopupOpen = false;
        }

        [RelayCommand]
        public void CloseEdit()
        {
            IsEditPopupOpen = false;
        }

        [RelayCommand]
        public async void SaveEditFlight()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditDiemDi) || string.IsNullOrWhiteSpace(EditDiemDen) ||
                    string.IsNullOrWhiteSpace(EditSoHieuCB) || string.IsNullOrWhiteSpace(EditHangHangKhong) ||
                    string.IsNullOrWhiteSpace(EditTTKhaiThac))
                {
                    await Notification.ShowNotificationAsync("Vui lòng điền đầy đủ thông tin chuyến bay.", NotificationType.Warning);
                    return;
                }

                using (var context = new AirTicketDbContext())
                {
                    // Tìm chuyến bay cần chỉnh sửa
                    var existingFlight = context.Chuyenbays
                        .Include(cb => cb.Lichbays)
                        .Include(cb => cb.Sanbaytrunggians)
                        .FirstOrDefault(cb => cb.SoHieuCb == EditSoHieuCB);

                    if (existingFlight == null)
                    {
                        await Notification.ShowNotificationAsync("Không tìm thấy chuyến bay để chỉnh sửa.", NotificationType.Error);
                        return;
                    }

                    if (existingFlight.Lichbays.Any())
                    {
                        await Notification.ShowNotificationAsync("Không thể chỉnh sửa chuyến bay đã có lịch bay.", NotificationType.Warning);
                        return;
                    }

                    // Cập nhật thông tin chuyến bay
                    existingFlight.SbdiNavigation = context.Sanbays.FirstOrDefault(sb => sb.MaSb == ExtractMaSB(EditDiemDi));
                    existingFlight.SbdenNavigation = context.Sanbays.FirstOrDefault(sb => sb.MaSb == ExtractMaSB(EditDiemDen));
                    existingFlight.Sbdi = existingFlight.SbdiNavigation?.MaSb;
                    existingFlight.Sbden = existingFlight.SbdenNavigation?.MaSb;
                    existingFlight.HangHangKhong = EditHangHangKhong;
                    existingFlight.TtkhaiThac = EditTTKhaiThac;

                    // Xóa sân bay trung gian cũ
                    context.Sanbaytrunggians.RemoveRange(existingFlight.Sanbaytrunggians);

                    // Thêm lại sân bay trung gian mới
                    foreach (var sbtg in DanhSachSBTG)
                    {
                        if (!string.IsNullOrWhiteSpace(sbtg.MaSBTG))
                        {
                            var sb = context.Sanbays.FirstOrDefault(s => s.MaSb == ExtractMaSB(sbtg.MaSBTG));
                            if (sb != null)
                            {
                                var sbtgEntity = new Sanbaytrunggian
                                {
                                    Stt = sbtg.STT,
                                    SoHieuCb = existingFlight.SoHieuCb,
                                    MaSbtg = sb.MaSb,
                                    ThoiGianDung = sbtg.ThoiGianDung,
                                    GhiChu = sbtg.GhiChu
                                };
                                context.Sanbaytrunggians.Add(sbtgEntity);
                            }
                        }
                    }

                    context.SaveChanges();

                    await Notification.ShowNotificationAsync("Chuyến bay đã được cập nhật thành công!", NotificationType.Information);

                    // Đóng popup và làm mới danh sách
                    IsEditPopupOpen = false;
                    LoadFlights();
                }
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync("Đã xảy ra lỗi khi cập nhật chuyến bay: " + ex.Message, NotificationType.Error);
            }
        }

        [RelayCommand]
        public async Task EditIntermediateAirportAsync()
        {
            using (var context = new AirTicketDbContext())
            {
                int soSBTG = 0;
                var quyDinh = context.Quydinhs.FirstOrDefault();

                if (quyDinh != null)
                {
                    soSBTG = quyDinh.SoSanBayTgtoiDa.Value;
                }
                if (DanhSachSBTG.Count >= soSBTG)
                {
                    await Notification.ShowNotificationAsync($"Số sân bay trung gian tối đa là: {soSBTG}", NotificationType.Warning);
                    return;
                }
            }
            try
            {
                // Tạo sân bay trung gian mới với STT tự động tăng
                var sbtg = new SBTG()
                {
                    STT = DanhSachSBTG.Count + 1, // Tự động tăng STT
                    MaSBTG = string.Empty, // Mã sân bay trung gian sẽ được nhập sau
                    ThoiGianDung = 0, // Thời gian dừng mặc định là 0
                    GhiChu = string.Empty, // Ghi chú mặc định là rỗng
                    SbtgList = new ObservableCollection<string>(SBTGList),
                    OnMaSBTGChangedCallback = CapNhatSBTGList
                };

                // Thêm vào collection
                DanhSachSBTG.Add(sbtg);
                CapNhatSBTGList();
                System.Diagnostics.Debug.WriteLine($"Đã thêm sân bay trung gian thứ {sbtg.STT}");
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync($"Lỗi khi thêm sân bay trung gian: {ex.Message}", NotificationType.Error);
            }
        }

        [RelayCommand]
        public async void RemoveEditIntermediateAirport(SBTG editSBTG)
        {
            try
            {
                if (editSBTG == null)
                {
                    await Notification.ShowNotificationAsync("Không tìm thấy sân bay trung gian để xóa!", NotificationType.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận
                bool confirmed = await Notification.ShowNotificationAsync(
                    $"Bạn có chắc chắn muốn xóa sân bay trung gian thứ {editSBTG.STT}?",
                    NotificationType.Warning,
                    isConfirmation: true);

                if (confirmed)
                {
                    // Lưu STT của sân bay bị xóa
                    int removedSTT = editSBTG.STT;

                    // Xóa khỏi collection
                    DanhSachSBTG.Remove(editSBTG);

                    // Cập nhật lại STT cho các sân bay sau sân bay bị xóa
                    UpdateSTTAfterRemoval(removedSTT);

                    // Log hoặc thông báo thành công
                    System.Diagnostics.Debug.WriteLine($"Đã xóa sân bay trung gian thứ {removedSTT}");
                }
            }
            catch (Exception ex)
            {
                await Notification.ShowNotificationAsync($"Lỗi khi xóa sân bay trung gian: {ex.Message}", NotificationType.Error);
            }
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