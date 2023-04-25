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


        //GET
        [AllowAnonymous]
        public IActionResult Registration ()
        {
            RegistrationValidation vm = new();
            vm.banners = _userRepository.GetBanners();
            return View(vm);
        }

        //POST
        [AllowAnonymous]
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
                    TempData["error"] = "User already exists!!!";
                    return View(obj);
                }
            }
            return View(obj);
        }

        //GET
        [AllowAnonymous]
        public IActionResult Index ()
        {
            HttpContext.Session.Remove("UserId");
            LoginValidation vm = new();
            vm.banners = _userRepository.GetBanners();
            return View(vm);
        }

        //POST
        [HttpPost]
        [AllowAnonymous]
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
                    if (BCrypt.Net.BCrypt.Verify(obj.Password, user.Password))
                    {
                        if (user.DeletedAt == null && user.Status == 1)
                        {
                            HttpContext.Session.SetString("UserId", user.UserId.ToString());

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
                            TempData["error"] = "Login failed!!!";
                            return RedirectToAction("Index", "Home");
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

        //GET
        [AllowAnonymous]
        public IActionResult ForgotPassword ()
        {
            ForgotPasswordValidation vm = new();
            vm.banners = _userRepository.GetBanners();
            return View(vm);
        }

        //POST
        [HttpPost]
        [AllowAnonymous]
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

        //GET
        [AllowAnonymous]
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

        //POST
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ResetPassword (ResetPasswordValidation obj, IFormCollection form)
        {
            obj.banners = _userRepository.GetBanners();
            if (ModelState.IsValid)
            {
                _userRepository.UpdatePassword(obj);
                _userRepository.expireLink(obj.Email, obj.Token);
                TempData["success"] = "Password updated!!!";
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult CmsPage (long cmsId)
        {
            var viewmodel = new SessionUserViewModel();
            viewmodel.selectedCmsPage = _userRepository.GetCmsPage(cmsId);
            return View(viewmodel);
        }

        //Logout
        [AllowAnonymous]
        public IActionResult Logout ()
        {
            //HttpContext.Session.SetString("Email", "");
            //HttpContext.Session.SetString("UserName", "");
            HttpContext.Session.Remove("UserId");
            //HttpContext.Session.SetString("UserAvatar", "");
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}