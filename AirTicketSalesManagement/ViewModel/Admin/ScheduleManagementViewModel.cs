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
                FlightNumberList = new ObservableCollection<string>(list);
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
            TicketClassForScheduleList = new ObservableCollection<HangVeTheoLichBay>();
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
                    SLVeToiDa = 0, 
                    SLVeConLai = 0,
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
        public void EditSchedule()
        {
            ResetEditField();
            LoadSoHieuCB();
            IsAddSchedulePopupOpen = true;
        }

        private void ResetEditField()
        {

        }

        [RelayCommand]
        public void CancelEditSchedule()
        {
            IsAddSchedulePopupOpen = false;
        }

        [RelayCommand]
        public void CloseEditSchedule()
        {
            IsAddSchedulePopupOpen = false;
        }

        [RelayCommand]
        public void SaveEditSchedule()
        {

        }

        [RelayCommand]
        public void EditTicketClass()
        {

        }

    }
}