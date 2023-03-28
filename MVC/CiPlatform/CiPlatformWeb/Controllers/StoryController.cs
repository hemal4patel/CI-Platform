﻿using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CiPlatformWeb.Controllers
{
    public class StoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IStoryList _storyList;

        public StoryController (ApplicationDbContext db, IStoryList storyList)
        {
            _db = db;
            _storyList = storyList;
        }

        //GET
        public IActionResult StoryListing ()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            }


            var vm = new StoryListingViewModel();
            vm.CountryList = _db.Countries.ToList();
            vm.ThemeList = _db.MissionThemes.ToList();
            vm.SkillList = _db.Skills.ToList();

            return View(vm);
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> StoryListing (string searchText, int? countryId, string? cityId, string? themeId, string? skillId, int? pageNo, int? pagesize)
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
                SqlCommand command = new SqlCommand("spFilterStory", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@countryId", SqlDbType.VarChar).Value = countryId != null ? countryId : null;
                command.Parameters.Add("@cityId", SqlDbType.VarChar).Value = cityId != null ? cityId : null;
                command.Parameters.Add("@themeId", SqlDbType.VarChar).Value = themeId != null ? themeId : null;
                command.Parameters.Add("@skillId", SqlDbType.VarChar).Value = skillId != null ? skillId : null;
                command.Parameters.Add("@searchText", SqlDbType.VarChar).Value = searchText;
                command.Parameters.Add("@pageSize", SqlDbType.Int).Value = pagesize;
                command.Parameters.Add("@pageNo", SqlDbType.Int).Value = pageNo;
                SqlDataReader reader = command.ExecuteReader();

                List<long> StoryIds = new List<long>();
                while (reader.Read())
                {
                    long totalRecords = reader.GetInt32("TotalCards");
                    ViewBag.totalRecords = totalRecords;
                }
                reader.NextResult();

                while (reader.Read())
                {
                    long storyId = reader.GetInt64("story_id");
                    StoryIds.Add(storyId);
                }

                var vm = new StoryListingViewModel();
                vm.StoryList = _storyList.GetStories(StoryIds);

                return PartialView("_StoryListing", vm);
            }
        }


        public IActionResult ShareStory ()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            }
            var userId = Convert.ToInt64(ViewBag.UserId);

            var vm = new ShareStoryViewModel();
            vm.MissionTitles = _storyList.GetMissions(userId);

            return View(vm);
        }


        public IActionResult GetStory (long missionId)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            }
            var Id = Convert.ToInt64(ViewBag.UserId);
            var userId = (long) Id;

            var story = _storyList.GetDraftedStory(missionId, userId);

            return Json(story);
        }

        [HttpPost]
        public IActionResult SaveStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            }
            var Id = Convert.ToInt64(ViewBag.UserId);
            var userId = (long) Id;


            var story = _storyList.GetDraftedStory(viewmodel.MissionId, userId);
            if (story != null)
            {
                _storyList.UpdateDraftedStory(viewmodel, story);

                if (viewmodel.VideoUrl != null)
                {
                    _storyList.UpdateStoryUrls(story.StoryId, viewmodel.VideoUrl);
                }

                if (viewmodel.Images != null)
                {
                    _storyList.UpdateStoryImages(story.StoryId, viewmodel.Images);
                }

                return Ok(new { icon = "warning", message = "Story saved as draft!!!", published = 0 });
            }
            else
            {
                if (_storyList.CheckPublishedStory(viewmodel.MissionId, userId) == true)
                {
                    return Ok(new { icon = "error", message = "Only one story for a mission is allowed!!!", published = 1 });
                }
                else
                {
                    _storyList.AddNewStory(viewmodel, userId);
                    return Ok(new { icon = "success", message = "New story saved as draft!!!", published = 0 });
                }
            }
        }


        [HttpPost]
        public IActionResult SubmitStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            }
            var Id = Convert.ToInt64(ViewBag.UserId);
            var userId = (long) Id;


            var story = _storyList.GetDraftedStory(viewmodel.MissionId, userId);
            if (story != null)
            {
                _storyList.SubmitStory(viewmodel, story);

                if (viewmodel.VideoUrl != null)
                {
                    _storyList.UpdateStoryUrls(story.StoryId, viewmodel.VideoUrl);
                }

                if (viewmodel.Images != null)
                {
                    _storyList.UpdateStoryImages(story.StoryId, viewmodel.Images);
                }

                return Ok(new { icon = "success", message = "Story submitted!!! Approval is pending!!!" });
            }
            else
            {
                return Ok(new { icon = "error", message = "Only one story for a mission is allowed!!!" });
            }

        }


        public IActionResult StoryDetail (long MissionId, long UserId)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
            }
            var Id = Convert.ToInt64(ViewBag.UserId);
            var userId = (long) Id;

            Story storyDetail = _db.Stories.Where(s => s.MissionId == MissionId && s.UserId == UserId)
                                .Include(s => s.StoryMedia)
                                .Include(s => s.Mission)
                                .Include(s => s.User).FirstOrDefault();

            return View(storyDetail);
        }
    }
}
