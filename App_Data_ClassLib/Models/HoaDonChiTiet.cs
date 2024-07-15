using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class HoaDonChiTiet
    {
        [Key]
        public Guid IDHDCT { get; set; }
        public Guid IDHD { get; set; }
        public Guid IDSPCT { get; set; }
        public Guid IDKM { get; set; }
        public Decimal GiaSP { get; set; }
        public int SoLuong { get; set; }

        public virtual HoaDon HoaDon { get; set; }
        public virtual SanPhamChiTiet SanPhamChiTiet { get; set; }
        public virtual KhuyenMai KhuyenMai { get; set; }

    }
}
