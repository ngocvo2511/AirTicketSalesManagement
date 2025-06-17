using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Khachhang
{
    public int MaKh { get; set; }

    public string? HoTenKh { get; set; }

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? SoDt { get; set; }

    public string? Cccd { get; set; }

    public virtual ICollection<Datve> Datves { get; set; } = new List<Datve>();

    public virtual ICollection<Taikhoan> Taikhoans { get; set; } = new List<Taikhoan>();
}
