using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Controllers
{
    public class MissionController : Controller
    {
        public IActionResult Index ()
        {
            return View();
        }

        public IActionResult ListView ()
        {
            return View();
        }
    }
}
