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
    internal class HangConfigs : IEntityTypeConfiguration<Hang>
    {
        public void Configure(EntityTypeBuilder<Hang> builder)
        {
            builder.ToTable("Hang");
            builder.HasKey(p => new { p.IDHang });
            builder.Property(p => p.TenHang).HasColumnName("TenHang").HasColumnType("nvarchar(50)");
            builder.Property(p => p.QuocGia).HasColumnName("QuocGia").HasColumnType("nvarchar(50)");
        }
    }
}
