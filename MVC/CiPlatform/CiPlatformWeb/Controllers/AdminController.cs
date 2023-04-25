using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAdminBanner _adminBanner;

        public AdminController (IAdminUser adminUser, IAdminMission adminMission, IAdminTheme adminTheme, IAdminSkill adminSkill, IAdminApplication adminApplication, IAdminStory adminStory, IAdminCms adminCms, IAdminBanner adminBanner)
        {
            _adminUser = adminUser;
            _adminMission = adminMission;
            _adminTheme = adminTheme; _adminSkill = adminSkill;
            _adminApplication = adminApplication;
            _adminStory = adminStory;
            _adminCms = adminCms;
            _adminBanner = adminBanner;
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminUser ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminUserViewModel vm = new();
                vm.users = _adminUser.GetUsers();
                vm.countryList = _adminUser.GetCountries();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult GetCitiesByCountry (long? countryId)
        {
            List<City> cities = _adminUser.GetCitiesByCountry(countryId);
            return Json(cities);
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddUser ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminUserViewModel vm = new();
                vm.countryList = _adminUser.GetCountries();
                return PartialView("_addUser", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult EditUser (long userId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteUser (long userId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminUser.DeleteUser(userId);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AdminUser (AdminUserViewModel vm)
        {
            if (HttpContext.Session.GetString("UserId") != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminCms ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminCmsViewModel vm = new();
                vm.cmsPages = _adminCms.GetCmsPages();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddCms ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return PartialView("_addCms");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult EditCms (long cmsId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminCmsViewModel vm = new();
                vm.newCms = _adminCms.GetCmsToEdit(cmsId);
                return PartialView("_addCms", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteCmsPage (long cmsId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminCms.DeleteCmsPage(cmsId);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AdminCms (AdminCmsViewModel vm)
        {
            if (HttpContext.Session.GetString("UserId") != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminMission ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminMissionViewModel vm = new();
                vm.missions = _adminMission.GetMissions();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddMission ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminMissionViewModel vm = new();
                vm.countryList = _adminUser.GetCountries();
                vm.themeList = _adminMission.GetThemes();
                vm.skillList = _adminMission.GetSkills();
                return PartialView("_addMission", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult EditMission (long missionId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteMission (long missionId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminMission.DeleteMission(missionId);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AdminMission (AdminMissionViewModel vm)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                if (vm.newMission.missionId != 0)
                {
                    if (_adminMission.MissionExistsForUpdate(vm.newMission.missionId, vm.newMission.misssionTitle, vm.newMission.organizationName))
                    {
                        return Ok(new { icon = "error", message = "Mission already exists!!!" });
                    }
                    else
                    {
                        _adminMission.EditMission(vm);
                        return Ok(new { icon = "success", message = "Mission updated successfully!!!" });
                    }

                }
                else
                {
                    if (_adminMission.MissionExistsForNew(vm.newMission.misssionTitle, vm.newMission.organizationName))
                    {
                        return Ok(new { icon = "error", message = "Mission already exists!!!" });
                    }
                    else
                    {
                        _adminMission.AddMission(vm);
                        return Ok(new { icon = "success", message = "Mission added successfully!!!" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminTheme ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminThemeViewModel vm = new();
                vm.themes = _adminTheme.GetThemes();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddTheme ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return PartialView("_addTheme");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult EditTheme (long themeId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminThemeViewModel vm = new();
                vm.newTheme = _adminTheme.GetThemeToEdit(themeId);
                return PartialView("_addTheme", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteTheme (long themeId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminTheme.DeleteTheme(themeId);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AdminTheme (AdminThemeViewModel vm)
        {
            if (HttpContext.Session.GetString("UserId") != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminSkill ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminSkillViewModel vm = new();
                vm.skills = _adminSkill.GetSkills();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddSkill ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return PartialView("_addSkill");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult EditSkill (long skillId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminSkillViewModel vm = new();
                vm.newSkill = _adminSkill.GetSkillToEdit(skillId);
                return PartialView("_addSkill", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteSkill (long skillId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminSkill.DeleteSkill(skillId);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AdminSkill (AdminSkillViewModel vm)
        {
            if (HttpContext.Session.GetString("UserId") != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminApplication ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminApplicationViewModel vm = new();
                vm.applications = _adminApplication.GetApplications();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult ChangeApplicationStatus (long applicationId, int status)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminApplication.ChangeApplicationStatus(applicationId, status);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminStory ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminStoryViewModel vm = new();
                vm.stories = _adminStory.GetStories();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult ViewStory (long storyId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminStoryViewModel vm = new();
                vm.storyDetail = _adminStory.GetStoryDetails(storyId);
                return PartialView("_viewStory", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteStory (long storyId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminStory.DeleteStory(storyId);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult ChangeStoryStatus (long storyId, int status)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminStory.ChangeStoryStatus(storyId, status);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminBanner ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminBannerViewModel vm = new();
                vm.banners = _adminBanner.GetBanners();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddBanner ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return PartialView("_addBanner");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult EditBanner (long bannerId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                AdminBannerViewModel vm = new();
                vm.newBanner = _adminBanner.GetBannerToEdit(bannerId);
                return PartialView("_addBanner", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteBanner (long bannerId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _adminBanner.DeleteBanner(bannerId);
                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AdminBanner (AdminBannerViewModel vm)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //addnew
                if (vm.newBanner.bannerId == null)
                {
                    _adminBanner.AddNewBanner(vm.newBanner);
                    return Ok(new { icon = "success", message = "Banner added successfully!!!" });
                }
                //update
                else
                {
                    _adminBanner.UpdateBanner(vm.newBanner);
                    return Ok(new { icon = "success", message = "Banner updated successfully!!!" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
