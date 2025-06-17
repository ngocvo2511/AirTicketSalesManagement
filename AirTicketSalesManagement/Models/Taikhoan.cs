using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Taikhoan
{
    public int MaTk { get; set; }

    public string? Email { get; set; }

    public string MatKhau { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public int? MaNv { get; set; }

    public int? MaKh { get; set; }

    public virtual Khachhang? MaKhNavigation { get; set; }

    public virtual Nhanvien? MaNvNavigation { get; set; }
}
