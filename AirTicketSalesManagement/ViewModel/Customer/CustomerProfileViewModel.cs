using AirTicketSalesManagement.Data;
using AirTicketSalesManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.ViewModel.Customer
{
    public partial class CustomerProfileViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string hoTen;
        [ObservableProperty]
        private string soDienThoai;
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string maKhachHang;
        [ObservableProperty]
        private string canCuoc;
        [ObservableProperty]
        private string gioiTinh;
        [ObservableProperty]
        private DateOnly? ngaySinh;

        public CustomerProfileViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new AirTicketDbContext())
                {
                    var khachHang = context.Khachhangs.FirstOrDefault(kh => kh.MaKh == UserSession.Current.CustomerId);
                    if (khachHang != null)
                    {
                        HoTen = khachHang.HoTenKh;
                        SoDienThoai = khachHang.SoDt;
                        Email = khachHang.Email;
                        MaKhachHang = khachHang.MaKh;
                        CanCuoc = khachHang.GiayToTt;
                        GioiTinh = khachHang.GioiTinh;
                        if (khachHang.NgaySinh.HasValue)
                        {
                            NgaySinh = khachHang.NgaySinh.Value;
                        }
                        else
                        {
                            NgaySinh = null; // hoặc gán một giá trị mặc định rõ ràng hơn
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                Console.WriteLine($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }
    }
}
