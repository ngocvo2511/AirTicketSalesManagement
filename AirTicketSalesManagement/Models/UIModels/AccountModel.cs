using AirTicketSalesManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AirTicketSalesManagement.Models.UIModels;

public partial class AccountModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string matKhau = string.Empty;

    [ObservableProperty]
    private string vaiTro = string.Empty;

    [ObservableProperty]
    private int? maNv;

    [ObservableProperty]
    private int? maKh;

    [ObservableProperty]
    private string hoTen = string.Empty;

    [ObservableProperty]
    private string displayRole = string.Empty;

    public static AccountModel FromTaiKhoan(Taikhoan tk)
    {
        var model = new AccountModel
        {
            Id = tk.MaTk,
            Email = tk.Email,
            MatKhau = tk.MatKhau,
            VaiTro = tk.VaiTro,
            MaNv = tk.MaNv,
            MaKh = tk.MaKh
        };

        if (tk.MaNvNavigation != null)
        {
            model.HoTen = tk.MaNvNavigation.HoTenNv ?? string.Empty;
        }
        else if (tk.MaKhNavigation != null)
        {
            model.HoTen = tk.MaKhNavigation.HoTenKh ?? string.Empty;
        }

        model.DisplayRole = GetDisplayRole(tk.VaiTro);
        return model;
    }

    private static string GetDisplayRole(string role)
    {
        return role switch
        {
            "Admin" => "Quản trị viên",
            "NhanVien" => "Nhân viên",
            "KhachHang" => "Khách hàng",
            _ => role
        };
    }
} 