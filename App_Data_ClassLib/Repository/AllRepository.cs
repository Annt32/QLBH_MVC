using App_Data_ClassLib.lRepository;
using App_Data_ClassLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App_Data_ClassLib.Repository
{
    public class AllRepository<G> : IAllRepository<G> where G : class
    {
        SD18302_NET104Context context;
        DbSet<G> dbset; //CRUD trên DBset vì nó đại diện cho bảng
        // Khi cần gọi lại và dùng thật thì lại cần chính xác nó là DBset Nào
        //Lúc đó ta sẽ gán Dbset = DBset cần dùng.
        public AllRepository()
        {
            context = new SD18302_NET104Context();

        }
        public AllRepository(DbSet<G> dbset, SD18302_NET104Context context)
        {
            this.dbset = dbset; // Gán lại khi dùng.
            this.context = context;
        }
        public bool CreateObj(G obj)
        {
            try
            {
                dbset.Add(obj);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool DeleteObj(dynamic id)
        {
            try
            {
                //tìm đối tượng cần xóa
                var deleteObj = dbset.Find(id); // find truyền vào thuộc tính
                //chỉ sử dụng với PK
                dbset.Remove(deleteObj);// Xóa
                context.SaveChanges(); // Lưu lại
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public ICollection<G> GetAll()
        {
            return dbset.ToList();
        }

        public G GetByID(dynamic id)
        {
            return dbset.Find(id);
        }
        public G FindByUsername(string username, Guid currentUserId)
        {
            return context.Set<User>().FirstOrDefault(u => u.Username == username && u.UserID != currentUserId) as G;
        }
        public G CheckEmail(string email, Guid currentUserId)
        {
            return context.Set<User>().FirstOrDefault(u => u.Email == email && u.UserID != currentUserId) as G;
        }
        public G CheckSdt(string SoDienThoai, Guid currentUserId)
        {
            return context.Set<User>().FirstOrDefault(u => u.SoDienThoai == SoDienThoai && u.UserID != currentUserId) as G;
        }
        public G CheckName(string Name, Guid currentUserId)
        {
            return context.Set<User>().FirstOrDefault(u => u.Ten == Name && u.UserID != currentUserId) as G;
        }
        public G CheckNameSP(string NameSP, Guid currentUserId)
        {
            return context.Set<SanPham>().FirstOrDefault(u => u.TenSP == NameSP && u.IDSP != currentUserId) as G;
        }        

        public bool UpdateObj(G obj)
        {
            try
            {
                dbset.Update(obj); // sửa
                context.SaveChanges(); // Lưu lại
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        

    }
}
