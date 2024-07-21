using App_Data_ClassLib.Models;
using App_Data_ClassLib.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json;
using System;

namespace App_MVC.Controllers
{
    public class UserController : Controller
    {
        SD18302_NET104Context _context;
        AllRepository<User> _repo;
        DbSet<User> _users;


        public UserController()
        {
            // Khởi tạo DBcontext
            _context = new SD18302_NET104Context();
            // Khởi tạo repository với 2 tham số là Dbset và DBContext
            _users = _context.Users;
            _repo = new AllRepository<User>( _users,_context);

            CheckAndAddAdminAccount();
        }
        // Lấy ra tất cả danh sách User
        private void CheckAndAddAdminAccount()
        {
            var adminUser = _repo.GetAll().FirstOrDefault(u => u.Role == "Admin");
            if (adminUser == null)
            {
                var admin = new User
                {
                    UserID = Guid.NewGuid(),
                    Ten = "Admin",
                    Email = "admin@example.com",
                    Username = "admin",
                    Password = "admin123", // Lưu ý: Bạn nên mã hóa mật khẩu
                    SoDienThoai = "0123456789",
                    DiaChi = "Admin Address",
                    NamSinh = DateTime.Now.AddYears(-30), // Giả định tuổi admin là 30
                    Role = "Admin"
                };
                _context.Add(admin);
                _context.SaveChanges();
            }
        }

        public IActionResult Index(string name) // tham số name để tìm kiếm
        {
            

            var sessionData = HttpContext.Session.GetString("user");
            if (sessionData == null)
            {
                ViewData["account"] = "Bạn chưa đăng nhập";
            } else
            {
                ViewData["account"] = $"Chào mừng {sessionData} đến với unfinished square integer";

            }

            var userData = _repo.GetAll();
            if (string.IsNullOrEmpty(name))
            {
                return View(userData);
            }
            else
            {
                var searchData = _repo.GetAll().Where(x => x.Ten.Contains(name)).ToList(); // Tìm theo tên           
                ViewData["count"] = searchData.Count;
                ViewBag.Count = searchData.Count;
                if (searchData.Count == 0) // Nếu ko tìm thấy 
                {
                    return View(userData);
                }
                else return View(searchData); // có tìm thấy
            }           
        }

        private Guid GetUserIdFromClaimsPrincipal()
        {
            //var role = HttpContext.Session.Get<User>("role");
            var role = HttpContext.Session.GetString("role");
            if (role == "Admin")
            {
                return Guid.NewGuid();
            }
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

        // Thêm data
        public IActionResult Create() // Action để mở form điền thông tin user
        {
            return View();
        }
        // Action để thực hiện thêm vào DB
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            var existingUser = _repo.FindByUsername(user.Username, user.UserID);
            var checkEmail = _repo.CheckEmail(user.Email, user.UserID);
            var CheckSdt = _repo.CheckSdt(user.SoDienThoai, user.UserID);
            var CheckName = _repo.CheckName(user.Ten, user.UserID);

            var role = HttpContext.Session.GetString("role");

            // Nếu CheckEmail trả về một đối tượng, có nghĩa là email đã tồn tại
            if (CheckName != null)
            {
                ModelState.AddModelError("Ten", "Tên đã được sử dụng.");
                return View(user);

            }
            if (user.NamSinh > DateTime.Now)
            {
                ModelState.AddModelError("NamSinh", "Ngày sinh không được lớn hơn ngày hiện tại.");
                return View(user);
            }
            if (checkEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(user);

            }
            if (existingUser != null)
            {
                // Nếu user đã tồn tại, trả về view kèm thông báo lỗi
                ModelState.AddModelError("Username", "Tên tài khoản đã được sử dụng.");
                return View(user);
            }
            if (CheckSdt != null)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã được sử dụng.");
                return View(user);

            }
            if (user.Role == "...")
            {
                ModelState.AddModelError("Role", "Chọn phân quyền");
                return View(user);
            }
            
            var newUser = new User
            {
                UserID = GetUserIdFromClaimsPrincipal(), // Tạo một ID mới
                Ten = user.Ten,
                Email = user.Email,
                Username = user.Username,
                Password = user.Password,
                SoDienThoai = user.SoDienThoai,
                DiaChi = user.DiaChi,
                NamSinh = user.NamSinh,
                Role = user.Role,
            };
            _context.Users.Add(newUser);

            var gioHang = await _context.GioHang.FirstOrDefaultAsync(gh => gh.UserID == newUser.UserID);
            if (gioHang == null && role == "Admin")
            {
                gioHang = new GioHang
                {
                    CartID = Guid.NewGuid(), // Sử dụng UserID mới làm CartID
                    UserID = newUser.UserID, // Đặt UserID mới
                    NgayThem = DateTime.Now,
                    TongSoLuong = 0 
                };

                _context.GioHang.Add(gioHang);
            } else if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    CartID = Guid.NewGuid(), // Sử dụng UserID mới làm CartID
                    UserID = GetUserIdFromClaimsPrincipal(), // Đặt UserID mới
                    NgayThem = DateTime.Now, 
                    TongSoLuong = 0 
                };

                _context.GioHang.Add(gioHang);
            }

            var gioHangSession = HttpContext.Session.Get<GioHangSession>("GioHang");
            // Kiểm tra xem giỏ hàng session có tồn tại không
            if (gioHangSession != null && gioHangSession.ChiTiets.Any())
            {               
                foreach (var chiTiet in gioHangSession.ChiTiets)
                {
                    var gioHangChiTiet = new GioHangChiTiet
                    {
                        CartItemID = Guid.NewGuid(), // Tạo mới CartItemID
                        CartID = gioHang.CartID, // Sử dụng CartID của giỏ hàng
                        IDSPCT = chiTiet.IDSPCT,
                        SoLuong = chiTiet.SoLuong,
                        TrangThai = chiTiet.TrangThai,
                    };

                    gioHang.TongSoLuong += chiTiet.SoLuong; // Cập nhật tổng số lượng trong giỏ hàng

                    _context.GioHangChiTiets.Add(gioHangChiTiet);
                }

                var userIdAo = Guid.NewGuid();
                var CartIdAo = Guid.NewGuid();
                HttpContext.Session.Remove("GioHang");

                var gioHangMoi = new GioHangSession()
                {
                    CartID = Guid.NewGuid()                                          
                };

                // Lưu giỏ hàng mới vào session
                HttpContext.Session.Set<GioHangSession>("GioHang", gioHangMoi);
                // Cập nhật session để dùng UserID mới
                HttpContext.Session.SetString("UserID", userIdAo.ToString());
                HttpContext.Session.SetString("CartID", CartIdAo.ToString());                
            }

            await _context.SaveChangesAsync();

            // Redirect dựa trên vai trò của người dùng
            if (user.Role == "Admin")
            {
                TempData["DangKy"] = " Tạo mới thành công (Admin)";
                
                return RedirectToAction("Index");
            }
            else 
            {
                TempData["DangKy"] = " Đăng ký thành công";
                
                return RedirectToAction("Login");
            }
            //return RedirectToAction("Login");
        }


        // Sửa
        public IActionResult Edit(Guid id) // Form load ra đối tượng cần sửa
        {

            // Lấy ra đối tượng cần sửa
            var updateUser = _repo.GetByID(id);
            return View(updateUser);
        }
        // Sửa
        [HttpPost]

        public IActionResult Edit(User user) // Action này thực hiện thay đổi, khi cần thì trỏ tới nó
        {
            var existingUser = _repo.FindByUsername(user.Username, user.UserID);
            var checkEmail = _repo.CheckEmail(user.Email, user.UserID);
            var CheckSdt = _repo.CheckSdt(user.SoDienThoai, user.UserID);
            var CheckName = _repo.CheckName(user.Ten, user.UserID);

            // Nếu CheckEmail trả về một đối tượng, có nghĩa là email đã tồn tại
            if (CheckName != null)
            {
                ModelState.AddModelError("Ten", "Tên đã được sử dụng.");
                return View(user);

            }
            if (user.NamSinh > DateTime.Now)
            {
                ModelState.AddModelError("NamSinh", "Ngày sinh không được lớn hơn ngày hiện tại.");
                return View(user);
            }
            if (checkEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(user);

            }
            if (existingUser != null)
            {
                // Nếu user đã tồn tại, trả về view kèm thông báo lỗi
                ModelState.AddModelError("Username", "Tên tài khoản đã được sử dụng.");
                return View(user);
            }
            if (CheckSdt != null)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã được sử dụng.");
                return View(user);

            }

            _repo.UpdateObj(user);
            return RedirectToAction("Index");
        }
        // Xóa
        public IActionResult Delete(Guid id)
        {
            // lấy ra đối tượng cần xóa
            var deleteUser = _repo.GetByID(id);

            var jsonData = JsonConvert.SerializeObject(deleteUser); // ép kiểu sang json 
            HttpContext.Session.SetString("deleted", jsonData); // Cho dữ liệu vào session

            _repo.DeleteObj(id);
            return RedirectToAction("Index");
        }
      
        // Thông tin Details
        public IActionResult Details(Guid id)
        {
            var getUser = _repo.GetByID(id);
            return View(getUser);
        }


        //private async Task<Guid?> ValidateUserCredentialsAndGetUserId(string username, string password)
        //{
        //    // Tìm kiếm người dùng dựa trên thông tin đăng nhập
        //    var user = await _context.Users
        //                             .FirstOrDefaultAsync(u => u.Username == username && u.Password == password); // Ghi chú: Mật khẩu nên được hash và so sánh với giá trị đã hash trong cơ sở dữ liệu

        //    // Nếu người dùng tồn tại và thông tin đăng nhập đúng, trả về UserID
        //    if (user != null)
        //    {
        //        return user.UserID;
        //    }

        //    // Nếu thông tin đăng nhập không đúng, trả về null
        //    return null;
        //}


        private async Task<IActionResult> ChuyenDoiGioHangSessionSangThuc(Guid userId)
        {
            // Lấy giỏ hàng hiện tại của người dùng dựa trên userID
            var gioHang = await _context.GioHang
                                        .Include(gh => gh.GioHangChiTiets)
                                        .FirstOrDefaultAsync(gh => gh.UserID == userId);

            var gioHangSession = HttpContext.Session.Get<GioHangSession>("GioHang");
           
            // Kiểm tra xem giỏ hàng session có tồn tại và có chứa chi tiết không
            if (gioHangSession != null && gioHangSession.ChiTiets.Any())
            {
                // Lặp qua từng chi tiết trong giỏ hàng session và thêm vào giỏ hàng thực
                foreach (var chiTietSession in gioHangSession.ChiTiets)
                {
                    var sanPhamChitiet = await _context.SanPhamCTs
                       .Include(s => s.SanPham)
                       .FirstOrDefaultAsync(s => s.IDSPCT == chiTietSession.IDSPCT);

                    if (sanPhamChitiet == null)
                    {
                        TempData["SanPhamError"] = "Sản phẩm không tồn tại trong cơ sở dữ liệu!";
                        continue;
                    }

                    var gioHangChiTiet = gioHang.GioHangChiTiets
                                                .FirstOrDefault(ghct => ghct.IDSPCT == chiTietSession.IDSPCT);

                    // Tính tổng số lượng sản phẩm trong cả giỏ hàng ảo và thực
                    int tongSoLuong = (gioHangChiTiet?.SoLuong ?? 0) + chiTietSession.SoLuong;

                    // Kiểm tra tổng số lượng so với số lượng sản phẩm chi tiết có sẵn
                    if (tongSoLuong > sanPhamChitiet.Soluong)
                    {
                        TempData["SanPhamError"] = "Số lượng sản phẩm đã lớn hơn số lượng có sẵn. Vui lòng xem lại trong giỏ hàng của tài khoản của bạn ";
                        continue;
                        // Không thêm hoặc cập nhật sản phẩm này nữa
                    }

                    if (gioHangChiTiet != null)
                    {
                        // Cập nhật số lượng trong giỏ hàng thực
                        gioHangChiTiet.SoLuong = tongSoLuong;
                    }
                    else
                    {
                        // Thêm mới vào giỏ hàng thực
                        _context.GioHangChiTiets.Add(new GioHangChiTiet
                        {
                            CartItemID = Guid.NewGuid(),
                            CartID = gioHang.CartID,
                            IDSPCT = chiTietSession.IDSPCT,
                            SoLuong = chiTietSession.SoLuong,
                            TrangThai = chiTietSession.TrangThai,
                        });
                    }

                    // Lưu thay đổi sau mỗi lần thêm mới hoặc cập nhật
                    await _context.SaveChangesAsync();
                }

                
            }

            return View();
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _repo.GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role), // Sử dụng chuỗi Role đã lưu trong database
                new Claim("UserID", user.UserID.ToString()) // Lưu UserID vào Claim
                };

                if (user != null) // Kiểm tra nếu người dùng đăng nhập thành công
                {

                    // Chuyển đổi giỏ hàng từ session sang thực nếu có
                    await ChuyenDoiGioHangSessionSangThuc(user.UserID);

                    HttpContext.Session.SetString("user", username);
                    HttpContext.Session.SetString("role", user.Role); // Lưu Role vào session
                    HttpContext.Session.SetString("UserID", user.UserID.ToString()); // Lưu UserID vào session
                    // Redirect người dùng sau khi đăng nhập và chuyển đổi giỏ hàng thành công
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Nếu thông tin đăng nhập không chính xác, hiển thị lỗi
                    ViewBag.LoginError = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.LoginError = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return View("Login");
            }


            //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            //lưu trữ thông tin đăng nhập vào Session

            // Lưu thông tin đăng nhập vào Session




        }


        public IActionResult Logout()
        {
            // Xóa thông tin người dùng khỏi session
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("UserID");

            // Bạn cũng có thể sử dụng Clear() để xóa tất cả session, nếu không còn giữ lại thông tin nào khác
            // HttpContext.Session.Clear();

            return RedirectToAction("Login", "User");
        }


    }
}
