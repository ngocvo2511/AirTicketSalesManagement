using System;
using System.Collections.Generic;
using AirTicketSalesManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AirTicketSalesManagement.Data;

public partial class AirTicketDbContext : DbContext
{
    public AirTicketDbContext()
    {
    }

    public AirTicketDbContext(DbContextOptions<AirTicketDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chuyenbay> Chuyenbays { get; set; }

    public virtual DbSet<Ctdv> Ctdvs { get; set; }

    public virtual DbSet<Datve> Datves { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Lichbay> Lichbays { get; set; }

    public virtual DbSet<Loaive> Loaives { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Quydinh> Quydinhs { get; set; }

    public virtual DbSet<Sanbay> Sanbays { get; set; }

    public virtual DbSet<Sanbaytrunggian> Sanbaytrunggians { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-9JTTD1R2;Database=QLVMB;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chuyenbay>(entity =>
        {
            entity.HasKey(e => e.SoHieuCb).HasName("PK__CHUYENBA__FB4E27FB9B9F7175");

            entity.ToTable("CHUYENBAY");

            entity.Property(e => e.SoHieuCb)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SoHieuCB");
            entity.Property(e => e.HangHangKhong).HasMaxLength(50);
            entity.Property(e => e.Sbden)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SBDen");
            entity.Property(e => e.Sbdi)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SBDi");
            entity.Property(e => e.TtkhaiThac)
                .HasMaxLength(30)
                .HasColumnName("TTKhaiThac");

            entity.HasOne(d => d.SbdenNavigation).WithMany(p => p.ChuyenbaySbdenNavigations)
                .HasForeignKey(d => d.Sbden)
                .HasConstraintName("FK__CHUYENBAY__SBDen__3A81B327");

            entity.HasOne(d => d.SbdiNavigation).WithMany(p => p.ChuyenbaySbdiNavigations)
                .HasForeignKey(d => d.Sbdi)
                .HasConstraintName("FK__CHUYENBAY__SBDi__398D8EEE");
        });

        modelBuilder.Entity<Ctdv>(entity =>
        {
            entity.HasKey(e => e.MaCtdv).HasName("PK__CTDV__1E4E40E6F3C4F32B");

            entity.ToTable("CTDV");

            entity.Property(e => e.MaCtdv).HasColumnName("MaCTDV");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.GiaVeTt)
                .HasColumnType("money")
                .HasColumnName("GiaVeTT");
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTenHk)
                .HasMaxLength(30)
                .HasColumnName("HoTenHK");
            entity.Property(e => e.HoTenNguoiGiamHo).HasMaxLength(30);
            entity.Property(e => e.MaDv).HasColumnName("MaDV");
            entity.Property(e => e.MaLv).HasColumnName("MaLV");

            entity.HasOne(d => d.MaDvNavigation).WithMany(p => p.Ctdvs)
                .HasForeignKey(d => d.MaDv)
                .HasConstraintName("FK__CTDV__MaDV__4D94879B");

            entity.HasOne(d => d.MaLvNavigation).WithMany(p => p.Ctdvs)
                .HasForeignKey(d => d.MaLv)
                .HasConstraintName("FK__CTDV__MaLV__4E88ABD4");
        });

        modelBuilder.Entity<Datve>(entity =>
        {
            entity.HasKey(e => e.MaDv).HasName("PK__DATVE__27258657AC4138D1");

            entity.ToTable("DATVE");

            entity.Property(e => e.MaDv).HasColumnName("MaDV");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaLb).HasColumnName("MaLB");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.Slve).HasColumnName("SLVe");
            entity.Property(e => e.SoDtlienLac)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SoDTLienLac");
            entity.Property(e => e.ThoiGianDv)
                .HasColumnType("datetime")
                .HasColumnName("ThoiGianDV");
            entity.Property(e => e.TongTienTt)
                .HasColumnType("money")
                .HasColumnName("TongTienTT");
            entity.Property(e => e.TtdatVe)
                .HasMaxLength(30)
                .HasColumnName("TTDatVe");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Datves)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK__DATVE__MaKH__4AB81AF0");

            entity.HasOne(d => d.MaLbNavigation).WithMany(p => p.Datves)
                .HasForeignKey(d => d.MaLb)
                .HasConstraintName("FK__DATVE__MaLB__49C3F6B7");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Datves)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__DATVE__MaNV__59063A47");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACHHAN__2725CF1EDEDF8357");

            entity.ToTable("KHACHHANG");

            entity.HasIndex(e => e.Email, "UQ__KHACHHAN__A9D10534AE4F61E1").IsUnique();

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTenKh)
                .HasMaxLength(30)
                .HasColumnName("HoTenKH");
            entity.Property(e => e.SoDt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SoDT");
        });

        modelBuilder.Entity<Lichbay>(entity =>
        {
            entity.HasKey(e => e.MaLb).HasName("PK__LICHBAY__2725C761596210CE");

            entity.ToTable("LICHBAY");

            entity.Property(e => e.MaLb).HasColumnName("MaLB");
            entity.Property(e => e.GiaVe).HasColumnType("money");
            entity.Property(e => e.GioDen).HasColumnType("datetime");
            entity.Property(e => e.GioDi).HasColumnType("datetime");
            entity.Property(e => e.LoaiMb)
                .HasMaxLength(30)
                .HasColumnName("LoaiMB");
            entity.Property(e => e.SlveKt).HasColumnName("SLVeKT");
            entity.Property(e => e.SoHieuCb)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SoHieuCB");
            entity.Property(e => e.TtlichBay)
                .HasMaxLength(30)
                .HasColumnName("TTLichBay");

            entity.HasOne(d => d.SoHieuCbNavigation).WithMany(p => p.Lichbays)
                .HasForeignKey(d => d.SoHieuCb)
                .HasConstraintName("FK__LICHBAY__SoHieuC__412EB0B6");
        });

        modelBuilder.Entity<Loaive>(entity =>
        {
            entity.HasKey(e => e.MaLv).HasName("PK__LOAIVE__2725C74D7CE21880");

            entity.ToTable("LOAIVE");

            entity.Property(e => e.MaLv).HasColumnName("MaLV");
            entity.Property(e => e.HangGhe).HasMaxLength(30);
            entity.Property(e => e.MaLb).HasColumnName("MaLB");
            entity.Property(e => e.SlveConLai).HasColumnName("SLVeConLai");
            entity.Property(e => e.SlveToiDa).HasColumnName("SLVeToiDa");

            entity.HasOne(d => d.MaLbNavigation).WithMany(p => p.Loaives)
                .HasForeignKey(d => d.MaLb)
                .HasConstraintName("FK__LOAIVE__MaLB__440B1D61");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHANVIEN__2725D70A14FE292B");

            entity.ToTable("NHANVIEN");

            entity.HasIndex(e => e.Email, "UQ__NHANVIEN__A9D10534D87CFCC4").IsUnique();

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTenNv)
                .HasMaxLength(50)
                .HasColumnName("HoTenNV");
            entity.Property(e => e.SoDt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SoDT");
        });

        modelBuilder.Entity<Quydinh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QUYDINH__3214EC2751540B9B");

            entity.ToTable("QUYDINH");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SoSanBayTgtoiDa).HasColumnName("SoSanBayTGToiDa");
            entity.Property(e => e.TgdatVeChamNhat).HasColumnName("TGDatVeChamNhat");
            entity.Property(e => e.TgdungMax).HasColumnName("TGDungMax");
            entity.Property(e => e.TgdungMin).HasColumnName("TGDungMin");
            entity.Property(e => e.TghuyDatVe).HasColumnName("TGHuyDatVe");
        });

        modelBuilder.Entity<Sanbay>(entity =>
        {
            entity.HasKey(e => e.MaSb).HasName("PK__SANBAY__2725080E7DD4F959");

            entity.ToTable("SANBAY");

            entity.Property(e => e.MaSb)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSB");
            entity.Property(e => e.QuocGia).HasMaxLength(30);
            entity.Property(e => e.TenSb)
                .HasMaxLength(50)
                .HasColumnName("TenSB");
            entity.Property(e => e.ThanhPho).HasMaxLength(30);
        });

        modelBuilder.Entity<Sanbaytrunggian>(entity =>
        {
            entity.HasKey(e => new { e.Stt, e.SoHieuCb }).HasName("PK__SANBAYTR__65AA54EF31D55EB7");

            entity.ToTable("SANBAYTRUNGGIAN");

            entity.Property(e => e.Stt).HasColumnName("STT");
            entity.Property(e => e.SoHieuCb)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SoHieuCB");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaSbtg)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSBTG");

            entity.HasOne(d => d.MaSbtgNavigation).WithMany(p => p.Sanbaytrunggians)
                .HasForeignKey(d => d.MaSbtg)
                .HasConstraintName("FK__SANBAYTRU__MaSBT__3D5E1FD2");

            entity.HasOne(d => d.SoHieuCbNavigation).WithMany(p => p.Sanbaytrunggians)
                .HasForeignKey(d => d.SoHieuCb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SANBAYTRU__SoHie__3E52440B");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__TAIKHOAN__A9D1053544B0221E");

            entity.ToTable("TAIKHOAN");

            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Taikhoans)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK__TAIKHOAN__MaKH__5812160E");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Taikhoans)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__TAIKHOAN__MaNV__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
