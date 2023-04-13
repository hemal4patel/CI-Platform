using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminUser _adminUser;
        private readonly IAdminMission _adminMission;

        public AdminController (IAdminUser adminUser, IAdminMission adminMission)
        {
            _adminUser = adminUser;
            _adminMission = adminMission;
        }

        public IActionResult AdminUser ()
        {
            AdminUserViewModel vm = new();
            vm.users = _adminUser.GetUsers();
            return View(vm);
        }

        public IActionResult AdminMission ()
        {
            AdminMissionViewModel vm = new();
            vm.missions = _adminMission.GetMissions();
            return View(vm);
        }
    }
}
