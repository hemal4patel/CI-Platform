using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IUserProfile _userProfile;

        public UserController (ILogger<UserController> logger, ApplicationDbContext db, IUserProfile userProfile)
        {
            _logger = logger;
            _db = db;
            _userProfile = userProfile;
        }

        //GET
        public IActionResult UserProfile ()
        {
            if (HttpContext.Session.GetString("Email") != "")
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");

                long userId = Convert.ToInt64(ViewBag.UserId);

                var vm = new UserProfileViewModel();
                vm = _userProfile.GetUserDetails(userId);
                vm.CountryList = _userProfile.GetCountryList();
                vm.SkillList = _userProfile.GetSkillList();

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult UserProfile (UserProfileViewModel viewmodel)
        {
            return Ok();
        }


        public IActionResult GetCitiesByCountry (int countryId)
        {
            var vm = new UserProfileViewModel();
            vm.CityList = _userProfile.GetCityList(countryId);
            return Json(vm.CityList);
        }

        [HttpPost]
        public IActionResult ChangePassword (string oldPassoword, string newPassword, string confirmPassword)
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");

            long userId = Convert.ToInt64(ViewBag.UserId);

            var user = _userProfile.CheckPassword(userId, oldPassoword);

            if (user != null)
            {
                _userProfile.UpdatePassword(user, newPassword);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
