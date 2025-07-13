using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using AirTicketSalesManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

    public virtual DbSet<Hangve> Hangves { get; set; }

    public virtual DbSet<Hangvetheolichbay> Hangvetheolichbays { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Lichbay> Lichbays { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Quydinh> Quydinhs { get; set; }

    public virtual DbSet<Sanbay> Sanbays { get; set; }

    public virtual DbSet<Sanbaytrunggian> Sanbaytrunggians { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public string GetConnectionString(string name = "DefaultConnection")
    {
        try
        {
            string documentsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AirTicketSettings"
            );

            string docAppsettingsPath = Path.Combine(documentsPath, "appsettings.json");
            string exeFolderAppsettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            string appsettingsPath = null;

            if (File.Exists(docAppsettingsPath))
            {
                appsettingsPath = docAppsettingsPath;
            }
            else if (File.Exists(exeFolderAppsettingsPath))
            {
                appsettingsPath = exeFolderAppsettingsPath;
            }
            else
            {
                MessageBox.Show("Không tìm thấy file appsettings.json ở Documents hoặc cùng thư mục với file exe.");
                return null;
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(appsettingsPath))
                .AddJsonFile(Path.GetFileName(appsettingsPath), optional: false, reloadOnChange: true);

            var config = builder.Build();
            var conn = config.GetConnectionString(name);

            if (string.IsNullOrEmpty(conn))
            {
                MessageBox.Show("Không tìm thấy chuỗi kết nối trong appsettings.json.");
            }

            return conn;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi đọc appsettings.json: {ex.Message}");
            return null;
        }
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = GetConnectionString();
        optionsBuilder.UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chuyenbay>(entity =>
        {
            entity.HasKey(e => e.SoHieuCb).HasName("PK__CHUYENBA__FB4E27FB3018DA55");

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
            entity.HasKey(e => e.MaCtdv).HasName("PK__CTDV__1E4E40E6ACE4C34D");

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
            entity.Property(e => e.MaHvLb).HasColumnName("MaHV_LB");

            entity.HasOne(d => d.MaDvNavigation).WithMany(p => p.Ctdvs)
                .HasForeignKey(d => d.MaDv)
                .HasConstraintName("FK__CTDV__MaDV__52593CB8");

            entity.HasOne(d => d.MaHvLbNavigation).WithMany(p => p.Ctdvs)
                .HasForeignKey(d => d.MaHvLb)
                .HasConstraintName("FK__CTDV__MaHV_LB__534D60F1");
        });

        modelBuilder.Entity<Datve>(entity =>
        {
            entity.HasKey(e => e.MaDv).HasName("PK__DATVE__27258657DF3F09E6");

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
                .HasConstraintName("FK__DATVE__MaKH__4E88ABD4");

            entity.HasOne(d => d.MaLbNavigation).WithMany(p => p.Datves)
                .HasForeignKey(d => d.MaLb)
                .HasConstraintName("FK__DATVE__MaLB__4D94879B");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Datves)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__DATVE__MaNV__4F7CD00D");
        });

        modelBuilder.Entity<Hangve>(entity =>
        {
            entity.HasKey(e => e.MaHv).HasName("PK__HANGVE__2725A6D2E8ED5E97");

            entity.ToTable("HANGVE");

            entity.Property(e => e.MaHv).HasColumnName("MaHV");
            entity.Property(e => e.TenHv)
                .HasMaxLength(30)
                .HasColumnName("TenHV");
        });

        modelBuilder.Entity<Hangvetheolichbay>(entity =>
        {
            entity.HasKey(e => e.MaHvLb).HasName("PK__HANGVETH__1853D482C3F10A5D");

            entity.ToTable("HANGVETHEOLICHBAY");

            entity.Property(e => e.MaHvLb).HasColumnName("MaHV_LB");
            entity.Property(e => e.MaHv).HasColumnName("MaHV");
            entity.Property(e => e.MaLb).HasColumnName("MaLB");
            entity.Property(e => e.SlveConLai).HasColumnName("SLVeConLai");
            entity.Property(e => e.SlveToiDa).HasColumnName("SLVeToiDa");

            entity.HasOne(d => d.MaHvNavigation).WithMany(p => p.Hangvetheolichbays)
                .HasForeignKey(d => d.MaHv)
                .HasConstraintName("FK__HANGVETHEO__MaHV__46E78A0C");

            entity.HasOne(d => d.MaLbNavigation).WithMany(p => p.Hangvetheolichbays)
                .HasForeignKey(d => d.MaLb)
                .HasConstraintName("FK__HANGVETHEO__MaLB__45F365D3");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACHHAN__2725CF1E232C6FE6");

            entity.ToTable("KHACHHANG");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
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
            entity.HasKey(e => e.MaLb).HasName("PK__LICHBAY__2725C761215B55AE");

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

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHANVIEN__2725D70AB18E030C");

            entity.ToTable("NHANVIEN");

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
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
            entity.HasKey(e => e.Id).HasName("PK__QUYDINH__3214EC279D88D6C2");

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
            entity.HasKey(e => e.MaSb).HasName("PK__SANBAY__2725080E4088113D");

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
            entity.HasKey(e => new { e.Stt, e.SoHieuCb }).HasName("PK__SANBAYTR__65AA54EFED46EA5C");

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
            entity.HasKey(e => e.MaTk).HasName("PK__TAIKHOAN__272500706F4EBA33");

            entity.ToTable("TAIKHOAN");

            entity.HasIndex(e => e.Email, "UQ__TAIKHOAN__A9D105344257395A").IsUnique();

            entity.Property(e => e.MaTk).HasColumnName("MaTK");
            entity.Property(e => e.Email)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VaiTro).HasMaxLength(20);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Taikhoans)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK__TAIKHOAN__MaKH__5AEE82B9");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Taikhoans)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__TAIKHOAN__MaNV__59FA5E80");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
