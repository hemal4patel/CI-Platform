using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

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
        public IActionResult PlatformLanding (MissionQueryParams viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);
                ViewBag.UserId = userId;

                var vm = new DisplayMissionCards();

                var data = _missionlist.GetMissions(viewmodel, userId);
                vm.MissionList = data.Item1;
                vm.MissionCount = data.Item2;
                vm.UserList = _missionlist.GetUserList(userId);

                return PartialView("_MissionDisplayPartial", vm);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public IActionResult AddToFavorites (int missionId)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

            _missionlist.AddToFavorites(missionId, userId);
            return Ok();
        }


        //GET
        public IActionResult VolunteeringMission (long MissionId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);
                ViewBag.UserId = userId;

                var vm = new VolunteeringMissionViewModel();

                vm.MissionDetails = _missiondetail.GetMissionDetails(MissionId, userId);
                vm.RelatedMissions = _missiondetail.GetRelatedMissions(MissionId, userId);
                vm.UserList = _missionlist.GetUserList(Convert.ToInt64(userId));
                vm.ApprovedComments = _missiondetail.GetApprovedComments(MissionId);

                return View(vm);
            }
            else
            {
                HttpContext.Session.SetString("MissionId", MissionId.ToString());
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult showRecentVounteers(int currVolPage, long missionId)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

            var vm = new VolunteeringMissionViewModel();

            var data = _missiondetail.GetRecentVolunteers(missionId, userId, currVolPage);
            vm.RecentVolunteers = data.recentVolunteers;
            ViewBag.volCount = data.count;

            return PartialView("_RecentVolunteers", vm);
        }

        [HttpPost]
        public IActionResult RateMission (int rating, long missionId)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

            _missionlist.RateMission(missionId, userId, rating);

            return Json(missionId);
        }

        [HttpPost]
        public IActionResult ApplyToMission (long missionId)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

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
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

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
                _missiondetail.ReInviteToMission(MissionInvite);
            }
            else
            {
                _missiondetail.InviteToMission(FromUserId, ToUserId, MissionId);
            }
            var MissionLink = Url.Action("VolunteeringMission", "Mission", new { MissionId = MissionId }, Request.Scheme);
            string link = MissionLink;
            await _missionlist.SendInvitationToCoWorker(ToUserId, FromUserId, link);

            return Json(new { success = true });
        }


        //[HttpPost]
        public IActionResult GetInvitations ()
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);
            List<MissionInvite> missionInvites = _missionlist.GetMissionInvites(userId);
            List<StoryInvite> storyInvites = _missionlist.GetStoryInvites(userId);
            var result = new { missionInvites = missionInvites, storyInvites = storyInvites };

            return Json(result);
        }

    }
}
