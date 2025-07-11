using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirTicketSalesManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HANGVE",
                columns: table => new
                {
                    MaHV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHV = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    HeSoGia = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HANGVE__2725A6D21B1F6F99", x => x.MaHV);
                });

            migrationBuilder.CreateTable(
                name: "KHACHHANG",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTenKH = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    SoDT = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    CCCD = table.Column<string>(type: "char(12)", unicode: false, fixedLength: true, maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KHACHHAN__2725CF1E96A6589D", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    MaNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTenNV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    SoDT = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    CCCD = table.Column<string>(type: "char(12)", unicode: false, fixedLength: true, maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NHANVIEN__2725D70A8A5D21A0", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "QUYDINH",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoSanBay = table.Column<int>(type: "int", nullable: true),
                    ThoiGianBayToiThieu = table.Column<int>(type: "int", nullable: true),
                    SoSanBayTGToiDa = table.Column<int>(type: "int", nullable: true),
                    TGDungMin = table.Column<int>(type: "int", nullable: true),
                    TGDungMax = table.Column<int>(type: "int", nullable: true),
                    SoHangVe = table.Column<int>(type: "int", nullable: true),
                    TGDatVeChamNhat = table.Column<int>(type: "int", nullable: true),
                    TGHuyDatVe = table.Column<int>(type: "int", nullable: true),
                    TuoiToiDaSoSinh = table.Column<int>(type: "int", nullable: true),
                    TuoiToiDaTreEm = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__QUYDINH__3214EC27DB96B719", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SANBAY",
                columns: table => new
                {
                    MaSB = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    TenSB = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThanhPho = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    QuocGia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SANBAY__2725080E986CBE4F", x => x.MaSB);
                });

            migrationBuilder.CreateTable(
                name: "TAIKHOAN",
                columns: table => new
                {
                    MaTK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(254)", unicode: false, maxLength: 254, nullable: true),
                    MatKhau = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNV = table.Column<int>(type: "int", nullable: true),
                    MaKH = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TAIKHOAN__27250070FBD01FEA", x => x.MaTK);
                    table.ForeignKey(
                        name: "FK__TAIKHOAN__MaKH__5AEE82B9",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK__TAIKHOAN__MaNV__59FA5E80",
                        column: x => x.MaNV,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "CHUYENBAY",
                columns: table => new
                {
                    SoHieuCB = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    SBDi = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                    SBDen = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                    HangHangKhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoGioBay = table.Column<double>(type: "float", nullable: true),
                    TTKhaiThac = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHUYENBA__FB4E27FB61189837", x => x.SoHieuCB);
                    table.ForeignKey(
                        name: "FK__CHUYENBAY__SBDen__3A81B327",
                        column: x => x.SBDen,
                        principalTable: "SANBAY",
                        principalColumn: "MaSB");
                    table.ForeignKey(
                        name: "FK__CHUYENBAY__SBDi__398D8EEE",
                        column: x => x.SBDi,
                        principalTable: "SANBAY",
                        principalColumn: "MaSB");
                });

            migrationBuilder.CreateTable(
                name: "LICHBAY",
                columns: table => new
                {
                    MaLB = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoHieuCB = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    GioDi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GioDen = table.Column<DateTime>(type: "datetime", nullable: true),
                    LoaiMB = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SLVeKT = table.Column<int>(type: "int", nullable: true),
                    GiaVe = table.Column<decimal>(type: "money", nullable: true),
                    TTLichBay = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LICHBAY__2725C761440C3794", x => x.MaLB);
                    table.ForeignKey(
                        name: "FK__LICHBAY__SoHieuC__412EB0B6",
                        column: x => x.SoHieuCB,
                        principalTable: "CHUYENBAY",
                        principalColumn: "SoHieuCB");
                });

            migrationBuilder.CreateTable(
                name: "SANBAYTRUNGGIAN",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    SoHieuCB = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaSBTG = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                    ThoiGianDung = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SANBAYTR__65AA54EFED1205D1", x => new { x.STT, x.SoHieuCB });
                    table.ForeignKey(
                        name: "FK__SANBAYTRU__MaSBT__3D5E1FD2",
                        column: x => x.MaSBTG,
                        principalTable: "SANBAY",
                        principalColumn: "MaSB");
                    table.ForeignKey(
                        name: "FK__SANBAYTRU__SoHie__3E52440B",
                        column: x => x.SoHieuCB,
                        principalTable: "CHUYENBAY",
                        principalColumn: "SoHieuCB");
                });

            migrationBuilder.CreateTable(
                name: "DATVE",
                columns: table => new
                {
                    MaDV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLB = table.Column<int>(type: "int", nullable: true),
                    MaKH = table.Column<int>(type: "int", nullable: true),
                    MaNV = table.Column<int>(type: "int", nullable: true),
                    ThoiGianDV = table.Column<DateTime>(type: "datetime", nullable: true),
                    SLVe = table.Column<int>(type: "int", nullable: true),
                    SoDTLienLac = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "varchar(254)", unicode: false, maxLength: 254, nullable: true),
                    TongTienTT = table.Column<decimal>(type: "money", nullable: true),
                    TTDatVe = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DATVE__27258657DB4E68B5", x => x.MaDV);
                    table.ForeignKey(
                        name: "FK__DATVE__MaKH__4E88ABD4",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK__DATVE__MaLB__4D94879B",
                        column: x => x.MaLB,
                        principalTable: "LICHBAY",
                        principalColumn: "MaLB");
                    table.ForeignKey(
                        name: "FK__DATVE__MaNV__4F7CD00D",
                        column: x => x.MaNV,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNV");
                });

            migrationBuilder.CreateTable(
                name: "HANGVETHEOLICHBAY",
                columns: table => new
                {
                    MaHV_LB = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLB = table.Column<int>(type: "int", nullable: true),
                    MaHV = table.Column<int>(type: "int", nullable: true),
                    SLVeToiDa = table.Column<int>(type: "int", nullable: true),
                    SLVeConLai = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HANGVETH__1853D482890994ED", x => x.MaHV_LB);
                    table.ForeignKey(
                        name: "FK__HANGVETHEO__MaHV__46E78A0C",
                        column: x => x.MaHV,
                        principalTable: "HANGVE",
                        principalColumn: "MaHV");
                    table.ForeignKey(
                        name: "FK__HANGVETHEO__MaLB__45F365D3",
                        column: x => x.MaLB,
                        principalTable: "LICHBAY",
                        principalColumn: "MaLB");
                });

            migrationBuilder.CreateTable(
                name: "CTDV",
                columns: table => new
                {
                    MaCTDV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDV = table.Column<int>(type: "int", nullable: true),
                    HoTenHK = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    CCCD = table.Column<string>(type: "char(12)", unicode: false, fixedLength: true, maxLength: 12, nullable: true),
                    HoTenNguoiGiamHo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MaHV_LB = table.Column<int>(type: "int", nullable: true),
                    GiaVeTT = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CTDV__1E4E40E6AB632E09", x => x.MaCTDV);
                    table.ForeignKey(
                        name: "FK__CTDV__MaDV__52593CB8",
                        column: x => x.MaDV,
                        principalTable: "DATVE",
                        principalColumn: "MaDV");
                    table.ForeignKey(
                        name: "FK__CTDV__MaHV_LB__534D60F1",
                        column: x => x.MaHV_LB,
                        principalTable: "HANGVETHEOLICHBAY",
                        principalColumn: "MaHV_LB");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHUYENBAY_SBDen",
                table: "CHUYENBAY",
                column: "SBDen");

            migrationBuilder.CreateIndex(
                name: "IX_CHUYENBAY_SBDi",
                table: "CHUYENBAY",
                column: "SBDi");

            migrationBuilder.CreateIndex(
                name: "IX_CTDV_MaDV",
                table: "CTDV",
                column: "MaDV");

            migrationBuilder.CreateIndex(
                name: "IX_CTDV_MaHV_LB",
                table: "CTDV",
                column: "MaHV_LB");

            migrationBuilder.CreateIndex(
                name: "IX_DATVE_MaKH",
                table: "DATVE",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_DATVE_MaLB",
                table: "DATVE",
                column: "MaLB");

            migrationBuilder.CreateIndex(
                name: "IX_DATVE_MaNV",
                table: "DATVE",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_HANGVETHEOLICHBAY_MaHV",
                table: "HANGVETHEOLICHBAY",
                column: "MaHV");

            migrationBuilder.CreateIndex(
                name: "IX_HANGVETHEOLICHBAY_MaLB",
                table: "HANGVETHEOLICHBAY",
                column: "MaLB");

            migrationBuilder.CreateIndex(
                name: "IX_LICHBAY_SoHieuCB",
                table: "LICHBAY",
                column: "SoHieuCB");

            migrationBuilder.CreateIndex(
                name: "IX_SANBAYTRUNGGIAN_MaSBTG",
                table: "SANBAYTRUNGGIAN",
                column: "MaSBTG");

            migrationBuilder.CreateIndex(
                name: "IX_SANBAYTRUNGGIAN_SoHieuCB",
                table: "SANBAYTRUNGGIAN",
                column: "SoHieuCB");

            migrationBuilder.CreateIndex(
                name: "IX_TAIKHOAN_MaKH",
                table: "TAIKHOAN",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_TAIKHOAN_MaNV",
                table: "TAIKHOAN",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "UQ__TAIKHOAN__A9D10534CAAAFF1D",
                table: "TAIKHOAN",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CTDV");

            migrationBuilder.DropTable(
                name: "QUYDINH");

            migrationBuilder.DropTable(
                name: "SANBAYTRUNGGIAN");

            migrationBuilder.DropTable(
                name: "TAIKHOAN");

            migrationBuilder.DropTable(
                name: "DATVE");

            migrationBuilder.DropTable(
                name: "HANGVETHEOLICHBAY");

            migrationBuilder.DropTable(
                name: "KHACHHANG");

            migrationBuilder.DropTable(
                name: "NHANVIEN");

            migrationBuilder.DropTable(
                name: "HANGVE");

            migrationBuilder.DropTable(
                name: "LICHBAY");

            migrationBuilder.DropTable(
                name: "CHUYENBAY");

            migrationBuilder.DropTable(
                name: "SANBAY");
        }
    }
}
