using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class ThongKeViewModel
    {
        public IEnumerable<HoaDon> HoaDons { get; set; }
        public IEnumerable<HoaDonChiTiet> HoaDonChiTiets { get; set; }
    }
    public class ChiTietHoaDonViewModel
    {
        public HoaDon HoaDon { get; set; }
        public List<HoaDonChiTiet> HoaDonChiTiets { get; set; }
    }

}
