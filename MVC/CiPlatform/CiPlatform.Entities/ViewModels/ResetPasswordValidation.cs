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
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Password is required")]
        [MaxLength(16, ErrorMessage = "Password must be between 8 to 16 characters")]
        [MinLength(8, ErrorMessage = "Password must be between 8 to 16 characters")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Password is required")]
        [Compare("Password", ErrorMessage = "Password doesn't match")]
        public string ConfirmPassword { get; set; } = null!;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$",
        ErrorMessage = "Invalid email address")]
        public string Email { get; set; } 

        public string Token { get; set; }

    }
}
