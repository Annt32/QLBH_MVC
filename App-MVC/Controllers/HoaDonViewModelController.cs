using App_Data_ClassLib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;


namespace App_MVC.Controllers
{
    public class HoaDonViewModelController : Controller
    {
        private readonly SD18302_NET104Context _context;

        // GET: HoaDonViewModelController
        public HoaDonViewModelController(SD18302_NET104Context context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
           

            return View();
        }
        private Guid GetUserIdFromClaimsPrincipal()
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

        public Guid GetOrCreateUserIdFromSession()
        {
            var sessionUserId = HttpContext.Session.GetString("SessionUserID");
            if (!string.IsNullOrEmpty(sessionUserId))
            {
                return Guid.Parse(sessionUserId);
            }
            else
            {
                var newUserId = Guid.NewGuid();
                HttpContext.Session.SetString("SessionUserID", newUserId.ToString());
                return newUserId;
            }
        }
        // GET: HoaDonViewModelController/ShowHoaDon
        //public ActionResult ShowHoaDon()
        //{
        //    var model = new HoaDonViewModel();


        //    // Lấy Dictionary tên sản phẩm từ session
        //    var tenSanPhamDict = HttpContext.Session.Get<Dictionary<Guid, string>>("TenSanPhamDict");
        //    if (tenSanPhamDict != null)
        //    {
        //        model.TenSanPhamDict = tenSanPhamDict;
        //    }

        //    var userID = GetOrCreateUserIdFromSession();

        //    ViewBag.SessionUserID = userID.ToString();

        //    HoaDonViewModel hoaDon;

        //    // Lấy thông tin hóa đơn từ session

        //    string hoaDonSession = HttpContext.Session.GetString("HoaDon");
        //    if (!string.IsNullOrEmpty(hoaDonSession))
        //    {
        //        hoaDon = JsonConvert.DeserializeObject<HoaDonViewModel>(hoaDonSession);
        //    }
        //    else
        //    {
        //        // Nếu không có thông tin trong session, có thể redirect tới trang khác
        //        // hoặc hiển thị thông báo lỗi, tùy thuộc vào logic ứng dụng của bạn
        //        return View(model);
        //    }

        //    // Truyền thông tin hóa đơn vào view để hiển thị
        //    return View(hoaDon);
        //}

        //public async Task<ActionResult> ShowHoaDonAsync()
        //{
        //    // Lấy UserID từ session, đây là logic đã được bạn viết từ trước.
        //    var userID = GetOrCreateUserIdFromSession();
        //    ViewBag.SessionUserID = userID.ToString();
        //    var user = await _context.Users.SingleOrDefaultAsync(u => u.UserID == userID);

        //    // Tạo ViewModel mới để hiển thị thông tin trên form
        //    var hoaDonViewModel = new HoaDonViewModel();

        //    // Nếu người dùng tồn tại, điền thông tin vào ViewModel
        //    if (user != null)
        //    {
        //        hoaDonViewModel.TenKhachHang = user.Ten;
        //        hoaDonViewModel.Email = user.Email;
        //        hoaDonViewModel.SoDienThoai = user.SoDienThoai;
        //        hoaDonViewModel.DiaChi = user.DiaChi;
        //        hoaDonViewModel.NamSinh = user.NamSinh;
        //        // ... điền các trường thông tin khác mà bạn cần vào ViewModel
        //    }
        //    // Lấy thông tin hóa đơn từ session.
        //    string hoaDonSession = HttpContext.Session.GetString("HoaDon");
        //    if (!string.IsNullOrEmpty(hoaDonSession))
        //    {
        //        var hoaDon = JsonConvert.DeserializeObject<HoaDonViewModel>(hoaDonSession);

        //        // Lấy Dictionary tên sản phẩm từ session để hiển thị tên sản phẩm.
        //        var tenSanPhamDict = HttpContext.Session.Get<Dictionary<Guid, string>>("TenSanPhamDict");
        //        if (tenSanPhamDict != null)
        //        {
        //            hoaDon.TenSanPhamDict = tenSanPhamDict;
        //        }

        //        // Truyền thông tin hóa đơn vào view để hiển thị.
        //        return View(hoaDon);
        //    }
        //    else
        //    {
        //        // Nếu không có thông tin trong session, có thể redirect tới trang khác
        //        // hoặc hiển thị thông báo lỗi, tùy thuộc vào logic ứng dụng của bạn.
        //        // Tạo một instance mới của HoaDonViewModel để tránh truyền một model null vào View.
        //        var emptyModel = new HoaDonViewModel();
        //        return View(emptyModel);
        //    }
        //    return View(hoaDonViewModel); // Truyền ViewModel này đến view

        //}

        public async Task<ActionResult> ShowHoaDonAsync()
        {
            var khuyenMaiList = await _context.KhuyenMai.ToListAsync();

            // Lọc và chỉ lấy những khuyến mãi còn hiệu lực
            var validKhuyenMaiList = khuyenMaiList.Where(km =>
                DateTime.Now >= km.NgayBatDau && DateTime.Now <= km.NgayKetThuc).ToList();

            // Tạo SelectList chỉ với những khuyến mãi còn hiệu lực
            ViewBag.KhuyenMaiSelectList = new SelectList(validKhuyenMaiList, "IDKM", "MoTa");

            // Lấy UserID từ session
            var userID = GetUserIdFromClaimsPrincipal();
            ViewBag.SessionUserID = userID.ToString();

            // Tạo ViewModel mới
            var hoaDonViewModel = new HoaDonViewModel();


            // Lấy thông tin hóa đơn từ session và cập nhật ViewModel
            string hoaDonSession = HttpContext.Session.GetString("HoaDon");
            if (!string.IsNullOrEmpty(hoaDonSession))
            {
                hoaDonViewModel = JsonConvert.DeserializeObject<HoaDonViewModel>(hoaDonSession);


                if (hoaDonViewModel.ChiTietSanPhams != null && hoaDonViewModel.ChiTietSanPhams.Any())
                {
                    foreach (var spct in hoaDonViewModel.ChiTietSanPhams)
                    {
                        // Lấy giá sản phẩm chi tiết hiện tại từ cơ sở dữ liệu theo IDSPCT
                        var sanPhamChiTietHienTai = await _context.SanPhamCTs.FindAsync(spct.IDSPCT);
                        if (sanPhamChiTietHienTai != null)
                        {
                            // Cập nhật giá trong ViewModel với giá mới từ cơ sở dữ liệu
                            spct.GiaSP = sanPhamChiTietHienTai.GiaSP;

                            decimal tongTienMoi = hoaDonViewModel.ChiTietSanPhams.Sum(spct => spct.GiaSP * spct.SoLuong);

                            // Cập nhật tổng tiền trong hoaDonViewModel
                            hoaDonViewModel.TongTien = tongTienMoi;
                        }
                    }

                    var updatedHoaDonSession = JsonConvert.SerializeObject(hoaDonViewModel);
                    HttpContext.Session.SetString("HoaDon", updatedHoaDonSession);
                    await HttpContext.Session.CommitAsync();
                }

                // // Lấy Dictionary tên sản phẩm từ session để hiển thị tên sản phẩm
                //var tenSanPhamDict = HttpContext.Session.Get<Dictionary<Guid, string>>("TenSanPhamDict");
                //if (tenSanPhamDict != null)
                //{
                //    hoaDonViewModel.TenSanPhamDict = tenSanPhamDict;
                //}
            }
            else
            {
                
                
            }

            // Truy vấn thông tin người dùng từ database
            var user = await _context.Users.FindAsync(userID);
            if (user != null)
            {
                // Nếu người dùng tồn tại, điền thông tin vào ViewModel
                hoaDonViewModel.ten = user.Ten;
                hoaDonViewModel.Email = user.Email;
                hoaDonViewModel.SoDienThoai = user.SoDienThoai;
                hoaDonViewModel.DiaChi = user.DiaChi;
                hoaDonViewModel.NamSinh = user.NamSinh; // Format ngày tháng
            }
            // Truyền ViewModel vào view
            return View(hoaDonViewModel);
        }

        [HttpPost]

        //public async Task<IActionResult> ThanhToan(HoaDonViewModel model)
        //{
        //    var hoaDonViewModel = HttpContext.Session.Get<HoaDonViewModel>("HoaDon");
        //    var khuyenMai = await _context.KhuyenMai.SingleOrDefaultAsync(km => km.IDKM == model.IDKM);

        //    // Lấy UserID từ Claims nếu có, nếu không lấy từ session.
        //    var userID = User.Identity.IsAuthenticated ? GetUserIdFromClaimsPrincipal() : Guid.Parse(HttpContext.Session.GetString("SessionUserID") ?? Guid.NewGuid().ToString());

        //    // Tạo hoá đơn mới từ HoaDonViewModel
        //    HoaDon hoaDon = new HoaDon
        //    {
        //        IDHD = Guid.NewGuid(),
        //        UserID = userID, // UserID lấy từ session hoặc từ người dùng đã đăng nhập
        //        NgayMua = DateTime.Now,
        //        TongTien = hoaDonViewModel.TongTien,
        //        HinhThucThanhToan = model.HinhThucThanhToan
        //    };

        //    // Áp dụng khuyến mãi (nếu có)
        //    if (khuyenMai != null)
        //    {
        //        hoaDon.TongTien -= hoaDon.TongTien * khuyenMai.Giam / 100;
        //    }

        //    _context.HoaDons.Add(hoaDon);

        //    // Lưu chi tiết hóa đơn
        //    foreach (var chiTietVM in hoaDonViewModel.ChiTietSanPhams)
        //    {
        //        var chiTietHoaDon = new HoaDonChiTiet
        //        {
        //            IDHDCT = Guid.NewGuid(),
        //            IDHD = hoaDon.IDHD,
        //            IDSPCT = chiTietVM.IDSPCT,
        //            IDKM = model.IDKM.HasValue ? model.IDKM.Value : Guid.Empty,
        //            GiaSP = chiTietVM.GiaSP,
        //            SoLuong = chiTietVM.SoLuong
        //        };

        //        _context.HoaDonChiTiet.Add(chiTietHoaDon);
        //    }

        //    await _context.SaveChangesAsync();

        //    // Xóa thông tin hóa đơn khỏi session sau khi lưu
        //    HttpContext.Session.Remove("HoaDon");

        //    // Redirect đến trang xác nhận thanh toán hoặc trang hiển thị thông tin hóa đơn
        //    return RedirectToAction("ShowHoaDon", new { id = hoaDon.IDHD });
        //}


        public async Task<IActionResult> ThanhToan(HoaDonViewModel model)
        {
            var hoaDonViewModelcu = HttpContext.Session.Get<HoaDonViewModel>("HoaDon");

            decimal tongTienMoi = hoaDonViewModelcu.ChiTietSanPhams.Sum(spct => spct.GiaSP * spct.SoLuong);

            // Cập nhật tổng tiền trong hoaDonViewModel
            hoaDonViewModelcu.TongTien = tongTienMoi;
            var updatedHoaDonSession = JsonConvert.SerializeObject(hoaDonViewModelcu);
            HttpContext.Session.SetString("HoaDon", updatedHoaDonSession);

            var hoaDonViewModel = HttpContext.Session.Get<HoaDonViewModel>("HoaDon");

            var khuyenMai = await _context.KhuyenMai.SingleOrDefaultAsync(km => km.IDKM == model.IDKM);

            // Khai báo hoaDon ở đây để có thể sử dụng sau này
            HoaDon hoaDon;
            var userID = GetUserIdFromClaimsPrincipal();
            var existingUser = await _context.Users.FindAsync(userID);

            // Kiểm tra xem existingUser có tồn tại hay không
            if (existingUser == null)
            {
                // Nếu không tồn tại, tạo một người dùng mới với thông tin từ model
                var hasher = new PasswordHasher<User>();
                var newUser = new User
                {
                    UserID = userID, // Tạo một ID mới
                    Ten = model.ten ?? "Tên mặc định",
                    Email = model.Email,
                    Username = "guest_" + DateTime.UtcNow.Ticks.ToString(),
                    Password = hasher.HashPassword(null, "password_tạm_thời"), // Sử dụng hàm để hash mật khẩu
                    SoDienThoai = model.SoDienThoai,
                    DiaChi = model.DiaChi,
                    NamSinh = new DateTime(1900, 1, 1),
                    Role = "Khách"
                };

                _context.Users.Add(newUser);
                _context.SaveChangesAsync();
            }           

            if (khuyenMai != null)
            {
                var tongTienDaGiam = hoaDonViewModel.TongTien - (hoaDonViewModel.TongTien * khuyenMai.Giam / 100);
                hoaDon = new HoaDon
                {
                    IDHD = Guid.NewGuid(),
                    UserID = GetUserIdFromClaimsPrincipal(),
                    NgayMua = DateTime.Now,
                    TongTien = tongTienDaGiam,
                    HinhThucThanhToan = 1 
                };
            }
            else
            {
                hoaDon = new HoaDon
                {
                    IDHD = Guid.NewGuid(),
                    UserID = GetUserIdFromClaimsPrincipal(),
                    NgayMua = DateTime.Now,
                    TongTien = hoaDonViewModel.TongTien,
                    HinhThucThanhToan = 1 
                };
            }
        
            // Lưu thông tin hoá đơn
            _context.HoaDons.Add(hoaDon);

            // Tạo và lưu chi tiết hóa đơn
            foreach (var chiTietVM in hoaDonViewModel.ChiTietSanPhams)
            {
                var chiTietHoaDon = new HoaDonChiTiet
                {
                    IDHDCT = Guid.NewGuid(),
                    IDHD = hoaDon.IDHD, // Sử dụng IDHD từ đối tượng hoaDon đã được tạo ở trên
                    IDSPCT = chiTietVM.IDSPCT,
                    IDKM = model.IDKM.HasValue ? model.IDKM.Value : Guid.Empty, // Kiểm tra xem IDKM có giá trị hay không
                    GiaSP = chiTietVM.GiaSP,
                    SoLuong = chiTietVM.SoLuong
                };

                _context.HoaDonChiTiet.Add(chiTietHoaDon);
                var sanPhamChiTiet = await _context.SanPhamCTs
                        .SingleOrDefaultAsync(spct => spct.IDSPCT == chiTietVM.IDSPCT);

                if (sanPhamChiTiet != null)
                {
                    if (sanPhamChiTiet.Soluong >= chiTietVM.SoLuong) // Kiểm tra nếu còn đủ số lượng để bán
                    {
                        sanPhamChiTiet.Soluong -= chiTietVM.SoLuong; 
                        _context.SanPhamCTs.Update(sanPhamChiTiet);
                    }
                    else
                    {
                        
                    }
                }
            }

            await _context.SaveChangesAsync();

            //var userID = GetUserIdFromClaimsPrincipal(); // Hoặc phương thức bạn dùng để lấy UserID

            var gioHang = await _context.GioHang
                .Include(gh => gh.GioHangChiTiets)
                .FirstOrDefaultAsync(gh => gh.UserID == userID);

            if (gioHang != null)
            {
                // Duyệt qua từng sản phẩm đã mua trong hóa đơn
                foreach (var chiTietVM in hoaDonViewModel.ChiTietSanPhams)
                {
                    // Tìm sản phẩm trong chi tiết giỏ hàng
                    var chiTietGioHang = gioHang.GioHangChiTiets
                        .FirstOrDefault(ctgh => ctgh.IDSPCT == chiTietVM.IDSPCT);

                    if (chiTietGioHang != null)
                    {
                        // Nếu sản phẩm tồn tại trong giỏ hàng, giảm số lượng hoặc xóa nó đi
                        if (chiTietGioHang.SoLuong > chiTietVM.SoLuong)
                        {
                            chiTietGioHang.SoLuong -= chiTietVM.SoLuong; // Giảm số lượng
                        }
                        else
                        {
                            _context.GioHangChiTiets.Remove(chiTietGioHang); // Xóa sản phẩm khỏi giỏ hàng
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            // Xóa thông tin hóa đơn khỏi session sau khi lưu
            HttpContext.Session.Remove("HoaDon");
            return RedirectToAction("Index", "ThongKeViewModel");
        }




        //public async Task<IActionResult> ThanhToan(HoaDonViewModel model)
        //{
        //    // Giả định rằng bạn đã lưu HoaDonViewModel vào session trước đó
        //    var hoaDonViewModel = HttpContext.Session.Get<HoaDonViewModel>("HoaDon");

        //    var khuyenMai = await _context.KhuyenMai
        //.SingleOrDefaultAsync(km => km.IDKM == model.IDKM);
        //    // Tạo hóa đơn mới
        //    if (khuyenMai != null)
        //    {
        //        // Tính tổng tiền và áp dụng khuyến mãi

        //        var tongTienDaGiam = hoaDonViewModel.TongTien - (hoaDonViewModel.TongTien * khuyenMai.Giam / 100);

        //        // Lưu hoá đơn và chi tiết hoá đơn...
        //        var hoaDon = new HoaDon
        //        {
        //            IDHD = Guid.NewGuid(),
        //            UserID = GetUserIdFromClaimsPrincipal(),
        //            NgayMua = DateTime.Now,
        //            TongTien = tongTienDaGiam,
        //            HinhThucThanhToan = 1 // Giả định này nên dựa vào logic hoặc dữ liệu đầu vào từ người dùng
        //        };

        //        //model.TongTien = hoaDon.TongTien - (hoaDon.TongTien * khuyenMai.Giam / 100);

        //        _context.HoaDons.Add(hoaDon);


        //        _context.HoaDons.Add(hoaDon);

        //        // Tạo các chi tiết hóa đơn từ ViewModel
        //        foreach (var chiTietVM in hoaDonViewModel.ChiTietSanPhams)
        //        {
        //            var chiTietHoaDon = new HoaDonChiTiet
        //            {
        //                IDHDCT = Guid.NewGuid(),
        //                IDHD = hoaDon.IDHD,
        //                IDSPCT = chiTietVM.IDSPCT,
        //                IDKM = model.IDKM,
        //                GiaSP = chiTietVM.GiaSP,
        //                SoLuong = chiTietVM.SoLuong
        //            };

        //            _context.HoaDonChiTiet.Add(chiTietHoaDon);
        //        }

        //        // Lưu tất cả thay đổi vào cơ sở dữ liệu
        //        // Xóa thông tin hóa đơn khỏi session sau khi lưu
        //        await _context.SaveChangesAsync();
        //        HttpContext.Session.Remove("HoaDon");
        //        // Các logic lưu cơ sở dữ liệu ở đây
        //    }


        //    // Redirect tới trang xác nhận thanh toán thành công hoặc hiển thị thông tin hóa đơn
        //    return RedirectToAction("ShowHoaDon", "HoaDonViewModel");
        //}


    }
}
