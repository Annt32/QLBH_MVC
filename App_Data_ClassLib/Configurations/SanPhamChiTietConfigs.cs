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
    internal class SanPhamChiTietConfigs : IEntityTypeConfiguration<SanPhamChiTiet>
    {
        public void Configure(EntityTypeBuilder<SanPhamChiTiet> builder)
        {
            builder.ToTable("SanPhamChiTiet");
            builder.HasKey(p => new { p.IDSPCT});
            builder.Property(p => p.MauSac).HasColumnName("MauSac").HasColumnType("nvarchar(50)");
            builder.Property(p => p.KichThuoc).HasColumnName("KichThuoc").HasColumnType("nvarchar(50)");
            builder.Property(p => p.MoTa).HasColumnName("MoTa").HasColumnType("nvarchar(50)");

            builder.HasOne(p => p.SanPham).WithMany(p => p.SanPhamChiTiets).HasForeignKey(p => p.IDSP);
            builder.HasOne(p => p.Hang).WithMany(p => p.SanPhamChiTiets).HasForeignKey(p => p.IDHang);
        }
    }
}
