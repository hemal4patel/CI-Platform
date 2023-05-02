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
        [MinLength(3, ErrorMessage = "First Name must be atleast 3 characters long")]
        [MaxLength(16, ErrorMessage = "First Name cannot be longer than 16 characters")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name must contain only letters.")]
        public string FirstName { get; set; } = null!;

        [MinLength(3, ErrorMessage = "Last Name must be atleast 3 characters long")]
        [MaxLength(16, ErrorMessage = "Last Name cannot be longer than 16 characters")]
        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name must contain only letters.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$",
        ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone Number is required.")]
        [MaxLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        [MinLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must contain exactly 10 digits.")]
        public string PhoneNumber { get; set; } 

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be atleat 8 characters long")]
        [MaxLength(16, ErrorMessage = "Password cannot be longer than 16 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$", ErrorMessage = "Password must have at least one capital letter, one number, and one special character")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [Compare("Password", ErrorMessage = "Password doesn't match")]
        public string ConfirmPassword { get; set; } = null!;

        public List<Banner>? banners { get; set; }
    }
}
