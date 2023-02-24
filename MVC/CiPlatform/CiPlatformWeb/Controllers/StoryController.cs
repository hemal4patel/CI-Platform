using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Controllers
{
    public class StoryController : Controller
    {
        public IActionResult StoryListing ()
        {
            return View();
        }
    }
}
