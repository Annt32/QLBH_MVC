using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class SanPhamViewModel
    {
        public SanPham SanPham { get; set; }
        public IEnumerable<SanPhamChiTiet> SanPhamChiTiets { get; set; }

    }

}
