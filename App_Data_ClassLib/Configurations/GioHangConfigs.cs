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
    internal class GioHangConfigs : IEntityTypeConfiguration<GioHang>
    {
        public void Configure(EntityTypeBuilder<GioHang> builder)
        {
            builder.ToTable("GioHang");

            builder.HasKey(gh => gh.CartID);

            builder.Property(gh => gh.NgayThem)
                   .HasColumnName("NgayThem")
                   .HasColumnType("DateTime");

            builder.Property(gh => gh.TongSoLuong)
                   .HasColumnName("TongSoLuong")
                   .HasColumnType("int");

            // Cấu hình mối quan hệ giữa GioHang và User
            builder.HasOne(gh => gh.User)
                   .WithOne(u => u.GioHang)
                   .HasForeignKey<GioHang>(gh => gh.UserID);

            // Các cấu hình khác...
        }
    }
}
