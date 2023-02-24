using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Controllers
{
    public class MissionController : Controller
    {
        public IActionResult Index ()
        {
            return View();
        }

        public IActionResult VolunteeringMission()
        {
            return View();
        }
    }
}
