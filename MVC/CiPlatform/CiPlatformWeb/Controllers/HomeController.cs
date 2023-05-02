using CiPlatformWeb.Auth;
using CiPlatformWeb.Entities.Auth;
using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Models;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CiPlatformWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEmailGeneration _emailGeneration;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public HomeController (ILogger<HomeController> logger, IUserRepository userRepository, IEmailGeneration emailGeneration, ApplicationDbContext db, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _emailGeneration = emailGeneration;
            _db = db;
            _configuration = configuration;
        }



        //REGISTER
        public IActionResult Registration ()
        {
            RegistrationValidation vm = new();
            vm.banners = _userRepository.GetBanners();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Registration (RegistrationValidation obj)
        {
            obj.banners = _userRepository.GetBanners();
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj.Email);
                if (user == null)
                {
                    _userRepository.RegisterUser(obj);
                    TempData["success"] = "Registered!!!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "User " + Messages.exists;
                    return View(obj);
                }
            }
            return View(obj);
        }



        //LOGIN
        public IActionResult Index ()
        {
            HttpContext.Session.Remove("Token");

            LoginValidation vm = new();
            vm.banners = _userRepository.GetBanners();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Index (LoginValidation obj)
        {
            obj.banners = _userRepository.GetBanners();
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj.Email);
                if (user == null)
                {
                    TempData["error"] = "User does not exist!!!";
                    return View(obj);
                }
                else
                {
                    if (_userRepository.verifyPassword(obj.Password, user.Password))
                    {
                        if (user.DeletedAt == null && user.Status == 1)
                        {
                            var jwtSettings = _configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();
                            var token = JwtTokenHelper.GenerateToken(jwtSettings, user);
                            HttpContext.Session.SetString("Token", token);

                            if (user.Role == "user")
                            {
                                TempData["success"] = "Logged In!!!";
                                var missionId = HttpContext.Session.GetString("MissionId");
                                var storyMissionId = HttpContext.Session.GetString("StoryMissionId");
                                var storyUserId = HttpContext.Session.GetString("StoryUserId");
                                if (!string.IsNullOrEmpty(missionId))
                                {
                                    HttpContext.Session.Remove("MissionId");
                                    return RedirectToAction("VolunteeringMission", "Mission", new { missionId = Convert.ToInt64(missionId) });

                                }
                                else if (!string.IsNullOrEmpty(storyMissionId) && !string.IsNullOrEmpty(storyUserId))
                                {
                                    HttpContext.Session.Remove("StoryMissionId");
                                    HttpContext.Session.Remove("StoryUserId");
                                    return RedirectToAction("StoryDetail", "Story", new { missionId = Convert.ToInt64(storyMissionId), userId = Convert.ToInt64(storyUserId) });
                                }
                                else
                                {
                                    return RedirectToAction("PlatformLanding", "Mission");
                                }
                            }
                            else
                            {
                                return RedirectToAction("AdminUser", "Admin");
                            }
                        }
                        else
                        {
                            if (user.DeletedAt == null)
                            {
                                TempData["error"] = "User is blocked!!!";
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["error"] = "User is deleted!!!";
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                    else
                    {
                        TempData["error"] = "Incorrect password!!!";
                        return View(obj);
                    }
                }
            }
            return View(obj);
        }




        //FORGOT PASSWORD
        public IActionResult ForgotPassword ()
        {
            ForgotPasswordValidation vm = new();
            vm.banners = _userRepository.GetBanners();
            return View(vm);
        }

        [HttpPost]
        public IActionResult ForgotPassword (ForgotPasswordValidation obj)
        {
            obj.banners = _userRepository.GetBanners();
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj.Email);
                if (user == null)
                {
                    TempData["error"] = "User does not exist!!!";
                    return View(obj);
                }
                else
                {
                    string token = _emailGeneration.GenerateToken();
                    string PasswordResetLink = Url.Action("ResetPassword", "Home", new { Email = obj.Email, Token = token }, Request.Scheme);
                    _emailGeneration.GenerateEmail(token, PasswordResetLink, obj);
                    TempData["success"] = "Link sent to the registered email!!!";
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(obj);
        }






        //RESET PASSWORD
        public IActionResult ResetPassword (string email, string token)
        {
            var cutoffTime = DateTime.Now.AddHours(-4);
            var ResetPasswordData = _db.PasswordResets.Any(e => e.Email == email && e.Token == token && e.CreatedAt >= cutoffTime && e.DeletedAt == null);

            if (ResetPasswordData)
            {
                ResetPasswordValidation obj = new ResetPasswordValidation()
                {
                    Email = email,
                    Token = token
                };
                obj.banners = _userRepository.GetBanners();
                return View(obj);
            }
            else
            {
                TempData["error"] = "Link is not valid!!!";
                return RedirectToAction("ForgotPassword");
            }
        }

        [HttpPost]
        public IActionResult ResetPassword (ResetPasswordValidation obj, IFormCollection form)
        {
            obj.banners = _userRepository.GetBanners();
            if (ModelState.IsValid)
            {
                _userRepository.UpdatePassword(obj);
                _userRepository.expireLink(obj.Email, obj.Token);
                TempData["success"] = "Password " + Messages.update;
                return RedirectToAction("Index", "Home");

            }
            return View();
        }
        


        //CMS PAGE LIST
        [AllowAnonymous]
        public IActionResult CmsPage (long cmsId)
        {
            var viewmodel = new SessionUserViewModel();
            viewmodel.selectedCmsPage = _userRepository.GetCmsPage(cmsId);
            return View(viewmodel);
        }





        //Logout
        public IActionResult Logout ()
        {
            HttpContext.Session.Remove("Token");
            HttpContext.Request.Headers.Remove("Authorization");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}