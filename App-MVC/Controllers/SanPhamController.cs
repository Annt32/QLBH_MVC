using App_Data_ClassLib.Models;
using App_Data_ClassLib.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;



namespace App_MVC.Controllers
{
    public class SanPhamController : Controller
    {
        SD18302_NET104Context _context;
        AllRepository<SanPham> _repos;
        DbSet<SanPham> _sanPham;

        public SanPhamController()
        {
            // Khởi tạo DBcontext
            _context = new SD18302_NET104Context();
            // Khởi tạo repository với 2 tham số là Dbset và DBContext
            _sanPham = _context.SanPhams;
            _repos = new AllRepository<SanPham>(_sanPham, _context);
        }

        public IActionResult SanPhamIndex()
        {
            // Đầu tiên lấy tất cả sản phẩm
            var sanPhams = _context.SanPhams.ToDictionary(sp => sp.IDSP, sp => sp);

            // Tính tổng số lượng sản phẩm chi tiết, nếu không có sản phẩm chi tiết thì tổng số lượng là 0
            var tongSoLuongSanPham = _context.SanPhamCTs
                 .Where(spct => spct.IDSP != null)
                 .GroupBy(spct => spct.IDSP)
                 .Select(group => new {
                     IDSP = group.Key,
                     TongSL = group.Sum(spct => spct.Soluong)
                 }).ToList();

            // Đối với mỗi sản phẩm
            foreach (var sanPham in sanPhams.Values)
            {
                var tongSLItem = tongSoLuongSanPham.FirstOrDefault(t => t.IDSP == sanPham.IDSP);
                if (tongSLItem != null)
                {
                    sanPham.TongSL = tongSLItem.TongSL;
                    sanPham.TrangThai = tongSLItem.TongSL > 0 ? 1 : 0;
                }
                else
                {
                    sanPham.TongSL = 0;
                    sanPham.TrangThai = 0;
                }
            }

            _context.SaveChanges();

            return View(sanPhams.Values.ToList());
        }

        //public async Task<IActionResult> SanPhamIndex()
        //{
        //    var tongSoLuongSanPham = _context.SanPhamCTs
        //     .Where(spct => spct.IDSP != null) // Đảm bảo IDSP không phải là null
        //     .GroupBy(spct => spct.IDSP) // Nhóm theo IDSP
        //     .Select(group => new {
        //         IDSP = group.Key, // IDSP là khóa nhóm
        //         TongSL = group.Sum(spct => spct.Soluong) // Tính tổng số lượng cho mỗi nhóm
        //     }).ToList();

        //    // Cập nhật thông tin tổng số lượng và trạng thái sản phẩm
        //    foreach (var item in tongSoLuongSanPham)
        //    {
        //        var sanPham = _context.SanPhams.Find(item.IDSP);
        //        if (sanPham != null)
        //        {
        //            sanPham.TongSL = item.TongSL; // Cập nhật tổng số lượng
        //            sanPham.TrangThai = item.TongSL > 0 ? 1 : 0; // Nếu tồn kho > 0, trạng thái là còn hàng (1), ngược lại là hết hàng (2)
        //        }
        //    }


        //    _context.SaveChanges(); // Lưu thay đổi vào database

        //    var sanPhamData = _repos.GetAll();
        //    return View(sanPhamData);
        //}

        // Thêm data
        public IActionResult Create() // Action để mở form điền thông tin user
        {
           
            return View();
        }
        // Action để thực hiện thêm vào DB
        [HttpPost]
        public IActionResult Create(SanPham sanPham, IFormFile imgFile)
        {
            var CheckNameSP = _repos.CheckNameSP(sanPham.TenSP, sanPham.IDSP);
            if (CheckNameSP != null)
            {
                ModelState.AddModelError("TenSP", "Tên sản phẩm đã được sử dụng.");
                return View(sanPham);
            }
            //if (sanPham.GiaSP < 1000)
            //{
            //    ModelState.AddModelError("GiaSP", "Giá phải >= 1000");
            //    return View(sanPham);
            //}
            //if (sanPham.SLTon < 0)
            //{
            //    ModelState.AddModelError("SLTon", "Số lượng tồn không được nhỏ hơn 0");
            //    return View(sanPham);
            //}
            if (sanPham.TrangThai < 0 || sanPham.TrangThai > 1)
            {
                ModelState.AddModelError("TrangThai", "Trạng thái không hợp lệ ( 1 - còn hàng, 0 - Hết hàng)");
                return View(sanPham);
            }
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", imgFile.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                // Thực hiện sao chép ảnh vào thư mục root
                imgFile.CopyTo(stream);
            }
            sanPham.ImgURL = imgFile.FileName;

            sanPham.IDSP = Guid.NewGuid();
            _repos.CreateObj(sanPham);
            return RedirectToAction("SanPhamIndex");
        }
        
        // Sửa
        public IActionResult Edit(Guid id) // Form load ra đối tượng cần sửa
        {
            // Lấy ra đối tượng cần sửa
            var updateSanPham = _repos.GetByID(id);
            return View(updateSanPham);
        }
        // Sửa
        [HttpPost]

        public IActionResult Edit(SanPham sanPham) // Action này thực hiện thay đổi, khi cần thì trỏ tới nó
        {
            var CheckNameSP = _repos.CheckNameSP(sanPham.TenSP, sanPham.IDSP);
            if (CheckNameSP != null)
            {
                ModelState.AddModelError("TenSP", "Tên sản phẩm đã được sử dụng.");
                return View(sanPham);
            }
            //if (sanPham.GiaSP < 1000)
            //{
            //    ModelState.AddModelError("GiaSP", "Giá phải >= 1000");
            //    return View(sanPham);
            //}
            //if (sanPham.SLTon < 0)
            //{
            //    ModelState.AddModelError("SLTon", "Số lượng tồn không được nhỏ hơn 0");
            //    return View(sanPham);
            //}
            if (sanPham.TrangThai < 0 || sanPham.TrangThai > 1)
            {
                ModelState.AddModelError("TrangThai", "Trạng thái không hợp lệ ( 1 - còn hàng, 0 - Hết hàng)");
                return View(sanPham);
            }      
            _repos.UpdateObj(sanPham);
            return RedirectToAction("SanPhamIndex");
        }
        // Xóa
        public IActionResult Delete(Guid id)
        {
           
            //var deleteUser = _repos.GetByID(id);
            //var jsonData = HttpContext.Session.GetString("deleted");

            //// Kiểm tra xem đã có danh sách sản phẩm trong session chưa
            //List<SanPham> deletedProducts = string.IsNullOrEmpty(jsonData) ? new List<SanPham>() : JsonConvert.DeserializeObject<List<SanPham>>(jsonData);

            //// Thêm sản phẩm vào danh sách và cập nhật lại session
            //deletedProducts.Add(deleteUser);
            //HttpContext.Session.SetString("deleted", JsonConvert.SerializeObject(deletedProducts));

            _repos.DeleteObj(id);
            return RedirectToAction("SanPhamIndex");
            

        }
        //public IActionResult RollBack()
        //{
        //    if (HttpContext.Session.Keys.Contains("deleted"))
        //    {
        //        var jsonData = HttpContext.Session.GetString("deleted");
        //        var deletedProducts = JsonConvert.DeserializeObject<List<SanPham>>(jsonData);

        //        // Khôi phục từng sản phẩm
        //        foreach (var product in deletedProducts)
        //        {
        //            _repos.CreateObj(product);
        //        }

        //        // Xóa danh sách sản phẩm đã khôi phục khỏi session
        //        HttpContext.Session.Remove("deleted");

        //        return RedirectToAction("SanPhamIndex");
        //    }
        //    else
        //    {
        //        return Content("No products to rollback.");
        //    }
        //}
        // Thông tin Details
        public IActionResult Details(Guid id)
        {
            var getSanPham = _repos.GetByID(id);
            return View(getSanPham);
        }
        

    }
}
