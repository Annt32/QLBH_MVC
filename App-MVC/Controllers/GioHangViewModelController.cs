using App_Data_ClassLib.Models;
using App_Data_ClassLib.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace App_MVC.Controllers
{
    public class GioHangViewModelController : Controller
    {
        private readonly SD18302_NET104Context _context;
        AllRepository<GioHangChiTiet> _repos;
        DbSet<GioHangChiTiet> _giohangCT;



        public GioHangViewModelController()
        {
            _context = new SD18302_NET104Context();
            _giohangCT = _context.GioHangChiTiets;
            _repos = new AllRepository<GioHangChiTiet>(_giohangCT, _context);
        }

        //private Guid GetUserOrCreateNewSessionGuid()
        //{
        //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");
        //    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        //    {
        //        return userId;
        //    }
        //    else
        //    {
        //        var sessionUserId = HttpContext.Session.GetString("SessionUserID");
        //        if (sessionUserId == null || !Guid.TryParse(sessionUserId, out Guid sessionGuid))
        //        {
        //            sessionGuid = Guid.NewGuid();
        //            HttpContext.Session.SetString("SessionUserID", sessionGuid.ToString());
        //        }
        //        return sessionGuid;
        //    }
        //    return Guid.Empty; // Hoặc xử lý tùy chỉnh nếu không tìm thấy UserID
        //}

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


        public ActionResult Index()
        {
            var userID = GetUserOrCreateNewSessionGuid();

            ViewBag.SessionUserID = userID.ToString();

            var isRealUser = _context.Users.Any(u => u.UserID == userID);

            GioHangViewModel gioHangViewModel = new GioHangViewModel();

            if (!isRealUser)
            {
               
                // Lấy thông tin giỏ hàng từ session
                var gioHangSession = HttpContext.Session.Get<GioHangSession>("GioHang");

                // Nếu giỏ hàng tồn tại trong session
                if (gioHangSession != null)
                {
                    // Chuyển đổi từ GioHangSession sang GioHangViewModel
                    gioHangViewModel = new GioHangViewModel
                    {
                        CartID = gioHangSession.CartID,
                        UserID = gioHangSession.UserID,
                        NgayThem = gioHangSession.NgayThem,
                        TongSoLuong = gioHangSession.TongSoLuong,
                        TenSanPhamDict = HttpContext.Session.Get<Dictionary<Guid, string>>("TenSanPhamSession") ?? new Dictionary<Guid, string>(),
                        GioHangChiTiets = gioHangSession.ChiTiets.Select(ct => new GioHangChiTietViewModel
                        {
                            CartItemID = ct.CartItemID,
                            IDSPCT = ct.IDSPCT,
                            CartID = ct.CartID,
                            SoLuong = ct.SoLuong,
                            TrangThai = ct.TrangThai,
                            // Bạn cần thêm logic tại đây để xác định và thiết lập tên sản phẩm
                            //TenSanPham = ct.TenSanPham
                        }).ToList()
                       

                    };
                }
                else
                {
                    gioHangViewModel = new GioHangViewModel();
                }

            }
            else
            {
                // Đối với người dùng thực (sử dụng database)
                var gioHang = _context.GioHang.FirstOrDefault(gh => gh.UserID == userID);

                if (gioHang != null)
                {
                    // Lấy thông tin chi tiết giỏ hàng từ database
                    var chiTietList = _context.GioHangChiTiets.Where(ct => ct.CartID == gioHang.CartID).ToList();

                    gioHangViewModel = new GioHangViewModel
                    {
                        CartID = gioHang.CartID,
                        UserID = gioHang.UserID,
                        NgayThem = gioHang.NgayThem,
                        TongSoLuong = gioHang.TongSoLuong,
                        GioHangChiTiets = chiTietList.Select(ct => new GioHangChiTietViewModel
                        {
                            CartItemID = ct.CartItemID,
                            IDSPCT = ct.IDSPCT,
                            CartID = ct.CartID,
                            SoLuong = ct.SoLuong,
                            TrangThai = ct.TrangThai,
                            TenSanPham = _context.SanPhamCTs.FirstOrDefault(spct => spct.IDSPCT == ct.IDSPCT)?.TenSP
                        }).ToList()
                    };
                }

            }
            return View(gioHangViewModel);

        }

        

        [HttpPost]
        public IActionResult ThanhToan(List<Guid> selectedProductIds)
        {
            // Tạo hóa đơn mới
            GioHangViewModel gioHangViewModel = new GioHangViewModel();

            var hoaDon = new HoaDonViewModel
            {
                IDHD = Guid.NewGuid(),
                NgayMua = DateTime.Now,
                TongTien = 0, // Sẽ cập nhật sau khi tính tổng
                              //... khởi tạo các trường khác nếu cần
            };
            var userID = GetUserOrCreateNewSessionGuid();
            var isRealUser = _context.Users.Any(u => u.UserID == userID);
            if (isRealUser)
            {
                hoaDon.UserID = userID; // Đặt UserID cho hóa đơn nếu là người dùng thực
                foreach (var productId in selectedProductIds)
                {
                    var chiTietSanPham = _context.GioHangChiTiets
                        .Include(ghct => ghct.SanPhamChiTiet)
                        .ThenInclude(spct => spct.SanPham)
                        .SingleOrDefault(ghct => ghct.CartItemID == productId);

                    if (chiTietSanPham != null)
                    {
                        var chiTietHoaDon = new HoaDonChiTietViewModel
                        {
                            IDHDCT = Guid.NewGuid(),
                            IDSPCT = chiTietSanPham.IDSPCT,
                            TenSP = chiTietSanPham.SanPhamChiTiet.TenSP,
                            GiaSP = chiTietSanPham.SanPhamChiTiet.GiaSP,
                            SoLuong = chiTietSanPham.SoLuong,
                            // Lấy thông tin khác từ chi tiết sản phẩm nếu cần
                        };

                        hoaDon.TongTien += chiTietSanPham.SanPhamChiTiet.GiaSP * chiTietSanPham.SoLuong;
                        hoaDon.ChiTietSanPhams.Add(chiTietHoaDon);
                    }
                }
                HttpContext.Session.SetString("HoaDon", JsonConvert.SerializeObject(hoaDon));

                // Code lưu thông tin hóa đơn ở đây

                return RedirectToAction("ShowHoaDon", "HoaDonViewModel");
            }
            else
            {
                // Người dùng là người dùng ảo
                // Lấy thông tin giỏ hàng từ session
                var gioHangSession = HttpContext.Session.Get<GioHangSession>("GioHang");
                if (gioHangSession == null)
                {
                    // Xử lý trường hợp không tìm thấy giỏ hàng trong session
                    // Có thể redirect về trang giỏ hàng hoặc hiển thị thông báo lỗi
                    return RedirectToAction("Index", "GioHang");
                }

                foreach (var chiTietSession in gioHangSession.ChiTiets)
                {
                    // Lấy thông tin sản phẩm từ IDSPCT trong chi tiết session
                    var sanPhamChiTiet = _context.SanPhamCTs
                        .Include(s => s.SanPham)
                        .SingleOrDefault(spct => spct.IDSPCT == chiTietSession.IDSPCT);

                    if (sanPhamChiTiet != null)
                    {
                        var chiTietHoaDon = new HoaDonChiTietViewModel
                        {
                            IDHDCT = Guid.NewGuid(),
                            IDSPCT = chiTietSession.IDSPCT,
                            TenSP = sanPhamChiTiet.TenSP,
                            GiaSP = sanPhamChiTiet.GiaSP,
                            SoLuong = chiTietSession.SoLuong,
                            // Lấy thông tin khác từ chi tiết sản phẩm nếu cần
                        };

                        hoaDon.TongTien += sanPhamChiTiet.GiaSP * chiTietSession.SoLuong;
                        hoaDon.ChiTietSanPhams.Add(chiTietHoaDon);
                    }
                }
            }

            // Lưu thông tin hóa đơn vào session hoặc database
            HttpContext.Session.SetString("HoaDon", JsonConvert.SerializeObject(hoaDon));



            return RedirectToAction("ShowHoaDon", "HoaDonViewModel");
        }

        //[HttpPost]
        //public IActionResult ThanhToan(List<Guid> selectedProductIds)
        //{
        //    // Tạo hóa đơn mới
        //    GioHangViewModel gioHangViewModel = new GioHangViewModel();

        //    var hoaDon = new HoaDonViewModel
        //    {
        //        IDHD = Guid.NewGuid(),
        //        UserID = GetUserOrCreateNewSessionGuid(), // Hoặc phương thức bạn sử dụng để lấy UserId
        //        NgayMua = DateTime.Now,
        //        TongTien = 0, // Sẽ cập nhật sau khi tính tổng
        //                      //... khởi tạo các trường khác nếu cần
        //    };

        //    // Lấy thông tin chi tiết từ cơ sở dữ liệu
        //    foreach (var productId in selectedProductIds)
        //    {
        //        var chiTietSanPham = _context.GioHangChiTiets
        //            .Include(ghct => ghct.SanPhamChiTiet)
        //            .ThenInclude(spct => spct.SanPham)
        //            .SingleOrDefault(ghct => ghct.CartItemID == productId);

        //        if (chiTietSanPham != null)
        //        {
        //            var chiTietHoaDon = new HoaDonChiTietViewModel
        //            {
        //                IDHDCT = Guid.NewGuid(),
        //                IDSPCT = chiTietSanPham.IDSPCT,
        //                TenSP = chiTietSanPham.SanPhamChiTiet.TenSP,
        //                GiaSP = chiTietSanPham.SanPhamChiTiet.GiaSP,
        //                SoLuong = chiTietSanPham.SoLuong,
        //                // Lấy thông tin khác từ chi tiết sản phẩm nếu cần
        //            };

        //            hoaDon.TongTien += chiTietSanPham.SanPhamChiTiet.GiaSP * chiTietSanPham.SoLuong;
        //            hoaDon.ChiTietSanPhams.Add(chiTietHoaDon);
        //        }
        //    }

        //    // Lưu thông tin hóa đơn vào session hoặc database
        //    HttpContext.Session.SetString("HoaDon", JsonConvert.SerializeObject(hoaDon));

        //    // Code lưu thông tin hóa đơn ở đây

        //    return RedirectToAction("ShowHoaDon", "HoaDonViewModel");
        //}


        //[HttpPost]
        //public IActionResult ThanhToan(List<Guid> selectedProductIds)
        //{
        //    // Lấy thông tin người dùng từ session hoặc authentication
        //    var userId = GetUserOrCreateNewSessionGuid(); // Bạn sẽ cần thay đổi phương thức này cho phù hợp

        //    // Tạo hóa đơn mới
        //    var hoaDon = new HoaDonViewModel
        //    {
        //        IDHD = Guid.NewGuid(),
        //        UserID = userId,
        //        NgayMua = DateTime.Now,
        //        TongTien = 0M,
        //        HinhThucThanhToan = 0, // 0 có thể đại diện cho "Chưa thanh toán" hoặc "Tiền mặt"
        //        ChiTietSanPhams = new List<HoaDonChiTietViewModel>()
        //    };

        //    // Lấy thông tin chi tiết các sản phẩm được chọn
        //    foreach (var productId in selectedProductIds)
        //    {
        //        var chiTietSanPham = _context.GioHangChiTiets
        //            .Where(x => x.CartItemID == productId)
        //            .Join(_context.SanPhamCTs, ghct => ghct.IDSPCT, spct => spct.IDSPCT, (ghct, spct) => new { ghct, spct })
        //            .Select(x => new HoaDonChiTietViewModel
        //            {
        //                IDHDCT = Guid.NewGuid(),
        //                IDSPCT = x.spct.IDSPCT,
        //                GiaSP = x.spct.GiaSP,
        //                SoLuong = x.ghct.SoLuong,
        //                // Lấy các thuộc tính khác tương ứng
        //                SanPhamChiTiet = x.spct
        //            }).FirstOrDefault();

        //        if (chiTietSanPham != null)
        //        {
        //            hoaDon.TongTien += chiTietSanPham.GiaSP * chiTietSanPham.SoLuong;
        //            hoaDon.ChiTietSanPhams.Add(chiTietSanPham);
        //        }
        //    }

        //    // Lưu thông tin vào session hoặc database tùy vào logic ứng dụng của bạn
        //    // Ví dụ lưu vào session:
        //    HttpContext.Session.SetString("HoaDon", JsonConvert.SerializeObject(hoaDon));

        //    // Chuyển hướng đến view hiển thị hóa đơn
        //    return RedirectToAction("Index", "HoaDonViewModel");
        //}

        //public ActionResult Index()
        //{
        //    // Lấy hoặc tạo UserID mới nếu cần
        //    var userID = GetUserOrCreateNewSessionGuid();

        //    // Kiểm tra xem đây có phải là user thực từ database hay không
        //    var isRealUser = _context.Users.Any(u => u.UserID == userID);

        //    // Lấy thông tin giỏ hàng từ session hoặc tạo mới nếu không tồn tại
        //    GioHang gioHang;
        //    List<GioHangChiTietViewModel> gioHangChiTietViewModelList;

        //    if (!isRealUser)
        //    {
        //        // Đối với người dùng không thực (sử dụng session)
        //        gioHang = HttpContext.Session.Get<GioHang>("GioHang") ?? new GioHang
        //        {
        //            CartID = Guid.NewGuid(),
        //            UserID = userID, // UserID ảo từ session
        //            NgayThem = DateTime.Now,
        //            TongSoLuong = 0
        //        };

        //        // Lấy thông tin chi tiết giỏ hàng từ session, hoặc tạo mới nếu session không có
        //        gioHangChiTietViewModelList = HttpContext.Session.Get<List<GioHangChiTietViewModel>>("GioHangChiTiet") ?? new List<GioHangChiTietViewModel>();
        //    }
        //    else
        //    {
        //        // Đối với người dùng thực (sử dụng database)
        //        gioHang = _context.GioHang.FirstOrDefault(gh => gh.UserID == userID);

        //        // Lấy thông tin chi tiết giỏ hàng từ database
        //        var chiTietList = _context.GioHangChiTiets
        //                                  .Where(ct => ct.CartID == gioHang.CartID)
        //                                  .ToList();

        //        gioHangChiTietViewModelList = chiTietList.Select(ct => new GioHangChiTietViewModel
        //        {
        //            CartItemID = ct.CartItemID,
        //            IDSPCT = ct.IDSPCT,
        //            CartID = ct.CartID,
        //            SoLuong = ct.SoLuong,
        //            TrangThai = ct.TrangThai,
        //            TenSanPham = _context.SanPhamCTs.FirstOrDefault(spct => spct.IDSPCT == ct.IDSPCT)?.TenSP
        //        }).ToList();
        //    }

        //    // Khởi tạo GioHangViewModel và gán các thuộc tính từ giỏ hàng và chi tiết giỏ hàng
        //    var gioHangViewModel = new GioHangViewModel
        //    {
        //        CartID = gioHang.CartID,
        //        UserID = gioHang.UserID,
        //        NgayThem = gioHang.NgayThem,
        //        TongSoLuong = gioHangChiTietViewModelList.Sum(item => item.SoLuong),
        //        GioHangChiTiets = gioHangChiTietViewModelList
        //    };

        //    // Truyền UserID vào ViewBag để sử dụng trong View
        //    ViewBag.SessionUserID = userID.ToString();

        //    // Truyền GioHangViewModel vào view
        //    return View(gioHangViewModel);
        //}

        //public ActionResult Index()
        //{
        //    var userId = GetOrCreateUserIdFromSession();

        //    // Truyền UserID vào ViewBag để sử dụng trong View
        //    ViewBag.SessionUserID = userId.ToString();

        //    var userID = GetUserOrCreateNewSessionGuid();

        //    // Lấy thông tin giỏ hàng của người dùng bằng UserID
        //    var gioHang = _context.GioHang.FirstOrDefault(gh => gh.UserID == userID);
        //    if (gioHang == null)
        //    {
        //        // Tạo mới giỏ hàng ảo và lưu vào session
        //        gioHang = new GioHang { UserID = userID, NgayThem = DateTime.Now };
        //        // Lưu đối tượng gioHang vào session thay vì database
        //        HttpContext.Session.Set("GioHang", gioHang);
        //    }

        //    // Sau đó, bạn có thể lấy lại đối tượng giỏ hàng từ session khi cần
        //    gioHang = HttpContext.Session.Get<GioHang>("GioHang");

        //    // Lấy thông tin chi tiết giỏ hàng liên quan đến giỏ hàng đã tìm được
        //    var chiTietList = _context.GioHangChiTiets
        //                              .Where(ct => ct.CartID == gioHang.CartID)
        //                              .ToList();

        //    // Tạo danh sách GioHangChiTietViewModel từ chiTietList
        //    var gioHangChiTietViewModelList = chiTietList.Select(ct => new GioHangChiTietViewModel
        //    {
        //        // Khởi tạo các thuộc tính từ ct, ví dụ:
        //        CartItemID = ct.CartItemID,
        //        IDSPCT = ct.IDSPCT,
        //        CartID = ct.CartID,
        //        //IDSP = ct.IDSP,
        //        SoLuong = ct.SoLuong,
        //        TrangThai = ct.TrangThai,
        //        // Bạn cần bổ sung thêm các thuộc tính khác nếu có
        //    }).ToList();

        //    // Khởi tạo GioHangViewModel và gán các thuộc tính từ giỏ hàng và chi tiết giỏ hàng
        //    var gioHangViewModel = new GioHangViewModel
        //    {
        //        CartID = gioHang.CartID,
        //        UserID = gioHang.UserID,
        //        NgayThem = gioHang.NgayThem,
        //        TongSoLuong = gioHang.TongSoLuong,
        //        // TongGiaTri = bạn có thể tính tổng giá trị ở đây nếu cần
        //        //GioHangChiTiets = gioHangChiTietViewModelList,

        //        GioHangChiTiets = _context.GioHangChiTiets
        //    .Where(ghct => ghct.GioHang.UserID == userID)
        //    .Join(_context.SanPhamCTs,  // Kết nối với bảng SanPhamChiTiets
        //        ghct => ghct.IDSPCT,         // Khóa ngoại từ GioHangChiTiet
        //        spct => spct.IDSPCT,         // Khóa chính của SanPhamChiTiet
        //        (ghct, spct) => new GioHangChiTietViewModel
        //        {
        //            CartItemID = ghct.CartItemID,
        //            CartID = ghct.CartID,
        //            IDSPCT = ghct.IDSPCT,
        //            SoLuong = ghct.SoLuong,
        //            TrangThai = ghct.TrangThai,
        //            TenSanPham = spct.TenSP  // Lấy trực tiếp tên từ bảng SanPhamChiTiets
        //            // Bổ sung thêm các thuộc tính khác nếu cần
        //        })
        //    .ToList()

        //    };

        //    // Truyền GioHangViewModel vào view
        //    return View(gioHangViewModel);
        //}

        //public ActionResult Index()
        //{
        //    // Giả sử GetUserIDFromSessionOrLogin() là phương thức để lấy UserID từ session hoặc thông tin đăng nhập
        //    var userID = GetUserIdFromClaimsPrincipal();

        //    // Lấy thông tin giỏ hàng của người dùng bằng UserID
        //    var gioHang = _context.GioHang.FirstOrDefault(gh => gh.UserID == userID);
        //    if (gioHang == null)
        //    {
        //        // Nếu không tìm thấy giỏ hàng, bạn có thể chuyển hướng người dùng hoặc hiển thị thông báo
        //        return View("Index"); // Tạo view "EmptyCart.cshtml" để thông báo rằng giỏ hàng trống
        //    }

        //    // Lấy thông tin chi tiết giỏ hàng liên quan đến giỏ hàng đã tìm được
        //    var chiTietList = _context.GioHangChiTiets
        //                              .Where(ct => ct.CartID == gioHang.CartID)
        //                              .ToList();

        //    // Tạo danh sách GioHangChiTietViewModel từ chiTietList
        //    var gioHangChiTietViewModelList = chiTietList.Select(ct => new GioHangChiTietViewModel
        //    {
        //        // Khởi tạo các thuộc tính từ ct, ví dụ:
        //        CartItemID = ct.CartItemID,
        //        IDSPCT = ct.IDSPCT,
        //        CartID = ct.CartID,
        //        //IDSP = ct.IDSP,
        //        SoLuong = ct.SoLuong,
        //        TrangThai = ct.TrangThai,
        //        // Bạn cần bổ sung thêm các thuộc tính khác nếu có
        //    }).ToList();

        //    // Khởi tạo GioHangViewModel và gán các thuộc tính từ giỏ hàng và chi tiết giỏ hàng
        //    var gioHangViewModel = new GioHangViewModel
        //    {
        //        CartID = gioHang.CartID,
        //        UserID = gioHang.UserID,
        //        NgayThem = gioHang.NgayThem,
        //        TongSoLuong = gioHang.TongSoLuong,
        //        // TongGiaTri = bạn có thể tính tổng giá trị ở đây nếu cần
        //        //GioHangChiTiets = gioHangChiTietViewModelList,

        //        GioHangChiTiets = _context.GioHangChiTiets
        //    .Where(ghct => ghct.GioHang.UserID == userID)
        //    .Join(_context.SanPhamCTs,  // Kết nối với bảng SanPhamChiTiets
        //        ghct => ghct.IDSPCT,         // Khóa ngoại từ GioHangChiTiet
        //        spct => spct.IDSPCT,         // Khóa chính của SanPhamChiTiet
        //        (ghct, spct) => new GioHangChiTietViewModel
        //        {
        //            CartItemID = ghct.CartItemID,
        //            CartID = ghct.CartID,
        //            IDSPCT = ghct.IDSPCT,
        //            SoLuong = ghct.SoLuong,
        //            TrangThai = ghct.TrangThai,
        //            TenSanPham = spct.TenSP  // Lấy trực tiếp tên từ bảng SanPhamChiTiets
        //            // Bổ sung thêm các thuộc tính khác nếu cần
        //        })
        //    .ToList()

        //    };

        //    // Truyền GioHangViewModel vào view
        //    return View(gioHangViewModel);
        //}




        // GET: GioHangViewModelController
        //public ActionResult Index()
        //{
        //    // Giả định rằng bạn đã lấy được userID từ session hoặc thông tin đăng nhập
        //    var userID = GetUserIdFromSessionOrLogin();

        //    // Lấy thông tin giỏ hàng của người dùng
        //    var gioHang = _context.GioHang.FirstOrDefault(gh => gh.UserID == userID);

        //    // Tạo ViewModel mới với giá trị mặc định
        //    var gioHangViewModel = new GioHangViewModel
        //    {
        //        GioHangChiTiets = new List<GioHangChiTietViewModel>() // Khởi tạo sẵn để tránh null
        //    };

        //    if (gioHang != null)
        //    {
        //        // Ánh xạ thông tin từ giỏ hàng sang ViewModel
        //        gioHangViewModel.CartID = gioHang.CartID;
        //        gioHangViewModel.UserID = gioHang.UserID;
        //        gioHangViewModel.NgayThem = gioHang.NgayThem;
        //        // Bạn cần tính toán và gán giá trị cho TongSoLuong và TongGiaTri dựa vào thông tin trong chi tiết giỏ hàng

        //        // Lấy thông tin chi tiết giỏ hàng
        //        var chiTiets = _context.GioHangChiTiets
        //                            .Where(ct => ct.CartID == gioHang.CartID)
        //                            .ToList();

        //        // Ánh xạ từ chi tiết giỏ hàng sang chi tiết ViewModel
        //        gioHangViewModel.GioHangChiTiets = chiTiets.Select(ct => new GioHangChiTietViewModel
        //        {
        //            CartItemID = ct.CartItemID,
        //            CartID = ct.CartID,
        //            IDSP = ct.IDSP,
        //            SoLuong = ct.SoLuong,
        //            TrangThai = ct.TrangThai
        //        }).ToList();
        //    }
        //    var viewModelList = new List<GioHangViewModel> { gioHangViewModel };

        //    // Truyền ViewModel vào view
        //    return View(viewModelList);
        //}




        public IActionResult Delete(Guid id)
        {
            // Giả sử 'CartItems' là khóa chúng ta sử dụng để lưu danh sách CartItemID trong session.
            var gioHangSession = HttpContext.Session.GetString("GioHang");
            if (!string.IsNullOrEmpty(gioHangSession))
            {
                // Deserialize giỏ hàng từ chuỗi JSON
                var gioHang = JsonConvert.DeserializeObject<GioHangSession>(gioHangSession);

                // Tìm chi tiết giỏ hàng cần xóa
                var chiTiet = gioHang.ChiTiets.FirstOrDefault(ct => ct.CartItemID == id);
                if (chiTiet != null)
                {
                    // Xóa chi tiết giỏ hàng khỏi danh sách
                    gioHang.ChiTiets.Remove(chiTiet);

                    // Cập nhật lại session
                    var gioHangSessionUpdated = JsonConvert.SerializeObject(gioHang);
                    HttpContext.Session.SetString("GioHang", gioHangSessionUpdated);
                }
            }

            // Chuyển hướng người dùng trở lại trang index của sản phẩm.
            _repos.DeleteObj(id);
            return RedirectToAction("SanPhamIndex", "SanPham");
        }

        // GET: GioHangViewModelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GioHangViewModelController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GioHangViewModelController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GioHangViewModelController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GioHangViewModelController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GioHangViewModelController/Delete/5

    }


    

}
