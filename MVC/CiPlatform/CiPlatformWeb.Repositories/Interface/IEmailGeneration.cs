using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IEmailGeneration
    {

        public string GenerateToken ();

        public void GenerateEmail (string token, string PasswordResetLink, ForgotPasswordValidation obj);

    }
}
