using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CiPlatformWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IUserProfile _userProfile;
        private readonly IVolunteeringTimesheet _timesheet;


        public UserController (ILogger<UserController> logger, ApplicationDbContext db, IUserProfile userProfile, IVolunteeringTimesheet timesheet)
        {
            _logger = logger;
            _db = db;
            _userProfile = userProfile;
            _timesheet = timesheet;
        }

        //GET
        public IActionResult UserProfile ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _userProfile.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;


                var vm = new UserProfileViewModel();
                vm = _userProfile.GetUserDetails(userId);
                ViewBag.UserAvatar = vm.AvatarName;
                vm.CountryList = _userProfile.GetCountryList();
                vm.SkillList = _userProfile.GetSkillList();
                vm.UserSkills = _userProfile.GetUserSkills(userId);
                if (vm.CountryId != null)
                {
                    long countryId = Convert.ToInt64(vm.CountryId);
                    vm.CityList = _userProfile.GetCityList(countryId);
                }

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
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _userProfile.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;

                if (ModelState.IsValid)
                {
                    viewmodel = _userProfile.UpdateUserProfile(viewmodel);

                    viewmodel.CountryList = _userProfile.GetCountryList();
                    viewmodel.SkillList = _userProfile.GetSkillList();
                    viewmodel.UserSkills = _userProfile.GetUserSkills(userId);
                    if (viewmodel.CountryId != null)
                    {
                        long countryId = Convert.ToInt64(viewmodel.CountryId);
                        viewmodel.CityList = _userProfile.GetCityList(countryId);
                    }

                    
                    TempData["message"] = "Profile updated successfully!!!";

                    return RedirectToAction("UserProfile");
                }
                return View(viewmodel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
            long userId = Convert.ToInt64(ViewBag.UserId);
            ViewBag.UserAvatar = _db.Users.Where(u => u.UserId == userId).Select(u => u.Avatar).FirstOrDefault();

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

        [HttpPost]
        public IActionResult ContactUs (string subject, string message)
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(ViewBag.UserId);
            ViewBag.UserAvatar = _db.Users.Where(u => u.UserId == userId).Select(u => u.Avatar).FirstOrDefault();

            if (userId != null)
            {
                _userProfile.ContactUs(userId, subject, message);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        //GET
        public IActionResult VolunteeringTimesheet ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _userProfile.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;

                var vm = new VolunteeringTimesheetViewModel();

                vm.timeMissions = _timesheet.GetTimeBasedMission(userId);
                vm.goalMissions = _timesheet.GetGoalBasedMission(userId);
                vm.timeBasedEntries = _timesheet.GetTimeBasedEntries(userId);
                vm.goalBasedEnteries = _timesheet.GetGoalBasedEntries(userId);

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult VolunteeringTimesheet (VolunteeringTimesheetViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _userProfile.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;

                //TIME BASED
                if (viewmodel.timeBasedSheet is not null)
                {
                    //EXISTING TIME ENTRY UPDATE
                    if (viewmodel.timeBasedSheet.timeSheetId is not null)
                    {
                        var timeBasedEntry = _timesheet.GetEntry(viewmodel.timeBasedSheet.timeSheetId);
                        if (timeBasedEntry is not null)
                        {
                            _timesheet.UpdateTimeBasedEntry(timeBasedEntry, viewmodel.timeBasedSheet);
                        }
                        TempData["message"] = "Entry updated successfully!!!";
                    }
                    //NEW TIME ENTRY ADD
                    else
                    {
                        _timesheet.AddTimeBasedEntry(viewmodel.timeBasedSheet, userId);
                        TempData["message"] = "Entry added successfully!!!";
                    }
                }

                //GOAL BASED
                else
                {
                    //EXISTING GOAL ENTRY UPDATE
                    if (viewmodel.goalBasedSheet.timeSheetId is not null)
                    {
                        var goalBasedEntry = _timesheet.GetEntry(viewmodel.goalBasedSheet.timeSheetId);
                        if (goalBasedEntry is not null)
                        {
                            _timesheet.UpdateGoalBasedEntry(goalBasedEntry, viewmodel.goalBasedSheet);
                        }
                        TempData["message"] = "Entry updated successfully!!!";
                    }
                    //NEW GOAL ENTRY ADD
                    else
                    {
                        _timesheet.AddGoalBasedEntry(viewmodel.goalBasedSheet, userId);
                        TempData["message"] = "Entry added successfully!!!";
                    }
                }

                return RedirectToAction("VolunteeringTimesheet");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult GetTimesheetData (long id)
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(ViewBag.UserId);
            ViewBag.UserAvatar = _db.Users.Where(u => u.UserId == userId).Select(u => u.Avatar).FirstOrDefault();

            if (userId != null)
            {
                var timesheet = _timesheet.GetEntry(id);
                return Json(timesheet);
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public IActionResult DeleteTimesheetData (long id)
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(ViewBag.UserId);
            ViewBag.UserAvatar = _db.Users.Where(u => u.UserId == userId).Select(u => u.Avatar).FirstOrDefault();

            if (userId != null)
            {
                _timesheet.DeleteTimesheetEntry(id);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult PrivacyPolicy ()
        {
            //if (HttpContext.Session.GetString("UserId") != null)
            //{
            //    //ViewBag.Email = HttpContext.Session.GetString("Email");
            //    //ViewBag.UserName = HttpContext.Session.GetString("UserName");
            //    ViewBag.UserId = HttpContext.Session.GetString("UserId");
            //    long userId = Convert.ToInt64(ViewBag.UserId);
            //    User sessionUser = _userProfile.sessionUser(userId);
            //    ViewBag.Email = sessionUser.Email;
            //    ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
            //    ViewBag.UserAvatar = sessionUser.Avatar;

            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }
    }
}
