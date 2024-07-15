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
    internal class HoaDonConfigs : IEntityTypeConfiguration<HoaDon>
    {
        public void Configure(EntityTypeBuilder<HoaDon> builder)
        {
            builder.ToTable("HoaDon");
            builder.HasKey(p => new { p.IDHD });
            builder.Property(p => p.NgayMua).HasColumnName("NgayMua").HasColumnType("DateTime");
            builder.Property(p => p.TongTien).HasColumnName("TongTien").HasColumnType("Decimal");
            builder.Property(p => p.HinhThucThanhToan).HasColumnName("HinhThucThanhToan").HasColumnType("int");
            builder.HasOne(p => p.User).WithMany(p => p.HoaDons).HasForeignKey(p => p.UserID);
        }
    }
}
