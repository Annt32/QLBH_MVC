using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class GioHang
    {
        [Key]
        public Guid CartID { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; } // Khóa ngoại tham chiếu đến User
        public DateTime NgayThem { get; set; }
        public int TongSoLuong { get; set; }

        public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; }

        public virtual User User { get; set; } // Navigation property
    }
}
