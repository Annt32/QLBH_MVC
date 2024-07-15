using App_Data_ClassLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Configurations
{
    internal class HoaDonChiTietConfigs : IEntityTypeConfiguration<HoaDonChiTiet>
    {
        public void Configure(EntityTypeBuilder<HoaDonChiTiet> builder)
        {
            builder.ToTable("HoaDonChiTiet");
            builder.HasKey(p => new { p.IDHDCT });
            builder.Property(p => p.GiaSP).HasColumnName("GiaSP").HasColumnType("Decimal");
            builder.Property(p => p.SoLuong).HasColumnName("SoLuong").HasColumnType("int");
            builder.HasOne(p => p.HoaDon).WithMany(p => p.HoaDonChitiets).HasForeignKey(p => p.IDHD);
            builder.HasOne(g => g.SanPhamChiTiet).WithMany().HasForeignKey(g => g.IDSPCT).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.KhuyenMai).WithMany(p => p.HoaDonChitiets).HasForeignKey(p => p.IDKM);
        }
    }
}
