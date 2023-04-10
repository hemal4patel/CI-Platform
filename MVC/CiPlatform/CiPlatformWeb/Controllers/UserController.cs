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

        public UserController (ILogger<UserController> logger, ApplicationDbContext db, IUserProfile userProfile)
        {
            _logger = logger;
            _db = db;
            _userProfile = userProfile;
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

                vm.timeMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == "Time" && m.ApprovalStatus == "APPROVE").Include(m => m.Mission).ToList();

                vm.goalMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == "Goal" && m.ApprovalStatus == "APPROVE").Include(m => m.Mission).ToList();

                vm.timeBasedEntries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == "Time").Include(t => t.Mission).ToList();

                vm.goalBasedEnteries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == "Goal").Include(t => t.Mission).ToList();


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
                        var timeBasedEntry = _db.Timesheets.Where(t => t.TimesheetId == viewmodel.timeBasedSheet.timeSheetId).FirstOrDefault();

                        if (timeBasedEntry is not null)
                        {
                            TimeSpan timeSpan = new TimeSpan(viewmodel.timeBasedSheet.hours, viewmodel.timeBasedSheet.minutes, 0);
                            timeBasedEntry.MissionId = viewmodel.timeBasedSheet.timeMissions;
                            timeBasedEntry.Time = timeSpan;
                            timeBasedEntry.Action = null;
                            timeBasedEntry.DateVolunteered = viewmodel.timeBasedSheet.dateVolunteered;
                            timeBasedEntry.Notes = viewmodel.timeBasedSheet.message;
                            timeBasedEntry.UpdatedAt = DateTime.Now;

                            _db.Update(timeBasedEntry);
                            _db.SaveChanges();
                        }
                    }
                    //NEW TIME ENTRY ADD
                    else
                    {
                        TimeSpan timeSpan = new TimeSpan(viewmodel.timeBasedSheet.hours, viewmodel.timeBasedSheet.minutes, 0);
                        var timeBasedEntry = new Timesheet
                        {
                            UserId = userId,
                            MissionId = viewmodel.timeBasedSheet.timeMissions,
                            Time = timeSpan,
                            Action = null,
                            DateVolunteered = viewmodel.timeBasedSheet.dateVolunteered,
                            Notes = viewmodel.timeBasedSheet.message,
                            CreatedAt = DateTime.Now
                        };
                        _db.Timesheets.Add(timeBasedEntry);
                        _db.SaveChanges();
                    }
                }

                //GOAL BASED
                else
                {
                    //EXISTING GOAL ENTRY UPDATE
                    if (viewmodel.goalBasedSheet.timeSheetId is not null)
                    {
                        var goalBasedEntry = _db.Timesheets.Where(t => t.TimesheetId == viewmodel.goalBasedSheet.timeSheetId).FirstOrDefault();

                        if (goalBasedEntry is not null)
                        {
                            goalBasedEntry.MissionId = viewmodel.goalBasedSheet.goalMissions;
                            goalBasedEntry.Time = null;
                            goalBasedEntry.Action = viewmodel.goalBasedSheet.actions;
                            goalBasedEntry.DateVolunteered = viewmodel.goalBasedSheet.dateVolunteered;
                            goalBasedEntry.Notes = viewmodel.goalBasedSheet.message;
                            goalBasedEntry.UpdatedAt = DateTime.Now;

                            _db.Update(goalBasedEntry);
                            _db.SaveChanges();
                        }
                    }
                    //NEW GOAL ENTRY ADD
                    else
                    {
                        var goalBasedEntry = new Timesheet
                        {
                            UserId = userId,
                            MissionId = viewmodel.goalBasedSheet.goalMissions,
                            Time = null,
                            Action = viewmodel.goalBasedSheet.actions,
                            DateVolunteered = viewmodel.goalBasedSheet.dateVolunteered,
                            Notes = viewmodel.goalBasedSheet.message,
                            CreatedAt = DateTime.Now
                        };
                        _db.Timesheets.Add(goalBasedEntry);
                        _db.SaveChanges();
                    }
                }

                var vm = new VolunteeringTimesheetViewModel();
                vm.timeMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == "Time" && m.ApprovalStatus == "APPROVE").Include(m => m.Mission).ToList();
                vm.goalMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == "Goal" && m.ApprovalStatus == "APPROVE").Include(m => m.Mission).ToList();

                vm.timeBasedEntries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == "Time").Include(t => t.Mission).ToList();

                vm.goalBasedEnteries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == "Goal").Include(t => t.Mission).ToList();

                return View(vm);
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
                var timesheet = _db.Timesheets.Where(t => t.TimesheetId == id).FirstOrDefault();
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
                var timesheet = _db.Timesheets.Where(t => t.TimesheetId == id).FirstOrDefault();
                _db.Timesheets.Remove(timesheet);
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
