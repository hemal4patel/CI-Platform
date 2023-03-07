using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CiPlatformWeb.Controllers
{
    public class MissionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMissionList _missionlist;

        public MissionController (ApplicationDbContext db, IMissionList missionlist)
        {
            _db = db;
            _missionlist = missionlist;
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

            //IEnumerable<Mission> MissionAll = _missionlist.GetMissions();
            //ViewBag.MissionList = MissionAll;

            var list = from m in _db.Missions
                       join cn in _db.Countries on m.CountryId equals cn.CountryId
                       join ct in _db.Cities on m.CityId equals ct.CityId
                       join t in _db.MissionThemes on m.ThemeId equals t.MissionThemeId
                       join g in _db.GoalMissions on m.MissionId equals g.MissionId
                       select new MissionListingModel
                       {
                           MissionId = m.MissionId,
                           MissionTitle = m.Title,
                           ShortDescription = m.ShortDescription,
                           MissionType = m.MissionType,
                           OrganizationName = m.OrganizationName,
                           StartDate = m.StartDate,
                           EndDate = m.EndDate,
                           ThemeName = t.Title,
                           CityName = ct.Name,
                           CountryId = cn.CountryId,
                           GoalObjectiveText = g.GoalObjectiveText
                       };

            return View(list);
        }

        public IActionResult GetCitiesByCountry (int countryId)
        {
            var cities = _db.Cities.Where(c => c.CountryId == countryId).ToList();
            return Json(cities);
        }


        public IActionResult VolunteeringMission()
        {
            return View();
        }
    }
}
