using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class SanPham
    {
        [Key]
        public Guid IDSP { get; set; }
        public string TenSP { get; set; }
        public string ImgURL { get; set; }
        public int TongSL { get; set; }
        public int TrangThai { get; set; }     
        public virtual ICollection<SanPhamChiTiet> SanPhamChiTiets { get; set; }

    }
}
