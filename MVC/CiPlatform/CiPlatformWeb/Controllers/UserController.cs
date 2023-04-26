using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace CiPlatformWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IUserProfile _userProfile;
        private readonly IVolunteeringTimesheet _timesheet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly long userId;

        public UserController (ILogger<UserController> logger, ApplicationDbContext db, IUserProfile userProfile, IVolunteeringTimesheet timesheet, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _db = db;
            _userProfile = userProfile;
            _timesheet = timesheet;

            _httpContextAccessor = httpContextAccessor;
            string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            if (token is not null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadJwtToken(token);
                var claims = decodedToken.Claims;
                var customClaimString = decodedToken.Claims.FirstOrDefault(c => c.Type == "CustomClaimForUser")?.Value;
                var customClaimValue = JsonSerializer.Deserialize<User>(customClaimString);
                userId = customClaimValue.UserId;
            }
        }

        [Authorize(Roles = "user")]
        //GET
        public IActionResult UserProfile ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
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

                    TempData["message"] = "Profile " + Messages.update;

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
                if (userId != null)
                {
                    _userProfile.ContactUs(userId, subject, message);
                    return Ok(new { status = 1 });
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return Ok(new { status = 0 });
            }
        }


        //GET
        [Authorize(Roles = "user")]
        public IActionResult VolunteeringTimesheet ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
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
                //TIME BASED
                if (viewmodel.timeBasedSheet is not null)
                {
                    if (_timesheet.TimeSheetExists(viewmodel.timeBasedSheet.timeMissions, userId, viewmodel.timeBasedSheet.dateVolunteered))
                    {
                        TempData["icon"] = "error";
                        TempData["message"] = "Entry " + Messages.exists;
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
                            TempData["message"] = "Entry " + Messages.update;
                        }
                        //NEW TIME ENTRY ADD
                        else
                        {
                            _timesheet.AddTimeBasedEntry(viewmodel.timeBasedSheet, userId);
                            TempData["icon"] = "success";
                            TempData["message"] = "Entry " + Messages.add;
                        }
                    }
                }

                //GOAL BASED
                else
                {
                    if (_timesheet.TimeSheetExists(viewmodel.goalBasedSheet.goalMissions, userId, viewmodel.goalBasedSheet.dateVolunteered))
                    {
                        TempData["icon"] = "error";
                        TempData["message"] = "Entry " + Messages.exists;
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
                            TempData["message"] = "Entry " + Messages.update;
                        }
                        //NEW GOAL ENTRY ADD
                        else
                        {
                            _timesheet.AddGoalBasedEntry(viewmodel.goalBasedSheet, userId);
                            TempData["icon"] = "success";
                            TempData["message"] = "Entry " + Messages.add;
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
