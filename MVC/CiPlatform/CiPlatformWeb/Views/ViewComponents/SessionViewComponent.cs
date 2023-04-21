using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CiPlatformWeb.Views.ViewComponents
{
    public class SessionViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public SessionViewComponent (ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync (string view)
        {
            long userId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));
            User sessionUser = _db.Users.Where(user => user.UserId == userId).FirstOrDefault();

            SessionUserViewModel vm = new();

            vm.userId = userId;
            vm.userName = sessionUser.FirstName + " " + sessionUser.LastName;
            vm.emailId = sessionUser.Email;
            if(sessionUser.Avatar is not null)
            {
                vm.avatarName = sessionUser.Avatar;
            }
            else
            {
                vm.avatarName = null;
            }
            vm.cmsPages = _db.CmsPages.Where(cms => cms.DeletedAt == null).ToList();

            return View(view, vm);
        }
    }
}
