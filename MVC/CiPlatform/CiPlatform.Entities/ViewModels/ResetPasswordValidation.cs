using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class ResetPasswordValidation
    {
        public string Password { get; set; }

        public string Email { get; set; } 

        public string Token { get; set; }

    }
}
