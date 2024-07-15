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
    internal class SanPhamConfigs : IEntityTypeConfiguration<SanPham>
    {
        public void Configure(EntityTypeBuilder<SanPham> builder)
        {
            builder.ToTable("SanPham");
            builder.HasKey(p => new { p.IDSP});
            builder.Property(p => p.TenSP).HasColumnName("TenSP").HasColumnType("nvarchar(50)");
            //builder.Property(p => p.SLTon).HasColumnName("SLTon").HasColumnType("int");
            builder.Property(p => p.TrangThai).HasColumnName("TrangThai").HasColumnType("int");
        }
    }
}
