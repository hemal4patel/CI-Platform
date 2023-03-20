using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CiPlatformWeb.Controllers
{
    public class MissionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMissionList _missionlist;
        private readonly IMissionDetail _missiondetail;

        public MissionController (ApplicationDbContext db, IMissionList missionlist, IMissionDetail missiondetail)
        {
            _db = db;
            _missionlist = missionlist;
            _missiondetail = missiondetail;
        }

        //GET
        public IActionResult PlatformLanding ()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
            }

            var userId = Convert.ToInt64(ViewBag.UserId);

            var country = _db.Countries.ToList();
            var countryall = new SelectList(country, "CountryId", "Name");
            ViewBag.CountryList = countryall;

            var skill = _db.Skills.ToList();
            var skillall = new SelectList(skill, "SkillId", "SkillName");
            ViewBag.SkillList = skillall;

            var theme = _db.MissionThemes.ToList();
            var themeall = new SelectList(theme, "MissionThemeId", "Title");
            ViewBag.ThemeList = themeall;

            var vm = new VolunteeringMissionViewModel();
            vm.UserList = _missionlist.UserList(userId);

            return View(vm);
        }

        public IActionResult GetCitiesByCountry (int countryId)
        {
            var cities = _db.Cities.Where(c => c.CountryId == countryId).ToList();
            return Json(cities);
        }


        //POST
        [HttpPost]
        public async Task<IActionResult> PlatformLanding (string searchText, int? countryId, string? cityId, string? themeId, string? skillId, int? sortCase, string? userId)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
            }

            var response = _db.Missions.FromSql($"exec spFilterSortSearchPagination @searchText={searchText}, @countryId={countryId}, @cityId={cityId}, @themeId={themeId}, @skillId={skillId}, @sortCase = {sortCase}, @userId = {userId}");

            var items = await response.ToListAsync();

            var MissionIds = items.Select(m => m.MissionId).ToList();

            var vm = new DisplayMissionCards();
            vm.MissionList = _missionlist.GetMissions(MissionIds);

            return PartialView("_MissionDisplayPartial", vm);
        }

        [HttpPost]
        public IActionResult AddToFavorites (int missionId)
        {
            string Id = HttpContext.Session.GetString("UserId");
            var userId = Convert.ToInt64(Id);

            // Check if the mission is already in favorites for the user
            if (_db.FavouriteMissions.Any(fm => fm.MissionId == missionId && fm.UserId == userId))
            {
                // Mission is already in favorites, return an error message or redirect back to the mission page
                var FavoriteMissionId = _db.FavouriteMissions.Where(fm => fm.MissionId == missionId && fm.UserId == userId).FirstOrDefault();
                _db.FavouriteMissions.Remove(FavoriteMissionId);
                _db.SaveChanges();
                return RedirectToAction("PlatformLanding", "Mission");
            }

            // Add the mission to favorites for the user
            var favoriteMission = new FavouriteMission { MissionId = missionId, UserId = userId };
            _db.FavouriteMissions.Add(favoriteMission);
            _db.SaveChanges();

            return RedirectToAction("PlatformLanding", "Mission");
        }


        //GET
        public IActionResult VolunteeringMission (long MissionId)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
            }
                var userId = Convert.ToInt64(ViewBag.UserId);

            var vm = new VolunteeringMissionViewModel();

            vm.MissionDetails = _missiondetail.GetMissionDetails(MissionId);
            vm.RelatedMissions = _missiondetail.GetRelatedMissions(MissionId);
            vm.RecentVolunteers = _missiondetail.GetRecentVolunteers(MissionId, userId);
            vm.ApprovedComments = _missiondetail.GetApprovedComments(MissionId);
            vm.UserList = _missionlist.UserList(userId);

            return View(vm);
        }


        [HttpPost]
        public IActionResult RateMission (int rating, long missionId)
        {
            string Id = HttpContext.Session.GetString("UserId");
            var userId = Convert.ToInt64(Id);

            var alredyRated = _db.MissionRatings.SingleOrDefault(mr => mr.MissionId == missionId && mr.UserId == userId);

            if (alredyRated != null)
            {
                alredyRated.Rating = rating;
                _db.SaveChanges();
            }
            else
            {
                var newRating = new MissionRating { UserId = userId, MissionId = missionId, Rating = rating };
                _db.MissionRatings.Add(newRating);
                _db.SaveChanges();
            }

            return Json(missionId);
        }


        [HttpPost]
        public IActionResult PostComment (string comment, long missionId)
        {
            string Id = HttpContext.Session.GetString("UserId");
            var userId = Convert.ToInt64(Id);

            if (comment != null)
            {
                var newComment = new Comment { UserId = userId, MissionId = missionId, CommentText = comment };
                _db.Comments.Add(newComment);
                _db.SaveChanges();
            }

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> MissionInvite (long ToUserId, long MissionId, long FromUserId, DisplayMissionCards viewmodel)
        {
            var missionInvite = new MissionInvite()
            {
                FromUserId = FromUserId,
                ToUserId = ToUserId,
                MissionId = MissionId,
            };

            _db.MissionInvites.Add(missionInvite);
            await _db.SaveChangesAsync();

            var MissionLink = Url.Action("VolunteeringMission", "Mission", new { MissionId = MissionId }, Request.Scheme);
            viewmodel.link = MissionLink;

            await _missionlist.SendInvitationToCoWorker(ToUserId, FromUserId, viewmodel);

            return Json(new { success = true });
        }




    }
}
