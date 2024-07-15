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
    internal class UserConfigs : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            

            builder.ToTable("users");
            builder.HasKey(p => p.UserID);
            builder.Property(p => p.Ten).HasColumnName("Ten").HasColumnType("nvarchar(50)");
            builder.Property(p => p.Username).HasColumnName("Username").HasColumnType("nvarchar(50)");
            builder.Property(p => p.Password).HasColumnName("Password").HasColumnType("nvarchar(50)");
            builder.Property(p => p.SoDienThoai).HasColumnName("SoDienThoai").HasColumnType("nvarchar(50)");
            builder.Property(p => p.DiaChi).HasColumnName("DiaChi").HasColumnType("nvarchar(50)");
            builder.Property(p => p.NamSinh).HasColumnName("NamSinh").HasColumnType("DateTime");
            builder.Property(p => p.Role).HasColumnName("Role").HasColumnType("nvarchar(50)");
            builder.HasOne(p => p.GioHang).WithOne(p => p.User).HasForeignKey<GioHang>(p => p.UserID);
        }


    }
}
