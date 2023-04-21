using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Models;
using CiPlatformWeb.Repositories.Interface;
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
        private readonly ApplicationDbContext _db;

        public HomeController (ILogger<HomeController> logger, IUserRepository userRepository, IEmailGeneration emailGeneration, ApplicationDbContext db)
        {
            _logger = logger;
            _userRepository = userRepository;
            _emailGeneration = emailGeneration;
            _db = db;
        }


        //GET
        public IActionResult Registration ()
        {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Registration (RegistrationValidation obj)
        {
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
        public IActionResult Index ()
        {
            HttpContext.Session.Remove("UserId");
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Index (LoginValidation obj)
        {
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
                        HttpContext.Session.SetString("UserId", user.UserId.ToString());

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
                        TempData["error"] = "Incorrect password!!!";
                        return View(obj);
                    }
                }
            }
            return View(obj);
        }

        //GET
        public IActionResult ForgotPassword ()
        {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult ForgotPassword (ForgotPasswordValidation obj)
        {
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
        public IActionResult ResetPassword (string email, string token)
        {
            ResetPasswordValidation obj = new ResetPasswordValidation()
            {
                Email = email,
                Token = token
            };
            return View(obj);
        }

        //POST
        [HttpPost]
        public IActionResult ResetPassword (ResetPasswordValidation obj, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                var ResetPasswordData = _db.PasswordResets.Any(e => e.Email == obj.Email && e.Token == obj.Token && e.DeletedAt == null);
                // && DateTime.Now.Subtract(e.CreatedAt).TotalHours < 4
                //var timeDifference = DateTime.Now - passwordReset.CreatedAt;
                //if (timeDifference.TotalHours >= 4)

                if (ResetPasswordData)
                {
                    _userRepository.UpdatePassword(obj);
                    _userRepository.expireLink(obj.Email, obj.Token);
                    TempData["success"] = "Password updated!!!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "Link is not valid!!!";
                    return View("ForgotPassword");
                }
            }
            return View();
        }


        //Logout
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