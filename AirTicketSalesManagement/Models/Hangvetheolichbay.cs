using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Hangvetheolichbay
{
    public int MaHvLb { get; set; }

    public int? MaLb { get; set; }

    public int? MaHv { get; set; }

    public int? SlveToiDa { get; set; }

    public int? SlveConLai { get; set; }

    public virtual ICollection<Ctdv> Ctdvs { get; set; } = new List<Ctdv>();

    public virtual Hangve? MaHvNavigation { get; set; }

    public virtual Lichbay? MaLbNavigation { get; set; }
}
