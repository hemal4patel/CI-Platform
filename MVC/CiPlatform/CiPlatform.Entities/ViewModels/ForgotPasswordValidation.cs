using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class ForgotPasswordValidation
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}
