using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Sanbay
{
    public string MaSb { get; set; } = null!;

    public string? TenSb { get; set; }

    public string? ThanhPho { get; set; }

    public string? QuocGia { get; set; }

    public virtual ICollection<Chuyenbay> ChuyenbaySbdenNavigations { get; set; } = new List<Chuyenbay>();

    public virtual ICollection<Chuyenbay> ChuyenbaySbdiNavigations { get; set; } = new List<Chuyenbay>();

    public virtual ICollection<Sanbaytrunggian> Sanbaytrunggians { get; set; } = new List<Sanbaytrunggian>();
}
