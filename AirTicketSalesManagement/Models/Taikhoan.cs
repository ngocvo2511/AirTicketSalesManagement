using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Taikhoan
{
    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public int? MaNv { get; set; }

    public int? MaKh { get; set; }

    public virtual Khachhang? MaKhNavigation { get; set; }

    public virtual Nhanvien? MaNvNavigation { get; set; }
}
