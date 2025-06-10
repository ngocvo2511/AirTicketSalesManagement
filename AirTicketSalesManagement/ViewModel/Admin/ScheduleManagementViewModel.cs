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

namespace AirTicketSalesManagement.ViewModel.Admin
{
    public partial class ScheduleManagementViewModel : BaseViewModel
    {
        [ObservableProperty] private string diemDi;
        [ObservableProperty] private string diemDen;
        [ObservableProperty] private string soHieuCB;
        [ObservableProperty] private string tinhTrangLichBay;
        [ObservableProperty] private DateTime? ngayDi;
        [ObservableProperty] private ObservableCollection<Lichbay> flightSchedule = new();
        [ObservableProperty] private ObservableCollection<string> sanBayList = new();

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
    }
}