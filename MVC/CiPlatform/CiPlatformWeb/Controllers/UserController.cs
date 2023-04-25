using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        [Authorize(Roles = "user")]
        //GET
        public IActionResult UserProfile ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

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


        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult UserProfile (UserProfileViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

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


        [Authorize(Roles = "user")]
        public IActionResult GetCitiesByCountry (int countryId)
        {
            var vm = new UserProfileViewModel();
            vm.CityList = _userProfile.GetCityList(countryId);
            return Json(vm.CityList);
        }


        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult ChangePassword (string oldPassoword, string newPassword, string confirmPassword)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

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


        //[Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult ContactUs (string subject, string message)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

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
            else
            {
                return RedirectToAction("Index", "Home"); 
            }
        }


        //GET
        [Authorize(Roles = "user")]
        public IActionResult VolunteeringTimesheet ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

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


        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult VolunteeringTimesheet (VolunteeringTimesheetViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);


                //TIME BASED
                if (viewmodel.timeBasedSheet is not null)
                {
                    if (_timesheet.TimeSheetExists(viewmodel.timeBasedSheet.timeMissions, userId, viewmodel.timeBasedSheet.dateVolunteered))
                    {
                        TempData["icon"] = "error";
                        TempData["message"] = "Entry alredy exists!!!";
                    }
                    else
                    {
                        //EXISTING TIME ENTRY UPDATE
                        if (viewmodel.timeBasedSheet.timeSheetId is not null)
                        {
                            var timeBasedEntry = _timesheet.GetEntry(viewmodel.timeBasedSheet.timeSheetId);
                            if (timeBasedEntry is not null)
                            {
                                _timesheet.UpdateTimeBasedEntry(timeBasedEntry, viewmodel.timeBasedSheet);
                            }
                            TempData["icon"] = "success";
                            TempData["message"] = "Entry updated successfully!!!";
                        }
                        //NEW TIME ENTRY ADD
                        else
                        {
                            _timesheet.AddTimeBasedEntry(viewmodel.timeBasedSheet, userId);
                            TempData["icon"] = "success";
                            TempData["message"] = "Entry added successfully!!!";
                        }
                    }
                }

                //GOAL BASED
                else
                {
                    if (_timesheet.TimeSheetExists(viewmodel.goalBasedSheet.goalMissions, userId, viewmodel.goalBasedSheet.dateVolunteered))
                    {
                        TempData["icon"] = "error";
                        TempData["message"] = "Entry alredy exists!!!";
                    }
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
                            TempData["icon"] = "success";
                            TempData["message"] = "Entry updated successfully!!!";
                        }
                        //NEW GOAL ENTRY ADD
                        else
                        {
                            _timesheet.AddGoalBasedEntry(viewmodel.goalBasedSheet, userId);
                            TempData["icon"] = "success";
                            TempData["message"] = "Entry added successfully!!!";
                        }
                    }
                }
                return RedirectToAction("VolunteeringTimesheet");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize(Roles = "user")]
        public IActionResult GetTimesheetData (long id)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

            if (userId != null)
            {
                var timesheet = _timesheet.GetEntry(id);
                var startDate = _db.Missions.Where(m => m.MissionId == timesheet.MissionId).Select(m => m.StartDate).FirstOrDefault();
                return Json(new { timesheet = timesheet, startDate = startDate });
            }
            else
            {
                return BadRequest();
            }
        }



        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult DeleteTimesheetData (long id)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

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

        [AllowAnonymous]
        public IActionResult PrivacyPolicy ()
        {
            UserProfileViewModel vm = new UserProfileViewModel();
            vm.PolicyPages = _userProfile.GetPolicyPages();
            return View(vm);
        }
    }
}
