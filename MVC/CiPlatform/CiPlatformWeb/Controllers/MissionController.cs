using CiPlatformWeb.Entities.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CiPlatformWeb.Controllers
{
    public class MissionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MissionController (ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult PlatformLanding ()
        {
            if(HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
            }

            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
            }

            var country = _db.Countries.ToList();
            var countryall = new SelectList(country, "CountryId", "Name");
            ViewBag.CountryList = countryall;

            var skill = _db.Skills.ToList();
            var skillall = new SelectList(skill, "SkillId", "SkillName");
            ViewBag.SkillList = skillall;

            var theme = _db.MissionThemes.ToList();
            var themeall = new SelectList(theme, "MissionThemeId", "Title");
            ViewBag.ThemeList = themeall;

            return View();
        }

        public IActionResult VolunteeringMission()
        {
            return View();
        }
    }
}
