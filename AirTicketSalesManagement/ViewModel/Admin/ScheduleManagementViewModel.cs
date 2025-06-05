using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using System.IO;

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

        public ScheduleManagementViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadSanBay();
                LoadFlightSchedule();
            }
        }

        public void LoadFlightSchedule()
        {
            using var context = new AirTicketDbContext();

            var danhSach = context.Lichbays
                .Include(lb => lb.SoHieuCbNavigation)
                    .ThenInclude(cb => cb.SbdiNavigation)
                .Include(lb => lb.SoHieuCbNavigation)
                    .ThenInclude(cb => cb.SbdenNavigation)
                .ToList();

            // Gán giá trị mặc định nếu sân bay null (phòng chống lỗi binding)
            foreach (var lb in danhSach)
            {
                lb.SoHieuCbNavigation ??= new Chuyenbay();
                lb.SoHieuCbNavigation.SbdiNavigation ??= new Sanbay();
                lb.SoHieuCbNavigation.SbdenNavigation ??= new Sanbay();
            }

            FlightSchedule = new ObservableCollection<Lichbay>(danhSach);
        }

        [RelayCommand]
        public void Refresh()
        {
            LoadFlightSchedule();
        }

        [RelayCommand]
        public void ClearSearch()
        {
            DiemDi = string.Empty;
            DiemDen = string.Empty;

            SoHieuCB = string.Empty;
            TinhTrangLichBay = string.Empty;
            NgayDi = null;
            LoadFlightSchedule();
        }

        [RelayCommand]
        public void Search()
        {
            FlightSchedule.Clear();

            using (var context = new AirTicketDbContext())
            {
                // Truy vấn lịch bay, bao gồm liên kết đến chuyến bay và các sân bay
                var query = context.Lichbays
                    .Include(lb => lb.SoHieuCbNavigation)
                        .ThenInclude(cb => cb.SbdiNavigation)
                    .Include(lb => lb.SoHieuCbNavigation)
                        .ThenInclude(cb => cb.SbdenNavigation)
                    .AsQueryable();

                // Lọc theo điểm đi
                if (!string.IsNullOrWhiteSpace(DiemDi))
                {
                    var maSBDi = ExtractMaSB(DiemDi);
                    query = query.Where(lb => lb.SoHieuCbNavigation.SbdiNavigation.MaSb == maSBDi);
                }

                // Lọc theo điểm đến
                if (!string.IsNullOrWhiteSpace(DiemDen))
                {
                    var maSBDen = ExtractMaSB(DiemDen);
                    query = query.Where(lb => lb.SoHieuCbNavigation.SbdenNavigation.MaSb == maSBDen);
                }

                // Lọc theo số hiệu chuyến bay
                if (!string.IsNullOrWhiteSpace(SoHieuCB))
                {
                    query = query.Where(lb => lb.SoHieuCbNavigation.SoHieuCb.Contains(SoHieuCB));
                }

                // Lọc theo trạng thái lịch bay
                if (!string.IsNullOrWhiteSpace(TinhTrangLichBay) && TinhTrangLichBay != "Tất cả")
                {
                    query = query.Where(lb => lb.TtlichBay == TinhTrangLichBay);
                }

                // Lọc theo ngày đi
                if (NgayDi.HasValue)
                {
                    var selectedDate = NgayDi.Value.Date;
                    query = query.Where(lb => lb.GioDi.Value.Date == selectedDate);
                }

                // Lấy danh sách kết quả
                var danhSach = query.ToList();

                // Gán giá trị mặc định tránh lỗi binding null
                foreach (var lb in danhSach)
                {
                    lb.SoHieuCbNavigation ??= new Chuyenbay();
                    lb.SoHieuCbNavigation.SbdiNavigation ??= new Sanbay();
                    lb.SoHieuCbNavigation.SbdenNavigation ??= new Sanbay();
                }

                // Gán vào ObservableCollection
                FlightSchedule = new ObservableCollection<Lichbay>(danhSach);
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
        public void ImportFromExcel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Chọn file Excel lịch bay"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExcelPackage.License.SetNonCommercialOrganization("AirTicketSale");

                    using var package = new ExcelPackage(new FileInfo(openFileDialog.FileName));
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    using var context = new AirTicketDbContext();

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Bỏ hàng tiêu đề
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

                    LoadFlightSchedule(); // Tải lại danh sách
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
