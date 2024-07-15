using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class GioHangChiTiet
    {
        [Key]
        public Guid CartItemID { get; set; }
        public Guid CartID { get; set; }
        public Guid IDSPCT { get; set; }
        public int SoLuong { get; set; }
        public int TrangThai { get; set; }
        public virtual GioHang GioHang { get; set; }
        public virtual SanPhamChiTiet SanPhamChiTiet { get; set; }

    }
}
