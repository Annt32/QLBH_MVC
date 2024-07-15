using Database;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class HoaDon
    {
        [Key]
        public Guid IDHD { get; set; }
        public int UserID { get; set; }
        public DateTime NgayMua { get; set; }
        public decimal TongTien { get; set; }
        public string HinhThucThanhToan { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
