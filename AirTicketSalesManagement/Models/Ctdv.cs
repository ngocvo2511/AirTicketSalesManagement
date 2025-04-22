using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Ctdv
{
    public string MaCtdv { get; set; } = null!;

    public string? MaDv { get; set; }

    public string? HoTenHk { get; set; }

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? GiayToTuyThan { get; set; }

    public string? SoDtlienLac { get; set; }

    public string? HoTenNguoiGiamHo { get; set; }

    public string? CccdnguoiGiamHo { get; set; }

    public string? MaLv { get; set; }

    public decimal? GiaVeTt { get; set; }

    public virtual Datve? MaDvNavigation { get; set; }

    public virtual Loaive? MaLvNavigation { get; set; }
}
