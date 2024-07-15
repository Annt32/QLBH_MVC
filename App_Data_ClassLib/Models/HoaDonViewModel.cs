using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class HoaDonViewModel
    {
        // Thông tin từ bảng HoaDon
        public Guid IDHD { get; set; }
        public Guid UserID { get; set; }
        public Guid? IDKM { get; set; }
        public DateTime NgayMua { get; set; }
        public Decimal TongTien { get; set; }
        public int HinhThucThanhToan { get; set; }
        public decimal TongTienBanDau { get; set; }
        public string KhuyenMaiMoTa { get; set; }
        public decimal KhuyenMaiPhanTramGiam { get; set; }
        public User User { get; set; } // Giả sử bạn có một lớp User tương ứng
        public string ten { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NamSinh { get; set; } // Giả sử bạn dùng DateTime, hoặc string nếu chỉ lưu năm

        // Danh sách các HoaDonChiTietViewModel để hiển thị thông tin chi tiết từ bảng HoaDonChiTiet
        public List<HoaDonChiTietViewModel> ChiTietSanPhams { get; set; } = new List<HoaDonChiTietViewModel>();
        public Dictionary<Guid, string> TenSanPhamDict { get; set; }

    }

    public class HoaDonChiTietViewModel
    {
        // Thông tin từ bảng HoaDonChiTiet
        public Guid IDHDCT { get; set; }
        public Guid IDSPCT { get; set; }
        public Guid IDKM { get; set; }
        public Decimal GiaSP { get; set; }
        public int SoLuong { get; set; }
        public string TenSP { get; set; }

        // Thêm bất kỳ thuộc tính liên quan khác từ SanPhamChiTiet và KhuyenMai nếu cần
        public SanPhamChiTiet SanPhamChiTiet { get; set; } // Giả sử bạn có một lớp SanPhamChiTiet tương ứng
        public KhuyenMai KhuyenMai { get; set; } // Giả sử bạn có một lớp KhuyenMai tương ứng
    }

}
