using App_Data_ClassLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App_MVC.Controllers
{
    public class ThongKeViewModelController : Controller
    {
        private readonly SD18302_NET104Context _context;

        public ThongKeViewModelController(SD18302_NET104Context context)
        {
            _context = context;
        }

        private Guid GetUserIdFromClaimsPrincipal()
        {
            //var role = HttpContext.Session.Get<User>("role");
            //var role = HttpContext.Session.GetString("role");
            //if (role == "Admin")
            //{
            //    return Guid.NewGuid();
            //}
            // Trước hết, kiểm tra xem có UserID trong Claims không
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }


            // Nếu không có trong Claims, kiểm tra trong Session
            var sessionUserId = HttpContext.Session.GetString("UserID");
            if (sessionUserId != null && Guid.TryParse(sessionUserId, out Guid sessionGuid))
            {
                return sessionGuid;
            }

            // Nếu không tìm thấy UserID trong cả Claims và Session, tạo mới và lưu vào Session
            sessionGuid = Guid.NewGuid();
            HttpContext.Session.SetString("UserID", sessionGuid.ToString());

            return sessionGuid;
        }

        public async Task<IActionResult> Index(Guid? userId)
        {
            // Kiểm tra xem người dùng hiện tại có phải là Admin không từ session
            var role = HttpContext.Session.GetString("role");
            bool isAdmin = role == "Admin";
            List<HoaDon> hoaDons;

            if (isAdmin)
            {
                // Nếu là Admin và có một UserID cụ thể, lấy hóa đơn của User đó
                // Nếu không có UserID, lấy tất cả hóa đơn
                hoaDons = await _context.HoaDons
                                        .Where(hd => !userId.HasValue || hd.UserID == userId.Value)
                                        .ToListAsync();
            }
            else
            {
                // Nếu không phải là Admin, lấy UserID từ ClaimsPrincipal
                var currentUserId = GetUserIdFromClaimsPrincipal();
                hoaDons = await _context.HoaDons
                                        .Where(hd => hd.UserID == currentUserId)
                                        .ToListAsync();
            }

            // Lấy tất cả chi tiết hóa đơn và thông tin khuyến mãi liên quan
            var hoaDonChiTiets = new List<HoaDonChiTiet>();
            foreach (var hoaDon in hoaDons)
            {
                var chiTiets = await _context.HoaDonChiTiet
                                            .Include(hdct => hdct.KhuyenMai)
                                            .Where(hdct => hdct.IDHD == hoaDon.IDHD)
                                            .ToListAsync();
                hoaDonChiTiets.AddRange(chiTiets);
            }

            // Tạo và gửi ViewModel đến view
            var viewModel = new ThongKeViewModel
            {
                HoaDons = hoaDons,
                HoaDonChiTiets = hoaDonChiTiets
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ChiTietHoaDon(Guid? IDHD)
        {
            // Bạn cần chuyển IDHD như một tham số để xác định hóa đơn cụ thể
            if (!IDHD.HasValue)
            {
                return NotFound(); // Hoặc một thông báo lỗi thích hợp
            }

            // Lấy chi tiết hóa đơn dựa trên IDHD
            var hoaDonChiTiets = await _context.HoaDonChiTiet
                                                .Include(hdct => hdct.KhuyenMai)
                                                .Where(hdct => hdct.IDHD == IDHD.Value)
                                                .ToListAsync();

            // Nếu bạn cần thông tin hóa đơn chung để hiển thị cùng chi tiết
            var hoaDon = await _context.HoaDons
                                       .FirstOrDefaultAsync(hd => hd.IDHD == IDHD.Value);

            if (hoaDon == null)
            {
                return NotFound(); // Hoặc một thông báo lỗi thích hợp
            }

            // Tạo và gửi ViewModel đến view
            var viewModel = new ChiTietHoaDonViewModel
            {
                HoaDon = hoaDon,
                HoaDonChiTiets = hoaDonChiTiets
            };

            return View(viewModel);
        }


    }
}
