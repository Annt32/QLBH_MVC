using App_Data_ClassLib.Models;
using App_Data_ClassLib.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App_MVC.Controllers
{
    public class SanPhamCTController : Controller
    {
        SD18302_NET104Context _context;
        AllRepository<SanPhamChiTiet> _repos;
        DbSet<SanPhamChiTiet> _sanPhamCT;
        // GET: SanPhamCTController
        public SanPhamCTController()
        {
            // Khởi tạo DBcontext
            _context = new SD18302_NET104Context();
            // Khởi tạo repository với 2 tham số là Dbset và DBContext
            _sanPhamCT = _context.SanPhamCTs;
            _repos = new AllRepository<SanPhamChiTiet>(_sanPhamCT, _context);
        }
        [HttpGet]
        public IActionResult CreateCT(Guid id)
        {
            var newSanPhamCT = new SanPhamChiTiet
            {
                IDSP = id, 
                IDHang = Guid.Empty, 
                IDSPCT = Guid.NewGuid()
            };

            return View(newSanPhamCT);
        }
        [HttpPost]
        public IActionResult CreateCT(SanPhamChiTiet sanPhamCT, IFormFile imgFile)
        {

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", imgFile.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                // Thực hiện sao chép ảnh vào thư mục root
                imgFile.CopyTo(stream);
            }
            sanPhamCT.ImgURL = imgFile.FileName;
            var sanPham = _context.SanPhams.Find(sanPhamCT.IDSP);
            if (sanPham != null)
            {         
                sanPhamCT.TenSP = sanPham.TenSP + " - " + sanPhamCT.MauSac + " - " + sanPhamCT.KichThuoc;
            }      
            else
            {

            }
            _repos.CreateObj(sanPhamCT);
            return RedirectToAction("SanPhamIndex", "SanPham");
        }

        // GET: SanPhamCTController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        

        public IActionResult EditCT(Guid id) // Form load ra đối tượng cần sửa
        {
            // Lấy ra đối tượng cần sửa
            var updateSanPham = _repos.GetByID(id);
            return View(updateSanPham);
        }
        // Sửa
        [HttpPost]

        public IActionResult EditCT(SanPhamChiTiet sanPham) // Action này thực hiện thay đổi, khi cần thì trỏ tới nó
        {
            
            _repos.UpdateObj(sanPham);
            return RedirectToAction("SanPhamIndex", "SanPham");
        }
        public IActionResult Delete(Guid id)
        {
            _repos.DeleteObj(id);
            return RedirectToAction("SanPhamIndex", "SanPham");
        }

    }
}
