using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Sanbaytrunggian
{
    public int Stt { get; set; }

    public string? MaSbtg { get; set; }

    public string SoHieuCb { get; set; } = null!;

    public int? ThoiGianDung { get; set; }

    public string? GhiChu { get; set; }

    public virtual Sanbay? MaSbtgNavigation { get; set; }

    public virtual Chuyenbay SoHieuCbNavigation { get; set; } = null!;
}
