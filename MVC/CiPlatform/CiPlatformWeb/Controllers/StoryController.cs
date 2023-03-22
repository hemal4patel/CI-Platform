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

            //var UserId = Convert.ToInt64(ViewBag.UserId);

            //var response = _db.Stories.FromSql($"exec spFilterStory @searchText={searchText}, @countryId={countryId}, @cityId={cityId}, @themeId={themeId}, @skillId={skillId}");

            //var items = await response.ToListAsync();

            //var StoryIds = items.Select(m => m.StoryId).ToList();

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
    }
}
