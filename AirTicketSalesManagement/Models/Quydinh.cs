using System;
using System.Collections.Generic;

namespace AirTicketSalesManagement.Models;

public partial class Quydinh
{
    public int Id { get; set; }

    public int? SoSanBay { get; set; }

    public int? ThoiGianBayToiThieu { get; set; }

    public int? SoSanBayTgtoiDa { get; set; }

    public int? TgdungMin { get; set; }

    public int? TgdungMax { get; set; }

    public int? SoHangVe { get; set; }

    public int? TgdatVeChamNhat { get; set; }

    public int? TghuyDatVe { get; set; }

    public int? TuoiToiDaSoSinh { get; set; }

    public int? TuoiToiDaTreEm { get; set; }
}
