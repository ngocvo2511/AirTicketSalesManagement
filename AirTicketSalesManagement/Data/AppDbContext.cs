using AirTicketSalesManagement.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<SanBay> SanBays { get; set; }
        public DbSet<ChuyenBay> ChuyenBays { get; set; }
        public DbSet<SanBayTrungGian> SanBayTrungGians { get; set; }
        public DbSet<LichBay> LichBays { get; set; }
        public DbSet<LoaiVe> LoaiVes { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DatVe> DatVes { get; set; }
        public DbSet<ChiTietDatVe> ChiTietDatVes { get; set; }
        public DbSet<QuyDinh> QuyDinhs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LAPTOP-9JTTD1R2;Database=QLVMB;User Id=sa;Password=123;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Khóa chính kết hợp cho bảng SANBAYTRUNGGIAN
            modelBuilder.Entity<SanBayTrungGian>()
                .HasKey(sbtg => new { sbtg.MaSBTG, sbtg.SoHieuCB });

            base.OnModelCreating(modelBuilder);
        }
    }
}
