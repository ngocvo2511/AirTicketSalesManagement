using CommunityToolkit.Mvvm.ComponentModel;

namespace AirTicketSalesManagement.Models.UIModels;

public partial class UserSelectionModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string type = string.Empty;

    public string DisplayName => $"{Name} ({Type})";

    public static UserSelectionModel FromNhanVien(Models.Nhanvien nv)
    {
        return new UserSelectionModel
        {
            Id = nv.MaNv,
            Name = nv.HoTenNv ?? string.Empty,
            Type = "Nhân viên"
        };
    }

    public static UserSelectionModel FromKhachHang(Models.Khachhang kh)
    {
        return new UserSelectionModel
        {
            Id = kh.MaKh,
            Name = kh.HoTenKh ?? string.Empty,
            Type = "Khách hàng"
        };
    }
} 