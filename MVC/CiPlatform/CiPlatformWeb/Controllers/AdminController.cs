using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminUser _adminUser;
        private readonly IAdminMission _adminMission;
        private readonly IAdminTheme _adminTheme;
        private readonly IAdminSkill _adminSkill;
        private readonly IAdminApplication _adminApplication;
        private readonly IAdminStory _adminStory;
        private readonly IAdminCms _adminCms;

        public AdminController (IAdminUser adminUser, IAdminMission adminMission, IAdminTheme adminTheme, IAdminSkill adminSkill, IAdminApplication adminApplication, IAdminStory adminStory, IAdminCms adminCms)
        {
            _adminUser = adminUser;
            _adminMission = adminMission;
            _adminTheme = adminTheme;
            _adminSkill = adminSkill;
            _adminApplication = adminApplication;
            _adminStory = adminStory;
            _adminCms = adminCms;
        }

        public IActionResult AdminUser ()
        {
            AdminUserViewModel vm = new();
            vm.users = _adminUser.GetUsers();
            vm.countryList = _adminUser.GetCountries();
            return View(vm);
        }

        public IActionResult GetCitiesByCountry (long countryId)
        {
            List<City> cities = _adminUser.GetCitiesByCountry(countryId);
            return Json(cities);
        }

        [HttpPost]
        public IActionResult AdminUser (AdminUserViewModel vm)
        {
            //addnew
            if (vm.newUser.userId == null)
            {
                //exists
                if (_adminUser.UserExistsForNew(vm.newUser.email))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "User already exists!!!";
                }
                //add
                else
                {
                    _adminUser.AddNewUser(vm.newUser);
                    TempData["icon"] = "success";
                    TempData["message"] = "User added successfully!!!";
                }
            }
            //update
            else
            {
                //exists
                if (_adminUser.UserExistsForUpdate(vm.newUser.email, vm.newUser.userId))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "User already exists!!!";
                }
                //update
                else
                {
                    _adminUser.UpdateUser(vm.newUser);
                    TempData["icon"] = "success";
                    TempData["message"] = "User added successfully!!!";
                }
            }

            return RedirectToAction("AdminUser");
        }

        public IActionResult AdminCms ()
        {
            AdminCmsViewModel vm = new();
            vm.cmsPages = _adminCms.GetCmsPages();
            return View(vm);
        }

        public IActionResult AdminMission ()
        {
            AdminMissionViewModel vm = new();
            vm.missions = _adminMission.GetMissions();
            return View(vm);
        }

        public IActionResult AdminTheme ()
        {
            AdminThemeViewModel vm = new();
            vm.themes = _adminTheme.GetThemes();
            return View(vm);
        }

        public IActionResult AdminSkill ()
        {
            AdminSkillViewModel vm = new();
            vm.skills = _adminSkill.GetSkills();
            return View(vm);
        }

        public IActionResult AdminApplication ()
        {
            AdminApplicationViewModel vm = new();
            vm.applications = _adminApplication.GetApplications();
            return View(vm);
        }

        public IActionResult AdminStory ()
        {
            AdminStoryViewModel vm = new();
            vm.stories = _adminStory.GetStories();
            return View(vm);
        }
    }
}
