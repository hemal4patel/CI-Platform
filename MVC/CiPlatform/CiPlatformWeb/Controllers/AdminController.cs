using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminUser _adminUser;

        public AdminController (IAdminUser adminUser)
        {
            _adminUser = adminUser;
        }

        public IActionResult AdminUser ()
        {
            AdminUserViewModel vm = new();
            vm.users = _adminUser.GetUsers();
            return View(vm);
        }
    }
}
