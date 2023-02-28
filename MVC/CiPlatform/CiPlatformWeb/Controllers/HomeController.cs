using CiPlatformWeb.Entities.DataModels;
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


        public HomeController (ILogger<HomeController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }


        //GET
        public IActionResult Registration ()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration (User obj)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj);
                if (user == null)
                {
                    _userRepository.RegisterUser(obj);
                    TempData["success"] = "Registered!!!";
                    return RedirectToAction("PlatformLanding", "Mission");
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

        public IActionResult ResetPassword ()
        {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult ForgotPassword (User obj)
        {
            if(ModelState.IsValid)
            {
                var user = _userRepository.CheckUser(obj);
                if(user == null)
                {
                    TempData["error"] = "User does not exist!!!";
                    return View(obj);
                }
                else
                {

                }
            }
            return View(obj);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}