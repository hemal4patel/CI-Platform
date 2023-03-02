using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class RegisterValidation
    {
        [Required(ErrorMessage = "First Name is reqiured.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is reqiured.")]
        public string? LastName { get; set; }

        [MaxLength(10, ErrorMessage = "Phone number must be of 10 digits only.")]
        [MinLength(10, ErrorMessage = "Phone number must be of 10 digits only.")]
        [Phone]
        public long PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

    }
}
