using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Models
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; }
        public string Ten { get; set; }
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Email phải có '@' và một dấu chấm sau '@'.")]
        public string Email { get; set; }
        [MaxLength(256)]
        public string Password { get; set; }
        public string Username { get; set; }
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Role { get; set; }
        public DateTime NamSinh { get; set; } // ngày sinh
        public virtual GioHang GioHang { get; set; }
        //ICollection<HoaDon> chỉ để thể hiện một User sẽ có nhiều hóa đơn.
        //ICollection<HoaDon> còn được để dùng để trỏ đến khi cần 
        public virtual ICollection<HoaDon> HoaDons { get; set;}
        //public virtual ICollection<GioHangChiTiet> GioHangChitiets { get; set; }

        

    }
}
