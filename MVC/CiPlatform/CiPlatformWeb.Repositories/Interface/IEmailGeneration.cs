using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IEmailGeneration
    {

        string GenerateToken ();

        string GenerateLink (User obj, string token);

        void ResetPasswordAdd (User obj, string token);

        void GenerateEmail (User obj, string token, string PasswordResetLink);
    }
}
