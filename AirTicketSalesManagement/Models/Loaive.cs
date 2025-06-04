using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Loaive
{
    public int MaLv { get; set; }

    public int? MaLb { get; set; }

    public string? HangGhe { get; set; }

    public double? HeSoGia { get; set; }

    public int? SlveToiDa { get; set; }

    public int? SlveConLai { get; set; }

    public virtual ICollection<Ctdv> Ctdvs { get; set; } = new List<Ctdv>();

    public virtual Lichbay? MaLbNavigation { get; set; }
}
