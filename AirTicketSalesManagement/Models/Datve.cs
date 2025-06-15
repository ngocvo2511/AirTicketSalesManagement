using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Datve
{
    public int MaDv { get; set; }

    public int? MaLb { get; set; }

    public int? MaKh { get; set; }

    public int? MaNv { get; set; }

    public DateTime? ThoiGianDv { get; set; }

    public int? Slve { get; set; }

    public string? SoDtlienLac { get; set; }

    public string? Email { get; set; }

    public decimal? TongTienTt { get; set; }

    public string? TtdatVe { get; set; }

    public virtual ICollection<Ctdv> Ctdvs { get; set; } = new List<Ctdv>();

    public virtual Khachhang? MaKhNavigation { get; set; }

    public virtual Lichbay? MaLbNavigation { get; set; }

    public virtual Nhanvien? MaNvNavigation { get; set; }
}
