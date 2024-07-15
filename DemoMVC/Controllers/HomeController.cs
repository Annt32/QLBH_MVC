using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DemoMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Crivacy()
        {
            return View();
        }
        public IActionResult Adu()
        {
            return View(); // trả về view có thên là Adu ( cùng trên với action)
            //return Content("Index");
            //return NoContent();
            //return NotFound();
        }
        public IActionResult DieuHuong()
        {
            // thực hiện hành vi nào đó trước đi
            return RedirectToAction("Index"); // thực hiện điều hướng didens Action nào đó
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}