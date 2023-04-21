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

        public IActionResult GetCitiesByCountry (long? countryId)
        {
            List<City> cities = _adminUser.GetCitiesByCountry(countryId);
            return Json(cities);
        }

        public IActionResult AddUser ()
        {
            AdminUserViewModel vm = new();
            vm.countryList = _adminUser.GetCountries();
            return PartialView("_addUser", vm);
        }

        public IActionResult EditUser (long userId)
        {
            AdminUserViewModel vm = new();
            vm.countryList = _adminUser.GetCountries();
            vm.newUser = _adminUser.GetUserToEdit(userId);
            if (vm.newUser.countryId != null)
            {
                long? countryId = vm.newUser.countryId;
                vm.cityList = _adminUser.GetCitiesByCountry(countryId);
            }

            return PartialView("_addUser", vm);
        }

        public IActionResult DeleteUser (long userId)
        {
            _adminUser.DeleteUser(userId);
            return Ok();
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
                    TempData["message"] = "User updated successfully!!!";
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

        public IActionResult AddCms ()
        {
            return PartialView("_addCms");
        }

        public IActionResult EditCms (long cmsId)
        {
            AdminCmsViewModel vm = new();
            vm.newCms = _adminCms.GetCmsToEdit(cmsId);
            return PartialView("_addCms", vm);
        }

        public IActionResult DeleteCmsPage (long cmsId)
        {
            _adminCms.DeleteCmsPage(cmsId);
            return Ok();
        }

        [HttpPost]
        public IActionResult AdminCms (AdminCmsViewModel vm)
        {
            if (vm.newCms.cmsId is not null)
            {
                if (_adminCms.CmsExistsForUpdate(vm.newCms.cmsId, vm.newCms.slug))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "Slug already exists!!!";
                }
                else
                {
                    _adminCms.EditCmsPage(vm.newCms);
                    TempData["icon"] = "success";
                    TempData["message"] = "Cms page updated successfully!!!";
                }
            }
            else
            {
                if (_adminCms.CmsExistsForNew(vm.newCms.slug))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "Slug already exists!!!";
                }
                else
                {
                    _adminCms.AddCmsPage(vm.newCms);
                    TempData["icon"] = "success";
                    TempData["message"] = "Cms page added successfully!!!";
                }
            }
            return RedirectToAction("AdminCms");
        }

        public IActionResult AdminMission ()
        {
            AdminMissionViewModel vm = new();
            vm.missions = _adminMission.GetMissions();
            return View(vm);
        }

        public IActionResult AddMission ()
        {
            AdminMissionViewModel vm = new();
            vm.countryList = _adminUser.GetCountries();
            vm.themeList = _adminMission.GetThemes();
            vm.skillList = _adminMission.GetSkills();
            return PartialView("_addMission", vm);
        }

        public IActionResult EditMission (long missionId)
        {
            AdminMissionViewModel vm = new();
            vm.newMission = _adminMission.GetMissionToEdit(missionId);
            vm.countryList = _adminUser.GetCountries();
            if (vm.newMission.countryId != null)
            {
                long? countryId = vm.newMission.countryId;
                vm.cityList = _adminUser.GetCitiesByCountry(countryId);
            }
            vm.themeList = _adminMission.GetThemes();
            vm.skillList = _adminMission.GetSkills();
            return PartialView("_addMission", vm);
        }

        public IActionResult DeleteMission (long missionId)
        {
            _adminMission.DeleteMission(missionId);
            return Ok();
        }

        [HttpPost]
        public IActionResult AdminMission (AdminMissionViewModel vm)
        {
            if (vm.newMission.missionId != 0)
            {
                if (_adminMission.MissionExistsForUpdate(vm.newMission.missionId, vm.newMission.misssionTitle, vm.newMission.organizationName))
                {
                    //TempData["icon"] = "error";
                    //TempData["message"] = "Mission already exists!!!";
                    return Ok(new { msg = "Mission already exists!!!" });
                }
                else
                {
                    _adminMission.EditMission(vm);
                    //TempData["icon"] = "success";
                    //TempData["message"] = "Mission updated successfully!!!";
                    return Ok(new { msg = "Mission updated successfully!!!" });
                }

            }
            else
            {
                if (_adminMission.MissionExistsForNew(vm.newMission.misssionTitle, vm.newMission.organizationName))
                {
                    //TempData["icon"] = "error";
                    //TempData["message"] = "Mission already exists!!!";
                    return Ok(new { msg = "Mission already exists!!!" });
                }
                else
                {
                    _adminMission.AddMission(vm);
                    //TempData["icon"] = "success";
                    //TempData["message"] = "Mission added successfully!!!";
                    return Ok(new { msg = "Mission added successfully!!!" });
                }
            }

        }

        public IActionResult AdminTheme ()
        {
            AdminThemeViewModel vm = new();
            vm.themes = _adminTheme.GetThemes();
            return View(vm);
        }

        public IActionResult AddTheme ()
        {
            return PartialView("_addTheme");
        }

        public IActionResult EditTheme (long themeId)
        {
            AdminThemeViewModel vm = new();
            vm.newTheme = _adminTheme.GetThemeToEdit(themeId);
            return PartialView("_addTheme", vm);
        }

        public IActionResult DeleteTheme (long themeId)
        {
            _adminTheme.DeleteTheme(themeId);
            return Ok();
        }

        [HttpPost]
        public IActionResult AdminTheme (AdminThemeViewModel vm)
        {
            //addnew
            if (vm.newTheme.themeId == null)
            {
                //exists
                if (_adminTheme.ThemeExistsForNew(vm.newTheme.themeName))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "Theme already exists!!!";
                }
                //add
                else
                {
                    _adminTheme.AddNewTheme(vm.newTheme);
                    TempData["icon"] = "success";
                    TempData["message"] = "Theme added successfully!!!";
                }
            }
            //update
            else
            {
                //exists
                if (_adminTheme.ThemeExistsForUpdate(vm.newTheme.themeId, vm.newTheme.themeName))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "Theme already exists!!!";
                }
                //update
                else
                {
                    _adminTheme.UpdateTheme(vm.newTheme);
                    TempData["icon"] = "success";
                    TempData["message"] = "Theme updated successfully!!!";
                }
            }

            return RedirectToAction("AdminTheme");
        }

        public IActionResult AdminSkill ()
        {
            AdminSkillViewModel vm = new();
            vm.skills = _adminSkill.GetSkills();
            return View(vm);
        }

        public IActionResult AddSkill ()
        {
            return PartialView("_addSkill");
        }

        public IActionResult EditSkill (long skillId)
        {
            AdminSkillViewModel vm = new();
            vm.newSkill = _adminSkill.GetSkillToEdit(skillId);
            return PartialView("_addSkill", vm);
        }

        public IActionResult DeleteSkill (long skillId)
        {
            _adminSkill.DeleteSkill(skillId);
            return Ok();
        }

        [HttpPost]
        public IActionResult AdminSkill (AdminSkillViewModel vm)
        {
            //addnew
            if (vm.newSkill.skillId == null)
            {
                //exists
                if (_adminSkill.SkillExistsForNew(vm.newSkill.skillName))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "Skill already exists!!!";
                }
                //add
                else
                {
                    _adminSkill.AddNewSkill(vm.newSkill);
                    TempData["icon"] = "success";
                    TempData["message"] = "Skill added successfully!!!";
                }
            }
            //update
            else
            {
                //exists
                if (_adminSkill.SkillExistsForUpdate(vm.newSkill.skillId, vm.newSkill.skillName))
                {
                    TempData["icon"] = "error";
                    TempData["message"] = "Skill already exists!!!";
                }
                //update
                else
                {
                    _adminSkill.UpdateSkill(vm.newSkill);
                    TempData["icon"] = "success";
                    TempData["message"] = "Skill updated successfully!!!";
                }
            }

            return RedirectToAction("AdminSkill");
        }

        public IActionResult AdminApplication ()
        {
            AdminApplicationViewModel vm = new();
            vm.applications = _adminApplication.GetApplications();
            return View(vm);
        }

        [HttpPost]
        public IActionResult ChangeApplicationStatus (long applicationId, int status)
        {
            _adminApplication.ChangeApplicationStatus(applicationId, status);
            return Ok();
        }

        public IActionResult AdminStory ()
        {
            AdminStoryViewModel vm = new();
            vm.stories = _adminStory.GetStories();
            return View(vm);
        }

        public IActionResult ViewStory ()
        {
            return PartialView("_viewStory");
        }

        public IActionResult DeleteStory (long storyId)
        {
            _adminStory.DeleteStory(storyId);
            return Ok();
        }

        [HttpPost]
        public IActionResult ChangeStoryStatus (long storyId, int status)
        {
            _adminStory.ChangeStoryStatus(storyId, status);
            return Ok();
        }
    }
}
