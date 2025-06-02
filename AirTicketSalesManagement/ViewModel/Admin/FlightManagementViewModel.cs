using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

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
    }
}
