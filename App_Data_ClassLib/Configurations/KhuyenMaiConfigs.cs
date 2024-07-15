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
    internal class KhuyenMaiConfigs : IEntityTypeConfiguration<KhuyenMai>
    {
        public void Configure(EntityTypeBuilder<KhuyenMai> builder)
        {
            builder.ToTable("KhuyenMai");
            builder.HasKey(p => new { p.IDKM });
            builder.Property(p => p.Giam).HasColumnName("Giam").HasColumnType("Decimal");
            builder.Property(p => p.NgayBatDau).HasColumnName("NgayBatDau").HasColumnType("DateTime");
            builder.Property(p => p.NgayKetThuc).HasColumnName("NgayKetThuc").HasColumnType("DateTime");
            builder.Property(p => p.MoTa).HasColumnName("MoTa").HasColumnType("nvarchar(50)");
        }
    }
}
