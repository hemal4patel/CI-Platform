using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Text.Json;

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
            if (HttpContext.Session.GetString("Email") != "")
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                ViewBag.UserAvatar = _db.Users.Where(u => u.UserId == userId).Select(u => u.Avatar).FirstOrDefault();

                var vm = new DisplayMissionCards();

                vm.CountryList = _missionlist.GetCountryList();
                vm.ThemeList = _missionlist.GetThemeList();
                vm.SkillList = _missionlist.GetSkillList();
                vm.UserList = _missionlist.GetUserList(userId);

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult GetCitiesByCountry (int countryId)
        {
            var cities = _missionlist.GetCityList(countryId);
            return Json(cities);
        }


        //POST
        [HttpPost]
        public IActionResult PlatformLanding (DisplayMissionCards viewmodel)
        {
            if (HttpContext.Session.GetString("Email") != "")
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
                long UserId = Convert.ToInt64(ViewBag.UserId);
                ViewBag.UserAvatar = _db.Users.Where(u => u.UserId == UserId).Select(u => u.Avatar).FirstOrDefault();

                var vm = new DisplayMissionCards();

                var data = _missionlist.GetMissions(viewmodel, UserId);
                vm.MissionList = data.Item1;
                ViewBag.totalRecords = data.Item2;
                vm.UserList = _missionlist.GetUserList(UserId);

                return PartialView("_MissionDisplayPartial", vm);
            }
            return RedirectToAction("Index", "Home");
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
                //return Ok();
            }
            else
            {

                // Add the mission to favorites for the user
                var favoriteMission = new FavouriteMission { MissionId = missionId, UserId = userId };
                _db.FavouriteMissions.Add(favoriteMission);
                _db.SaveChanges();
            }


            return Ok();
        }


        //GET
        public IActionResult VolunteeringMission (long MissionId)
        {
            if (HttpContext.Session.GetString("Email") != "")
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                string userId = ViewBag.UserId;
                ViewBag.UserAvatar = _db.Users.Where(u => u.UserId == Convert.ToInt64(userId)).Select(u => u.Avatar).FirstOrDefault();

                var vm = new VolunteeringMissionViewModel();

                vm.MissionDetails = _missiondetail.GetMissionDetails(MissionId);
                vm.RelatedMissions = _missiondetail.GetRelatedMissions(MissionId);
                vm.RecentVolunteers = _missiondetail.GetRecentVolunteers(MissionId, Convert.ToInt64(userId));
                vm.ApprovedComments = _missiondetail.GetApprovedComments(MissionId);
                vm.UserList = _missionlist.GetUserList(Convert.ToInt64(userId));
                vm.MissionDocuments = _missiondetail.GetMissionDocuments(MissionId);

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
                alredyRated.UpdatedAt = DateTime.Now;
                _db.Update(alredyRated);
                _db.SaveChanges();
            }
            else
            {
                var newRating = new MissionRating { UserId = userId, MissionId = missionId, Rating = rating };
                _db.MissionRatings.Add(newRating);
                _db.SaveChangesAsync();
            }

            return Json(missionId);
        }

        [HttpPost]
        public IActionResult ApplyToMission (long missionId)
        {
            string Id = HttpContext.Session.GetString("UserId");
            var userId = Convert.ToInt64(Id);

            var mission = _missiondetail.GetMissionDetails(missionId);


            _missiondetail.ApplyToMission(missionId, userId);
            return Ok(new { icon = "success", message = "Successfully applied to the mission!!!" });
        }



        public IActionResult DisplayDocument (string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot/Upload/MissionDocuments", fileName);

            var fileStream = new FileStream(filePath, FileMode.Open);

            return File(fileStream, "application/pdf");
        }


        [HttpPost]
        public IActionResult PostComment (string comment, long missionId)
        {
            string Id = HttpContext.Session.GetString("UserId");
            var userId = Convert.ToInt64(Id);

            if (comment != null)
            {
                _missiondetail.AddComment(missionId, userId, comment);
                return Ok(new { icon = "success", message = "Comment added!!" });
            }

            return Ok(new { icon = "error", message = "Failed!!" });
        }


        [HttpPost]
        public async Task<IActionResult> MissionInvite (long ToUserId, long MissionId, long FromUserId)
        {
            var MissionInvite = _missionlist.HasAlreadyInvited(ToUserId, MissionId, FromUserId);

            if (MissionInvite != null)
            {
                MissionInvite.UpdatedAt = DateTime.Now;
                _db.Update(MissionInvite);
                _db.SaveChanges();
            }

            else
            {
                var missionInvite = new MissionInvite()
                {
                    FromUserId = FromUserId,
                    ToUserId = ToUserId,
                    MissionId = MissionId,
                };

                _db.MissionInvites.Add(missionInvite);
                await _db.SaveChangesAsync();
            }

            var MissionLink = Url.Action("VolunteeringMission", "Mission", new { MissionId = MissionId }, Request.Scheme);
            string link = MissionLink;

            await _missionlist.SendInvitationToCoWorker(ToUserId, FromUserId, link);

            return Json(new { success = true });
        }


        //[HttpPost]
        public IActionResult GetInvitations ()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(ViewBag.UserId);

            List<MissionInvite> missionInvites = _db.MissionInvites.Where(m => m.ToUserId == userId).Include(m => m.FromUser).Include(m => m.Mission).ToList();

            List<StoryInvite> storyInvites = _db.StoryInvites.Where(m => m.ToUserId == userId).Include(m => m.FromUser).Include(m => m.Story).ToList();

            var result = new { missionInvites = missionInvites, storyInvites = storyInvites };

            return Json(result);
        }

    }
}
