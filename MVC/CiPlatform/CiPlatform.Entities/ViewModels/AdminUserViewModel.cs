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

        [Required(ErrorMessage = "Name is required.")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$",
        ErrorMessage = "Invalid email address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [MaxLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        [MinLength(10, ErrorMessage = "Phone number must be of 10 digits only")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number must contain only digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Employee Id is required.")]
        public string employeeId { get; set; }

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
