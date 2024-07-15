using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class GioHangViewModel
    {
        public Guid CartID { get; set; }
        public Guid UserID { get; set; }
        public DateTime NgayThem { get; set; }
        public int TongSoLuong { get; set; }
        public string TenSP { get; set; }

        //public decimal  TongGiaTri { get; set; }
        public List<GioHangChiTietViewModel> GioHangChiTiets { get; set; }
        public Dictionary<Guid, string> TenSanPhamDict { get; set; }

    }

    public class GioHangChiTietViewModel
    {
        public Guid CartItemID { get; set; }
        public Guid IDSPCT { get; set; }
        public Guid IDSP { get; set; }
        public Guid CartID { get; set; }
        public int SoLuong { get; set; }
        public int TrangThai { get; set; }
        public string TenSanPham { get; set; }

        public virtual SanPham SanPham { get; set; }
        public virtual GioHang GioHang { get; set; }
    }

    public class GioHangSession
    {
        public Guid CartID { get; set; }
        public Guid UserID { get; set; }
        public DateTime NgayThem { get; set; }
        public int TongSoLuong { get; set; }
        public string TenSanPham { get; set; }

        public List<GioHangChiTiet> ChiTiets { get; set; } = new List<GioHangChiTiet>();
    }
}
