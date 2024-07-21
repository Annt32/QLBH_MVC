using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace App_Data_ClassLib.Models
{
    public partial class SD18302_NET104Context : DbContext
    {

        public SD18302_NET104Context()
        {
        }

        public SD18302_NET104Context(DbContextOptions<SD18302_NET104Context> options)
            : base(options)
        {
        }
        //DBset trỏ đến mỗi bảng
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDonChiTiet> HoaDonChiTiet { get; set; }
        public DbSet<KhuyenMai> KhuyenMai { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<SanPhamChiTiet> SanPhamCTs { get; set; }
        public DbSet<GioHang> GioHang { get; set; }
        public DbSet<GioHangChiTiet> GioHangChiTiets { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-PMB8531\\SQLEXPRESS;Database=SD18302_NET104;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
