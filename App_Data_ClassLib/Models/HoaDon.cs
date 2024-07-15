using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class HoaDon
    {
        [Key]
        public Guid IDHD { get; set; }
        public Guid UserID { get; set; }
        public DateTime NgayMua { get; set; }
        public Decimal TongTien { get; set; }
        public int HinhThucThanhToan { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<HoaDonChiTiet> HoaDonChitiets { get; set; }

    }
}
