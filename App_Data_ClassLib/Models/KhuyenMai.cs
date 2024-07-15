using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class KhuyenMai
    {
        [Key]
        public Guid IDKM { get; set; }
        public Decimal Giam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string MoTa { get; set; }
        public virtual ICollection<HoaDonChiTiet> HoaDonChitiets { get; set; }

    }
}
