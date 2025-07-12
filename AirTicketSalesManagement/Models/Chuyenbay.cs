using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Chuyenbay
{
    public string SoHieuCb { get; set; } = null!;

    public string? Sbdi { get; set; }

    public string? Sbden { get; set; }

    public string? HangHangKhong { get; set; }

    public string? TtkhaiThac { get; set; }

    public virtual ICollection<Lichbay> Lichbays { get; set; } = new List<Lichbay>();

    public virtual ICollection<Sanbaytrunggian> Sanbaytrunggians { get; set; } = new List<Sanbaytrunggian>();

    public virtual Sanbay? SbdenNavigation { get; set; }

    public virtual Sanbay? SbdiNavigation { get; set; }

    public int SoSBTG => Sanbaytrunggians?.Count ?? 0;

}
