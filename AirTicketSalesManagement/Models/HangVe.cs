using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Hangve
{
    public int MaHv { get; set; }

    public string? TenHv { get; set; }

    public double? HeSoGia { get; set; }

    public virtual ICollection<Hangvetheolichbay> Hangvetheolichbays { get; set; } = new List<Hangvetheolichbay>();
}
