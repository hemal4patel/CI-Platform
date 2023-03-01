using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Models;
using CiPlatformWeb.Repositories.Interface;
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


        public HomeController (ILogger<HomeController> logger, IUserRepository userRepository, IEmailGeneration emailGeneration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _emailGeneration = emailGeneration;
        }


        //GET
        public IActionResult Registration ()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration (User obj, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj);
                if (user == null)
                {
                    if (form["cnf-password"] == obj.Password)
                    {
                        _userRepository.RegisterUser(obj);
                        TempData["success"] = "Registered!!!";
                        return RedirectToAction("PlatformLanding", "Mission");
                    }
                    else
                    {
                        TempData["error"] = "Password doesn't match!!!";
                        return View(obj);
                    }
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
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index (User obj)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj);
                if (user == null)
                {
                    TempData["error"] = "User does not exist!!!";
                    return View(obj);
                }
                else
                {
                    if (user.Password == obj.Password)
                    {
                        TempData["success"] = "Logged In!!!";
                        return RedirectToAction("PlatformLanding", "Mission");
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
                var user = _emailGeneration.CheckUser(obj);
                if (user == null)
                {
                    TempData["error"] = "User does not exist!!!";
                    return View(obj);
                }
                else
                {
                    _emailGeneration.GenerateEmail(obj);
                    TempData["success"] = "Link sent to the registered email!!!";
                }
            }
            return View(obj);
        }

        public IActionResult ResetPassword ()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword (ResetPasswordValidation obj, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (form["cnf-password"] == obj.Password)
                {
                    _userRepository.UpdatePassword(obj);
                    TempData["success"] = "Password updated!!!";
                    return RedirectToAction("PlatformLanding", "Mission");
                }
                else
                {
                    TempData["error"] = "Password doesn't match!!!";
                    return View(obj);
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}