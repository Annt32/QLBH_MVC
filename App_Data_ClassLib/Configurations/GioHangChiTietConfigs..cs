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
    internal class GioHangChiTietConfigs : IEntityTypeConfiguration<GioHangChiTiet>
    {
        public void Configure(EntityTypeBuilder<GioHangChiTiet> builder)
        {
            builder.ToTable("GiohangChiTiet");
            builder.HasKey(p => new { p.CartItemID});
            builder.Property(p => p.SoLuong).HasColumnName("SoLuong").HasColumnType("int");
            builder.Property(p => p.TrangThai).HasColumnName("TrangThai").HasColumnType("int");
            //builder.HasOne(p => p.User).WithMany(p => p.GioHangChitiets).HasForeignKey(p => p.UserID).HasConstraintName("FK_User_User");
            builder.HasOne(p => p.GioHang).WithMany(p => p.GioHangChiTiets).HasForeignKey(p => p.CartID);
            builder.HasOne(g => g.SanPhamChiTiet).WithMany().HasForeignKey(g => g.IDSPCT).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
