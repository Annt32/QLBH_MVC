using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Data_ClassLib.lRepository
{
    internal interface IAllRepository<T> where T : class
    {
        public ICollection<T> GetAll(); // Lấy ra tất cả
        public T GetByID(dynamic id); // Dùng được Type. Lấy bởi ID
        public bool CreateObj(T obj); // Thêm vào trong Db
        public bool UpdateObj(T obj); // Sửa và Lưu lại vào Db
        public bool DeleteObj(dynamic id); // Xóa đối tượng trong Db
    }
}
