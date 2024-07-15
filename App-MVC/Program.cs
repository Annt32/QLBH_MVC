using App_Data_ClassLib.Models;
using App_Data_ClassLib.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<GioHangChiTiet>();


builder.Services.AddDbContext<SD18302_NET104Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YourConnectionStringName")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/Forbidden/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

// Thêm dịch vụ Session vào container
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Đặt thời gian tồn tại của session.
                                                    // Còn thời gian tồn tại mặc định không set là 20 phút- 12000s
    options.Cookie.HttpOnly = true; // Tăng cường bảo mật
    options.Cookie.IsEssential = true; // Đánh dấu cookie là thiết yếu cho ứng dụng
});
// 1 phiên làm việc cho tới khi timeout sẽ được tính từ khi có requets cuối cùng cho tới khi
// hết thời gian timeout mà không có tác vụ khác chèn vào, nếu có, bộ đếm thời gian sẽ reset.
// Dữ liệu được lưu vào web server
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Sử dụng Session middleware
app.UseSession(); // Đặt này trước UseRouting và UseEndpoints

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Create}");

app.Run();



//using App_Data_ClassLib.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

////builder.Services.AddAuthentication("YourCookieAuthScheme")
////    .AddCookie("YourCookieAuthScheme", options =>
////    {

////    });

//builder.Services.AddDbContext<SD18302_NET104Context>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("YourConnectionStringName")));

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/User/Login";
//        options.AccessDeniedPath = "/User/Forbidden/";
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//    });
//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=User}/{action=Create}");

//app.Run();
