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
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using static CiPlatformWeb.Repositories.EnumStats;

namespace CiPlatformWeb.Controllers
{
    [Authorize(Roles = "user")]
    public class MissionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMissionList _missionlist;
        private readonly IMissionDetail _missiondetail;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly long userId;

        public MissionController (ApplicationDbContext db, IMissionList missionlist, IMissionDetail missiondetail, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _missionlist = missionlist;
            _missiondetail = missiondetail;

            _httpContextAccessor = httpContextAccessor;
            string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            if (token is not null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadJwtToken(token);
                var claims = decodedToken.Claims;
                var customClaimString = decodedToken.Claims.FirstOrDefault(c => c.Type == "CustomClaimForUser")?.Value;
                var customClaimValue = JsonSerializer.Deserialize<User>(customClaimString);
                userId = customClaimValue.UserId;
            }
        }





        //GRID LIST VIEW
        public IActionResult PlatformLanding ()
        {
            DisplayMissionCards vm = new DisplayMissionCards();

            vm.CountryList = _missionlist.GetCountryList();
            vm.ThemeList = _missionlist.GetThemeList();
            vm.SkillList = _missionlist.GetSkillList();
            vm.UserList = _missionlist.GetUserList(userId);

            return View(vm);
        }

        public IActionResult GetCitiesByCountry (int countryId)
        {
            List<City> cities = _missionlist.GetCityList(countryId);
            return Json(cities);
        }

        [HttpPost]
        public IActionResult PlatformLanding (MissionQueryParams viewmodel)
        {
            DisplayMissionCards vm = new DisplayMissionCards();

            var data = _missionlist.GetMissions(viewmodel, userId);
            vm.MissionList = data.Item1;
            vm.MissionCount = data.Item2;
            vm.UserList = _missionlist.GetUserList(userId);

            return PartialView("_MissionDisplayPartial", vm);
        }



        //FAVOURITES
        [HttpPost]
        public IActionResult AddToFavorites (int missionId)
        {
            _missionlist.AddToFavorites(missionId, userId);
            return Ok();
        }





        //VOLUNTEERING MISSION
        [AllowAnonymous]
        public IActionResult VolunteeringMission (long MissionId)
        {
            if (CheckSession())
            {
                if (_missiondetail.IsValidMissionId(MissionId))
                {
                    VolunteeringMissionViewModel vm = new VolunteeringMissionViewModel();

                    vm.MissionDetails = _missiondetail.GetMissionDetails(MissionId, userId);
                    vm.RelatedMissions = _missiondetail.GetRelatedMissions(MissionId, userId);
                    vm.UserList = _missionlist.GetUserList(Convert.ToInt64(userId));

                    return View(vm);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                HttpContext.Session.SetString("MissionId", MissionId.ToString());
                return RedirectToAction("Index", "Home");
            }
        }




        //COMMENTS
        public IActionResult GetComments (long missionId)
        {
            VolunteeringMissionViewModel vm = new VolunteeringMissionViewModel();

            vm.ApprovedComments = _missiondetail.GetComments(missionId);
            return PartialView("_commentsPartial", vm);
        }




        //RECENT VOLUNTEERS
        public IActionResult showRecentVounteers (int currVolPage, long missionId)
        {
            VolunteeringMissionViewModel vm = new VolunteeringMissionViewModel();

            var data = _missiondetail.GetRecentVolunteers(missionId, userId, currVolPage);
            vm.RecentVolunteers = data.recentVolunteers;
            ViewBag.volCount = data.count;

            return PartialView("_RecentVolunteers", vm);
        }



        //RATE MISSION
        [HttpPost]
        public IActionResult RateMission (int rating, long missionId)
        {
            _missionlist.RateMission(missionId, userId, rating);

            return Json(missionId);
        }


        //GET UPDATED RATINGS
        public IActionResult GetUpdatedRatings (long missionId)
        {
            (int ratings, int volunteers) data = _missiondetail.GetUpdatedRatings(missionId);
            return Json(data);
        }


        //APPLY TO MISSION
        [HttpPost]
        public IActionResult ApplyToMission (long missionId)
        {
            _missiondetail.ApplyToMission(missionId, userId);
            return Ok(new { icon = "success", message = "Successfully applied to the mission!!!" });

        }



        //DOCUMENTS DISPLAY
        public IActionResult DisplayDocument (string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot/Upload/MissionDocuments", fileName);
            if (Path.GetExtension(filePath).ToLower() == ".pdf")
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fileStream, "application/pdf");
            }
            else
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", fileName);
            }
        }




        //COMMENT ADD
        [HttpPost]
        public IActionResult PostComment (string comment, long missionId)
        {
            if (comment != null)
            {
                _missiondetail.AddComment(missionId, userId, comment);
                return Ok(new { icon = "success", message = "Comment added!!" });
            }

            return Ok(new { icon = "error", message = "Failed!!" });
        }




        //INVITE TO MISSION
        [HttpPost]
        public async Task<IActionResult> MissionInvite (long ToUserId, long MissionId, long FromUserId)
        {
            _missiondetail.InviteToMission(FromUserId, ToUserId, MissionId);

            string MissionLink = Url.Action("VolunteeringMission", "Mission", new { MissionId = MissionId }, Request.Scheme);
            string link = MissionLink;
            await _missionlist.SendInvitationToCoWorker(ToUserId, FromUserId, link);

            return Json(new { success = true });
        }




        //GET INVITATIONS
        public IActionResult GetInvitations ()
        {
            List<MissionInvite> missionInvites = _missionlist.GetMissionInvites(userId);
            List<StoryInvite> storyInvites = _missionlist.GetStoryInvites(userId);
            var result = new { missionInvites = missionInvites, storyInvites = storyInvites };

            return Json(result);
        }




        //PAGE NOT FOUND
        [AllowAnonymous]
        public IActionResult PageNotFound ()
        {
            return View();
        }



        //CHECK SESSION
        public bool CheckSession ()
        {
            return HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
