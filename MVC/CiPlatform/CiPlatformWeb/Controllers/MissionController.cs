using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;

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
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");

                long userId = Convert.ToInt64(ViewBag.UserId);

                var vm = new DisplayMissionCards();

                vm.CountryList = _missionlist.GetCountryList();
                vm.ThemeList = _missionlist.GetThemeList();
                vm.SkillList = _missionlist.GetSkillList();
                vm.UserList = _missionlist.GetUserList(userId);

                //vm.missionInvites = _db.MissionInvites.Where(m => m.ToUserId == userId).Include(m => m.Mission).ToList();
                //vm.storyInvites = _db.StoryInvites.Where(s => s.ToUserId == userId).Include(s => s.Story).ToList();

                //vm.combinedList = new List<baseClass>();
                //vm.combinedList.AddRange(vm.missionInvites);
                //vm.combinedList.AddRange(vm.storyInvites);

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
        public async Task<IActionResult> PlatformLanding (string searchText, int? countryId, string? cityId, string? themeId, string? skillId, int? sortCase, string? userId, int? pageNo, int? pagesize)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            }

            IConfigurationRoot _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("spFilterSortSearchPagination", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@countryId", SqlDbType.VarChar).Value = countryId != null ? countryId : null;
                command.Parameters.Add("@cityId", SqlDbType.VarChar).Value = cityId != null ? cityId : null;
                command.Parameters.Add("@themeId", SqlDbType.VarChar).Value = themeId != null ? themeId : null;
                command.Parameters.Add("@skillId", SqlDbType.VarChar).Value = skillId != null ? skillId : null;
                command.Parameters.Add("@searchText", SqlDbType.VarChar).Value = searchText;
                command.Parameters.Add("@sortCase", SqlDbType.VarChar).Value = sortCase;
                command.Parameters.Add("@userId", SqlDbType.VarChar).Value = userId;
                command.Parameters.Add("@pageSize", SqlDbType.Int).Value = pagesize;
                command.Parameters.Add("@pageNo", SqlDbType.Int).Value = pageNo;
                SqlDataReader reader = command.ExecuteReader();

                List<long> MissionIds = new List<long>();
                while (reader.Read())
                {
                    long totalRecords = reader.GetInt32("TotalRecords");
                    ViewBag.totalRecords = totalRecords;
                }
                reader.NextResult();

                while (reader.Read())
                {
                    long missionId = reader.GetInt64("mission_id");
                    MissionIds.Add(missionId);
                }


                var vm = new DisplayMissionCards();

                var UserId = Convert.ToInt64(ViewBag.UserId);
                vm.MissionList = _missionlist.GetMissions(MissionIds);
                vm.UserList = _missionlist.GetUserList(UserId);

                return PartialView("_MissionDisplayPartial", vm);
            }
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
            //if (HttpContext.Session.GetString("Email") != null)
            //{
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            //}
            //var userId = Convert.ToInt64(ViewBag.UserId);
            string userId = ViewBag.UserId;

            var vm = new VolunteeringMissionViewModel();

            vm.MissionDetails = _missiondetail.GetMissionDetails(MissionId);
            vm.RelatedMissions = _missiondetail.GetRelatedMissions(MissionId);
            vm.RecentVolunteers = _missiondetail.GetRecentVolunteers(MissionId, Convert.ToInt64(userId));
            vm.ApprovedComments = _missiondetail.GetApprovedComments(MissionId);
            vm.UserList = _missionlist.GetUserList(Convert.ToInt64(userId));
            vm.MissionDocuments = _missiondetail.GetMissionDocuments(MissionId);

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
                //var newComment = new Comment { UserId = userId, MissionId = missionId, CommentText = comment };
                //_db.Comments.Add(newComment);
                //_db.SaveChanges();
                _missiondetail.AddComment(missionId, userId, comment);
                return Ok(new { icon = "success", message = "Comment added!!" });
            }

            return Ok(new { icon = "error", message = "Failed!!" });
        }


        [HttpPost]
        public async Task<IActionResult> MissionInvite (long ToUserId, long MissionId, long FromUserId)
        {

            if (_db.MissionInvites.Any(m => m.MissionId == MissionId && m.ToUserId == ToUserId && m.FromUserId == FromUserId))
            {
                var MissionInvite = _db.MissionInvites.Where(m => m.MissionId == MissionId && m.ToUserId == ToUserId && m.FromUserId == FromUserId).FirstOrDefault();
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




    }
}
