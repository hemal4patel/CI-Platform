using CiPlatformWeb.Entities.DataModels;
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
            }
            var userId = Convert.ToInt64(ViewBag.UserId);

            var vm = new ShareStoryViewModel();
            vm.MissionTitles = _storyList.GetMissions();

            return View(vm);
        }


        public IActionResult GetStory (long missionId)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
            }
            var Id = Convert.ToInt64(ViewBag.UserId);
            var userId = (long) Id;

            var story = _storyList.GetDraftedStory(missionId, userId);

            return Json(story);
        }

        [HttpPost]
        public IActionResult SaveStory (ShareStoryViewModel viewmodel, long storyId)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
            }
            var Id = Convert.ToInt64(ViewBag.UserId);
            var userId = (long) Id;

            if(storyId != 0)
            {

                var draftStory = _storyList.GetDraftedStory(storyId);

                draftStory.Title = viewmodel.StoryTitle;
                draftStory.Description = viewmodel.StoryDescription;
                draftStory.Status = "DRAFT";
                draftStory.UpdatedAt = DateTime.Now;

                _db.Update(draftStory);
                _db.SaveChanges();

                return Ok(new { message = "Story saved as draft!!!"});
            }
            else
            {
                if (_storyList.CheckPublishedStory(viewmodel.MissionId, userId) == true)
                {
                    return Ok();
                }
                else
                {
                    var newStory = new Story()
                    {
                        MissionId = viewmodel.MissionId,
                        UserId = userId,
                        Title = viewmodel.StoryTitle,
                        Description = viewmodel.StoryDescription,
                        Status = "DRAFT",
                        CreatedAt = DateTime.Now,
                    };

                    _db.Stories.Add(newStory);
                    _db.SaveChanges();

                    return Ok();
                }            
               
            }

            return Ok();

        }
    }
}
