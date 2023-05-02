using CiPlatformWeb.Entities.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminUserViewModel
    {
        public List<AdminUserModel> users { get; set; }

        public AdminUserModel newUser { get; set; }

        public List<Country> countryList { get; set; }

        public List<City> cityList { get; set; }
    }

    public class AdminUserModel
    {
        public long? userId { get; set; }

        [MinLength(3, ErrorMessage = "Name must be atleast 3 characters long")]
        [MaxLength(16, ErrorMessage = "Name cannot be longer than 16 characters")]
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters.")]
        public string firstName { get; set; }

        [MinLength(3, ErrorMessage = "Surname must be atleast 3 characters long")]
        [MaxLength(16, ErrorMessage = "Surname cannot be longer than 16 characters")]
        [Required(ErrorMessage = "Surname is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Surname must contain only letters.")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$",
        ErrorMessage = "Invalid email address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be atleat 8 characters long")]
        [MaxLength(16, ErrorMessage = "Password cannot be longer than 16 characters")]
        public string password { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [MaxLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        [MinLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number must contain only digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string role { get; set; }

        [MinLength(6, ErrorMessage = "Employee Id must be atleat 6 characters long")]
        [MaxLength(16, ErrorMessage = "Employee Id cannot be longer than 16 characters")]
        [Required(ErrorMessage = "Employee Id is required.")]
        public string employeeId { get; set; }

        [MinLength(2, ErrorMessage = "Department must be atleat 2 characters long")]
        [MaxLength(16, ErrorMessage = "Department cannot be longer than 16 characters")]
        [Required(ErrorMessage = "Department is required.")]
        public string department { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public long? countryId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public long? cityId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int status { get; set; }
    }
}
