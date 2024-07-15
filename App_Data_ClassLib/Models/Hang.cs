using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class Hang
    {
        public Guid IDHang { get; set; }
        public string TenHang { get; set; }
        public string QuocGia { get; set;}
        public virtual ICollection<SanPhamChiTiet> SanPhamChiTiets { get; set; }

    }
}
