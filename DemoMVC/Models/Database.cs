using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Database
{    
    //Scaffold-DbContext "Server=DESKTOP-PMB8531\SQLEXPRESS;Database=SD18302_NET104;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
    public class User
    {
        [Key]
        public Guid UserID { get; set; }
        public string Ten { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string SoDienThoai { get; set; }
        public string Address { get; set; }
        public DateTime NamSinh { get; set; } // ngày sinh

    }
    public class SanPham
    {
        [Key]
        public int IDSP { get; set; }
        public decimal GiaSP { get; set; }
        public int SLTon { get; set; }
        public string TrangThai { get; set; }
        public string TenSP { get; set; }
        public string MoTa { get; set; }

  
    }

    public class GioHang
    {
        [Key]
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime NgayThem { get; set; }
        public int TongSoLuong { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }

    public class GioHangChitiet
    {
        [Key]
        public int CartItemID { get; set; }
        public int UserID { get; set; }
        public int IDSP { get; set; }
        public int CartID { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public string TrangThai { get; set; }

        [ForeignKey("UserID ")]
        public virtual User User { get; set; }

        [ForeignKey("IDSP")]
        public virtual SanPham SanPham { get; set; }

        [ForeignKey("CartID")]
        public virtual GioHang GioHang { get; set; }
    }

    public class HoaDon
    {
        [Key]
        public int IDHD { get; set; }
        public int UserID { get; set; }
        public DateTime NgayMua { get; set; }
        public decimal TongTien { get; set; }
        public string HinhThucThanhToan { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }

    public class HoaDonChitiet
    {
        [Key]
        public int IDHDCT { get; set; }
        public decimal GiaSP { get; set; }
        public int IDHD { get; set; }
        public int IDSP { get; set; }
        public int SoLuong { get; set; }
        public string KhuyenMai { get; set; }

        [ForeignKey("IDHD")]
        public virtual HoaDon HoaDon { get; set; }

        [ForeignKey("IDSP")]
        public virtual SanPham SanPham { get; set; }
    }
}