using App_Data_ClassLib.Models;
using App_Data_ClassLib.Repository;
using App_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;


namespace App_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SD18302_NET104Context _context;
        AllRepository<SanPham> _repos;
        DbSet<SanPham> _sanPham;
        
        public HomeController(ILogger<HomeController> logger, SD18302_NET104Context context)
        {
            _logger = logger;
            _context = context; // Bây giờ 'context' sẽ được truyền vào thông qua DI
            _sanPham = _context.SanPhams;
            _repos = new AllRepository<SanPham>(_sanPham, _context);

        }

        //public Guid GetOrCreateUserIdFromSession()
        //{
        //    var sessionUserId = HttpContext.Session.GetString("SessionUserID");
        //    if (!string.IsNullOrEmpty(sessionUserId))
        //    {
        //        return Guid.Parse(sessionUserId);
        //    }
        //    else
        //    {
        //        var newUserId = Guid.NewGuid();
        //        HttpContext.Session.SetString("SessionUserID", newUserId.ToString());
        //        return newUserId;
        //    }
        //}

        private Guid GetUserOrCreateNewSessionGuid()
        {
            // Trước hết, kiểm tra xem có UserID trong Claims không
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            // Nếu không có trong Claims, kiểm tra trong Session
            var sessionUserId = HttpContext.Session.GetString("UserID"); // Đã cập nhật để sử dụng "UserID" thay vì "SessionUserID"
            if (sessionUserId != null && Guid.TryParse(sessionUserId, out Guid sessionGuid))
            {
                return sessionGuid;
            }

            // Nếu không tìm thấy UserID trong cả Claims và Session, tạo mới và lưu vào Session
            sessionGuid = Guid.NewGuid();
            HttpContext.Session.SetString("UserID", sessionGuid.ToString());

            return sessionGuid;
        }

        public IActionResult Index()
        {
            var products = _context.SanPhams.ToList(); // Giả sử bạn lấy tất cả sản phẩm
                                                       // Lấy UserID từ session hoặc tạo mới nếu chưa có
            var userId = GetUserOrCreateNewSessionGuid();

            // Truyền UserID vào ViewBag để sử dụng trong View
            ViewBag.SessionUserID = userId.ToString();

          
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
        
        //private Guid GetUserIdFromClaimsPrincipal()
        //{
        //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");
        //    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        //    {
        //        return userId;
        //    }

        //    return Guid.Empty; // Hoặc xử lý tùy chỉnh nếu không tìm thấy UserID
        //}
        //public IActionResult AddToCart(Guid id)
        //{

        //    var product = _repos.GetByID(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    var userID = GetUserIdFromClaimsPrincipal();
        //    var gioHang = _context.GioHang.FirstOrDefault(gh => gh.UserID == userID);

        //    if (gioHang == null)
        //    {
        //        // Nếu giỏ hàng không tồn tại, tạo mới
        //        gioHang = new GioHang
        //        {
        //            CartID = Guid.NewGuid(),
        //            UserID = userID,
        //            NgayThem = DateTime.Now,
        //            TongSoLuong = 0,
        //            // Khởi tạo các thuộc tính khác nếu cần
        //        };
        //        _context.GioHang.Add(gioHang);
        //    }

        //    var sanPhamChiTiet = _context.SanPhamCTs
        //.Include(s => s.SanPham)
        //.FirstOrDefault(s => s.IDSPCT == id);

        //    //string tenSanPham = sanPhamChiTiet.SanPham.TenSP;

        //    if (sanPhamChiTiet == null)
        //    {
        //        return NotFound();
        //    }
        //    // Tìm xem sản phẩm đã có trong giỏ chưa
        //    var chiTiet = _context.GioHangChiTiets
        //                           .FirstOrDefault(ct => ct.IDSPCT == id && ct.CartID == gioHang.CartID);

        //    if (chiTiet != null)
        //    {
        //        // Nếu đã có, tăng số lượng
        //        chiTiet.SoLuong++;
        //    }
        //    else
        //    {
        //        // Nếu chưa có, thêm mới
        //        _context.GioHangChiTiets.Add(new GioHangChiTiet
        //        {
        //            CartItemID = Guid.NewGuid(),
        //            CartID = gioHang.CartID,
        //            IDSPCT = id,
        //            SoLuong = 1,
        //            TrangThai = 1, // Hoặc trạng thái phù hợp                  
        //        });
        //    }
        //    var chiTietViewModel = new GioHangChiTietViewModel
        //    {
        //        // Các thuộc tính khác...
        //        TenSanPham = sanPhamChiTiet.SanPham.TenSP
        //    };
        //    // Cập nhật tổng số lượng và/hoặc tổng giá trị giỏ hàng nếu cần
        //    //gioHangViewModel.GioHangChiTiets.Add(chiTietViewModel);

        //    gioHang.TongSoLuong++;

        //    // Lưu tất cả các thay đổi vào cơ sở dữ liệu
        //    _context.SaveChanges();

        //    return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang xem giỏ hàng
        //}



        //[HttpPost]
        //public ActionResult AddToCart(Guid id)
        //{
        //    // Tìm sản phẩm dựa trên ID sử dụng repository hoặc service
        //    var sanPham = _repos.GetByID(id);

        //    // Nếu tìm thấy sản phẩm, thực hiện thêm vào giỏ hàng
        //    if (sanPham != null)
        //    {
        //        // Lấy thông tin giỏ hàng của người dùng từ cơ sở dữ liệu hoặc tạo mới nếu chưa có
        //        var gioHang = _repos.GetByID(User.Identity.GetUserId());
        //        if (gioHang == null)
        //        {
        //            gioHang = new GioHangViewModel
        //            {
        //                CartID = Guid.NewGuid(),
        //                UserID = User.Identity.GetUserId(),
        //                NgayThem = DateTime.Now,
        //                GioHangChiTiets = new List<GioHangChiTietViewModel>()
        //            };
        //            // Thêm giỏ hàng mới vào cơ sở dữ liệu nếu cần
        //            _repos.CreateObj(gioHang);
        //        }

        //        // Tìm chi tiết giỏ hàng có sản phẩm đó không
        //        var chiTietGioHang = gioHang.GioHangChiTiets
        //                                  .FirstOrDefault(ct => ct.IDSP == id);
        //        if (chiTietGioHang == null)
        //        {
        //            // Tạo mới chi tiết giỏ hàng nếu sản phẩm chưa có trong giỏ
        //            chiTietGioHang = new GioHangChiTietViewModel
        //            {
        //                CartItemID = Guid.NewGuid(),
        //                IDSP = id,
        //                CartID = gioHang.CartID,
        //                SoLuong = 1,
        //                TrangThai = 1,
        //                SanPham = sanPham
        //            };
        //            gioHang.GioHangChiTiets.Add(chiTietGioHang);
        //        }
        //        else
        //        {
        //            // Nếu đã có sản phẩm trong giỏ, tăng số lượng
        //            chiTietGioHang.SoLuong++;
        //        }

        //        // Lưu thay đổi vào cơ sở dữ liệu
        //        _repos.UpdateObj(gioHang);

        //        return RedirectToAction("Index", "Cart");
        //    }
        //    else
        //    {
        //        // Xử lý trường hợp không tìm thấy sản phẩm
        //        return View("Error");
        //    }
        //}


    }
}