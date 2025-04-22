using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Lichbay
{
    public string MaLb { get; set; } = null!;

    public string? SoHieuCb { get; set; }

    public DateTime? GioDi { get; set; }

    public DateTime? GioDen { get; set; }

    public string? LoaiMb { get; set; }

    public int? SlveKt { get; set; }

    public decimal? GiaVe { get; set; }

    public string? TtlichBay { get; set; }

    public virtual ICollection<Datve> Datves { get; set; } = new List<Datve>();

    public virtual ICollection<Loaive> Loaives { get; set; } = new List<Loaive>();

    public virtual Chuyenbay? SoHieuCbNavigation { get; set; }
}
