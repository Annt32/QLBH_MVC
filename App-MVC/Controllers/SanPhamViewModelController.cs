using App_Data_ClassLib.Models;
using App_Data_ClassLib.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App_MVC.Controllers
{
    public class SanPhamViewModelController : Controller
    {
        private readonly SD18302_NET104Context _context;
        AllRepository<SanPhamChiTiet> _repos;
        DbSet<SanPhamChiTiet> _sanPhamCT;

        public SanPhamViewModelController(SD18302_NET104Context context)
        {
            _context = context;
            _sanPhamCT = _context.SanPhamCTs;
            _repos = new AllRepository<SanPhamChiTiet>(_sanPhamCT, _context);
        }
        public IActionResult SanPhamCTIndex(Guid id)
        {
            var getSanPham = _repos.GetByID(id);
            return View(getSanPham);
        }
        public IActionResult Details(Guid id)
        {
            // Tìm sản phẩm bằng ID
            var sanPham = _context.SanPhams.Find(id);
            if (sanPham == null)
            {
                return NotFound(); // 404
            }

            // Lấy danh sách chi tiết sản phẩm
            var sanPhamChiTiets = _context.SanPhamCTs.Where(s => s.IDSP == id).ToList();

            // Tạo ViewModel
            var viewModel = new SanPhamViewModel
            {
                SanPham = sanPham,
                SanPhamChiTiets = sanPhamChiTiets
            };

            // Trả về view kèm theo ViewModel
            return View(viewModel);
        }

        //private Guid GetUserIdFromClaimsPrincipal()
        //{
        //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");
        //    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        //    {
        //        return userId;
        //    }
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

        //    var isRealUser = _context.Users.Any(u => u.UserID == userID);
        //    if (isRealUser)
        //    {
        //        if (gioHang == null && isRealUser)
        //        {
        //           // Nếu giỏ hàng không tồn tại và đây là người dùng thực, tạo mới giỏ hàng
        //           gioHang = new GioHang
        //           {
        //               CartID = Guid.NewGuid(),
        //               UserID = userID,
        //               NgayThem = DateTime.Now,
        //               TongSoLuong = 0,
        //              // Khởi tạo các thuộc tính khác nếu cần
        //           };
        //            _context.GioHang.Add(gioHang);

        //            var sanPhamChiTiet = _context.SanPhamCTs
        //            .Include(s => s.SanPham)
        //            .FirstOrDefault(s => s.IDSPCT == id);

        //            string tenSanPham = sanPhamChiTiet.TenSP;

        //            if (sanPhamChiTiet == null)
        //            {
        //                return NotFound();
        //            }
        //            //Tìm xem sản phẩm đã có trong giỏ chưa
        //           var chiTiet = _context.GioHangChiTiets
        //                                  .FirstOrDefault(ct => ct.IDSPCT == id && ct.CartID == gioHang.CartID);

        //            if (chiTiet != null)
        //            {
        //               // Nếu đã có, tăng số lượng
        //                chiTiet.SoLuong++;
        //            }
        //            else
        //            {
        //                //Nếu chưa có, thêm mới
        //                _context.GioHangChiTiets.Add(new GioHangChiTiet
        //                {
        //                    CartItemID = Guid.NewGuid(),
        //                    CartID = gioHang.CartID,
        //                    IDSPCT = id,
        //                    SoLuong = 1,
        //                    TrangThai = 1, // Hoặc trạng thái phù hợp                  
        //                });
        //            }
        //            var chiTietViewModel = new GioHangChiTietViewModel
        //            {
        //                // Các thuộc tính khác...
        //                TenSanPham = sanPhamChiTiet.SanPham.TenSP
        //            };
        //            //Cập nhật tổng số lượng và hoặc tổng giá trị giỏ hàng nếu cần
        //            //gioHangViewModel.GioHangChiTiets.Add(chiTietViewModel);

        //            gioHang.TongSoLuong++;
        //            _context.SaveChanges();
        //            //Lưu tất cả các thay đổi vào cơ sở dữ liệu
        //        }

        //    }
        //    else
        //    {
        //        //Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
        //       var gioHangSession = HttpContext.Session.Get<GioHangSession>("GioHang") ?? new GioHangSession();

        //        //Tìm xem sản phẩm đã có trong giỏ hàng session chưa
        //       var chiTiet = gioHangSession.ChiTiets.FirstOrDefault(ct => ct.IDSPCT == id);
        //        if (chiTiet != null)
        //        {
        //           // Nếu đã có, tăng số lượng
        //            chiTiet.SoLuong++;
        //        }
        //        else
        //        {
        //            //Nếu chưa có, thêm mới vào giỏ hàng session
        //            gioHangSession.ChiTiets.Add(new GioHangChiTiet
        //            {
        //                CartItemID = Guid.NewGuid(),
        //                CartID = userID, // Mỗi session có một CartID duy nhất, ở đây sử dụng userID ảo
        //                IDSPCT = id,
        //                SoLuong = 1,
        //                TrangThai = 1, // Giả định mặc định là 1
        //            });
        //        }

        //        //Cập nhật lại session
        //        HttpContext.Session.Set("GioHang", gioHangSession);
        //        _context.SaveChanges();

        //       // Đưa ra thông báo hoặc xử lý tiếp
        //    }
        //    return RedirectToAction("SanPhamIndex", "SanPham"); // Chuyển hướng đến trang xem giỏ hàng
        //}



        //private Guid GetUserIdFromClaimsPrincipal()
        //{
        //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");
        //    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        //    {
        //        return userId;
        //    }
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

        //    var isRealUser = _context.Users.Any(u => u.UserID == userID);
        //    if (gioHang == null)
        //    {
        //        //Nếu giỏ hàng không tồn tại, tạo mới
        //       gioHang = new GioHang
        //       {
        //           CartID = Guid.NewGuid(),
        //           UserID = userID,
        //           NgayThem = DateTime.Now,
        //           TongSoLuong = 0,
        //          // Khởi tạo các thuộc tính khác nếu cần
        //       };
        //        _context.GioHang.Add(gioHang);
        //    }

        //    if (!isRealUser)
        //    {
        //        //Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
        //       var gioHangSession = HttpContext.Session.Get<GioHangSession>("GioHang") ?? new GioHangSession();

        //       // Tìm xem sản phẩm đã có trong giỏ hàng session chưa
        //       var chiTiet = gioHangSession.ChiTiets.FirstOrDefault(ct => ct.IDSPCT == id);
        //        if (chiTiet != null)
        //        {
        //           // Nếu đã có, tăng số lượng
        //            chiTiet.SoLuong++;
        //        }
        //        else
        //        {
        //           // Nếu chưa có, thêm mới vào giỏ hàng session
        //            gioHangSession.ChiTiets.Add(new GioHangChiTiet
        //            {
        //                CartItemID = Guid.NewGuid(),
        //                CartID = userID, // Mỗi session có một CartID duy nhất, ở đây sử dụng userID ảo
        //                IDSPCT = id,
        //                SoLuong = 1,
        //                TrangThai = 1, // Giả định mặc định là 1
        //            });
        //        }

        //        //Cập nhật lại session
        //        HttpContext.Session.Set("GioHang", gioHangSession);

        //        //Đưa ra thông báo hoặc xử lý tiếp
        //    }
        //    else
        //    {
        //        var sanPhamChiTiet = _context.SanPhamCTs
        //.Include(s => s.SanPham)
        //.FirstOrDefault(s => s.IDSPCT == id);

        //        string tenSanPham = sanPhamChiTiet.TenSP;

        //        if (sanPhamChiTiet == null)
        //        {
        //            return NotFound();
        //        }
        //       // Tìm xem sản phẩm đã có trong giỏ chưa
        //       var chiTiet = _context.GioHangChiTiets
        //                              .FirstOrDefault(ct => ct.IDSPCT == id && ct.CartID == gioHang.CartID);

        //        if (chiTiet != null)
        //        {
        //           // Nếu đã có, tăng số lượng
        //            chiTiet.SoLuong++;
        //        }
        //        else
        //        {
        //          //  Nếu chưa có, thêm mới
        //            _context.GioHangChiTiets.Add(new GioHangChiTiet
        //            {
        //                CartItemID = Guid.NewGuid(),
        //                CartID = gioHang.CartID,
        //                IDSPCT = id,
        //                SoLuong = 1,
        //                TrangThai = 1, // Hoặc trạng thái phù hợp                  
        //            });
        //        }
        //        var chiTietViewModel = new GioHangChiTietViewModel
        //        {
        //            // Các thuộc tính khác...
        //            TenSanPham = sanPhamChiTiet.SanPham.TenSP
        //        };
        //       // Cập nhật tổng số lượng và/ hoặc tổng giá trị giỏ hàng nếu cần
        //       // gioHangViewModel.GioHangChiTiets.Add(chiTietViewModel);

        //        gioHang.TongSoLuong++;

        //       // Lưu tất cả các thay đổi vào cơ sở dữ liệu
        //        _context.SaveChanges();
        //    }



        //    return RedirectToAction("SanPhamIndex", "SanPham"); // Chuyển hướng đến trang xem giỏ hàng
        //}
        private Guid GetUserIdFromClaimsPrincipal()
        {
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


        public IActionResult AddToCart(Guid id)
        {
            var product = _repos.GetByID(id);
            if (product == null)
            {
                return NotFound();
            }

            var userID = GetUserIdFromClaimsPrincipal();
            var isRealUser = _context.Users.Any(u => u.UserID == userID);

            if (!isRealUser)
            {
                // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
                var gioHangSession = HttpContext.Session.Get<GioHangSession>("GioHang") ?? new GioHangSession();

                // Kiểm tra xem giỏ hàng đã có CartID chưa, nếu chưa thì tạo mới
                if (gioHangSession.CartID == Guid.Empty)
                {
                    gioHangSession.CartID = Guid.NewGuid();
                }

                // lấy thông tin sản phẩm vào biến sanPhamChiTiet
                var sanPhamChiTiet = _context.SanPhamCTs
                    .Include(s => s.SanPham)
                    .FirstOrDefault(s => s.IDSPCT == id);

                // Kiểm tra sản phẩm có tồn tại không
                if (sanPhamChiTiet == null)
                {
                    return NotFound();
                }

                // Lấy tên sản phẩm
                string tenSanPham = sanPhamChiTiet.TenSP;

                // Lưu tên sản phẩm vào session cùng với ID sản phẩm chi tiết
                var tenSanPhamSession = HttpContext.Session.Get<Dictionary<Guid, string>>("TenSanPhamSession") ?? new Dictionary<Guid, string>();
                tenSanPhamSession[id] = tenSanPham;
                HttpContext.Session.Set("TenSanPhamSession", tenSanPhamSession);


                // Tìm xem sản phẩm đã có trong giỏ hàng session chưa
                var chiTiet = gioHangSession.ChiTiets.FirstOrDefault(ct => ct.IDSPCT == id);
                if (chiTiet != null)
                {
                    if (chiTiet.SoLuong >= sanPhamChiTiet.Soluong)
                    {
                        TempData["SanPhamError"] = " Hết hàng òi";
                        return RedirectToAction("SanPhamIndex", "SanPham");
                    }
                    else
                    {
                        chiTiet.SoLuong++;
                    }
                }
                else
                {
                    // Nếu chưa có, thêm mới vào giỏ hàng session
                    gioHangSession.ChiTiets.Add(new GioHangChiTiet
                    {
                        CartItemID = Guid.NewGuid(), 
                        CartID = gioHangSession.CartID, // Sử dụng CartID của giỏ hàng
                        IDSPCT = id, // ID sản phẩm chi tiết
                        SoLuong = 1, 
                        TrangThai = 1,
                    });
                }

                
                HttpContext.Session.Set("GioHang", gioHangSession);

            }
            else
            {
                // tìm giỏ hàng theo userID
                var gioHang = _context.GioHang.FirstOrDefault(gh => gh.UserID == userID);

                if (gioHang == null)
                {
                    // Nếu giỏ hàng không tồn tại, tạo mới
                    gioHang = new GioHang
                    {
                        CartID = Guid.NewGuid(),
                        UserID = userID,
                        NgayThem = DateTime.Now,
                        TongSoLuong = 0,
                    };
                    _context.GioHang.Add(gioHang);
                }

                // lấy thông tin sản phẩm vào biến sanPhamChiTiet
                var sanPhamChiTiet = _context.SanPhamCTs
        .Include(s => s.SanPham)
        .FirstOrDefault(s => s.IDSPCT == id);

                string tenSanPham = sanPhamChiTiet.TenSP;

                if (sanPhamChiTiet == null)
                {
                    return NotFound();
                }

                // Tìm xem sản phẩm đã có trong giỏ chưa
                var chiTiet = _context.GioHangChiTiets
                                       .FirstOrDefault(ct => ct.IDSPCT == id && ct.CartID == gioHang.CartID);

                if (chiTiet != null)
                {
                    if (chiTiet.SoLuong >= sanPhamChiTiet.Soluong)
                    {
                        TempData["SanPhamError"] = " Hết hàng òi";
                        return RedirectToAction("SanPhamIndex", "SanPham");
                    }
                    else
                    {
                        chiTiet.SoLuong++;
                    }
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    _context.GioHangChiTiets.Add(new GioHangChiTiet
                    {
                        CartItemID = Guid.NewGuid(),
                        CartID = gioHang.CartID,
                        IDSPCT = id,
                        SoLuong = 1,
                        TrangThai = 1, // Hoặc trạng thái phù hợp                  
                    });
                }
                

                //gioHang.TongSoLuong++;

                _context.SaveChanges();
            }

            return RedirectToAction("SanPhamIndex", "SanPham");
        }

    }

}
