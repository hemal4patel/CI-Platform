using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IUserRepository 
    {
        void RegisterUser (RegistrationValidation obj);

        User CheckUser (string email);

        void UpdatePassword (ResetPasswordValidation obj);

        bool verifyPassword (string objPassword, string userPassword);

        void expireLink(string email, string token);

        public CmsPage GetCmsPage (long id);

        public List<Banner> GetBanners ();

    }
}
