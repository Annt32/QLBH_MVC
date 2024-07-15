using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class SanPhamChiTiet
    {
        [Key]
        public Guid IDSPCT { get; set; }
        public Guid IDSP { get; set; } 
        public Guid IDHang { get; set; }
        public string TenSP { get; set; }
        public string ImgURL { get; set; }
        public Decimal GiaSP { get; set; }
        public int Soluong { get; set; }
        public string MauSac { get; set; }
        public string KichThuoc { get; set; }
        public string MoTa { get; set; }
        public virtual SanPham SanPham { get; set; }       
        public virtual Hang Hang { get; set; }

    }
}
