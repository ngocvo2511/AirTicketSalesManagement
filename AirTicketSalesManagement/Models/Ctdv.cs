using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Ctdv
{
    public int MaCtdv { get; set; }

    public int? MaDv { get; set; }

    public string? HoTenHk { get; set; }

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? Cccd { get; set; }

    public string? HoTenNguoiGiamHo { get; set; }

    public int? MaHvLb { get; set; }

    public decimal? GiaVeTt { get; set; }

    public virtual Datve? MaDvNavigation { get; set; }

    public virtual Hangvetheolichbay? MaHvLbNavigation { get; set; }
}
