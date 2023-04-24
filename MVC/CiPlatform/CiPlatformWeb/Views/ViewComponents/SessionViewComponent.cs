﻿using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            long userId = 0;
            string userName = "";
            string emailId = "";
            string avatarName = null;
            string role = "";

            userId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));
            if (userId != 0)
            {
                User sessionUser = _db.Users.Where(user => user.UserId == userId).FirstOrDefault();
                userId = userId;
                userName = sessionUser.FirstName + " " + sessionUser.LastName;
                emailId = sessionUser.Email;
                role = sessionUser.Role;
                if (sessionUser.Avatar is not null)
                {
                    avatarName = sessionUser.Avatar;
                }
                else
                {
                    avatarName = null;
                }
            }

            SessionUserViewModel vm = new();
            vm.userId = userId;
            vm.userName = userName;
            vm.emailId = emailId;
            vm.role = role;
            vm.avatarName = avatarName;
            vm.cmsPages = _db.CmsPages.Where(cms => cms.DeletedAt == null).ToList();

            return View(view, vm);
        }
    }
}