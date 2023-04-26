using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class RegistrationValidation
    {
        [MinLength(3, ErrorMessage = "First Name must be between 3 to 16 characters")]
        [MaxLength(16, ErrorMessage = "First Name must be between 3 to 16 characters")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; } = null!;

        [MinLength(3, ErrorMessage = "Last Name must be between 3 to 16 characters")]
        [MaxLength(16, ErrorMessage = "Last Name must be between 3 to 16 characters")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$",
        ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone Number is required.")]
        [MaxLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        [MinLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        public string PhoneNumber { get; set; } 

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(16, ErrorMessage = "Password must be between 8 to 16 characters")]
        [MinLength(8, ErrorMessage = "Password must be between 8 to 16 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [Compare("Password", ErrorMessage = "Password doesn't match")]
        public string ConfirmPassword { get; set; } = null!;

        public List<Banner>? banners { get; set; }
    }
}
