using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Nhanvien
{
    public string MaNv { get; set; } = null!;

    public string? HoTen { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? Email { get; set; }

    public string? SoDt { get; set; }

    public string? Cccd { get; set; }

    public virtual ICollection<Taikhoan> Taikhoans { get; set; } = new List<Taikhoan>();
}
