using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App_Data_ClassLib.Migrations
{
    public partial class SanPham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hang",
                columns: table => new
                {
                    IDHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenHang = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    QuocGia = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hang", x => x.IDHang);
                });

            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                columns: table => new
                {
                    IDKM = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Giam = table.Column<decimal>(type: "Decimal", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "DateTime", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "DateTime", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai", x => x.IDKM);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    IDSP = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenSP = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ImgURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongSL = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.IDSP);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    NamSinh = table.Column<DateTime>(type: "DateTime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "SanPhamChiTiet",
                columns: table => new
                {
                    IDSPCT = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDSP = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenSP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaSP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Soluong = table.Column<int>(type: "int", nullable: false),
                    MauSac = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    KichThuoc = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhamChiTiet", x => x.IDSPCT);
                    table.ForeignKey(
                        name: "FK_SanPhamChiTiet_Hang_IDHang",
                        column: x => x.IDHang,
                        principalTable: "Hang",
                        principalColumn: "IDHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanPhamChiTiet_SanPham_IDSP",
                        column: x => x.IDSP,
                        principalTable: "SanPham",
                        principalColumn: "IDSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GioHang",
                columns: table => new
                {
                    CartID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayThem = table.Column<DateTime>(type: "DateTime", nullable: false),
                    TongSoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHang", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_GioHang_users_UserID",
                        column: x => x.UserID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    IDHD = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayMua = table.Column<DateTime>(type: "DateTime", nullable: false),
                    TongTien = table.Column<decimal>(type: "Decimal", nullable: false),
                    HinhThucThanhToan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.IDHD);
                    table.ForeignKey(
                        name: "FK_HoaDon_users_UserID",
                        column: x => x.UserID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiohangChiTiet",
                columns: table => new
                {
                    CartItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDSPCT = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiohangChiTiet", x => x.CartItemID);
                    table.ForeignKey(
                        name: "FK_GiohangChiTiet_GioHang_CartID",
                        column: x => x.CartID,
                        principalTable: "GioHang",
                        principalColumn: "CartID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiohangChiTiet_SanPhamChiTiet_IDSPCT",
                        column: x => x.IDSPCT,
                        principalTable: "SanPhamChiTiet",
                        principalColumn: "IDSPCT",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoaDonChiTiet",
                columns: table => new
                {
                    IDHDCT = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDHD = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDSPCT = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDKM = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiaSP = table.Column<decimal>(type: "Decimal", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonChiTiet", x => x.IDHDCT);
                    table.ForeignKey(
                        name: "FK_HoaDonChiTiet_HoaDon_IDHD",
                        column: x => x.IDHD,
                        principalTable: "HoaDon",
                        principalColumn: "IDHD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDonChiTiet_KhuyenMai_IDKM",
                        column: x => x.IDKM,
                        principalTable: "KhuyenMai",
                        principalColumn: "IDKM",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDonChiTiet_SanPhamChiTiet_IDSPCT",
                        column: x => x.IDSPCT,
                        principalTable: "SanPhamChiTiet",
                        principalColumn: "IDSPCT",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_UserID",
                table: "GioHang",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GiohangChiTiet_CartID",
                table: "GiohangChiTiet",
                column: "CartID");

            migrationBuilder.CreateIndex(
                name: "IX_GiohangChiTiet_IDSPCT",
                table: "GiohangChiTiet",
                column: "IDSPCT");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_UserID",
                table: "HoaDon",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonChiTiet_IDHD",
                table: "HoaDonChiTiet",
                column: "IDHD");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonChiTiet_IDKM",
                table: "HoaDonChiTiet",
                column: "IDKM");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonChiTiet_IDSPCT",
                table: "HoaDonChiTiet",
                column: "IDSPCT");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiet_IDHang",
                table: "SanPhamChiTiet",
                column: "IDHang");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiet_IDSP",
                table: "SanPhamChiTiet",
                column: "IDSP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiohangChiTiet");

            migrationBuilder.DropTable(
                name: "HoaDonChiTiet");

            migrationBuilder.DropTable(
                name: "GioHang");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "KhuyenMai");

            migrationBuilder.DropTable(
                name: "SanPhamChiTiet");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Hang");

            migrationBuilder.DropTable(
                name: "SanPham");
        }
    }
}
