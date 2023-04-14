using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminUser : IAdminUser
    {
        private readonly ApplicationDbContext _db;

        public AdminUser (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminUserModel> GetUsers ()
        {
            IQueryable<User> users = _db.Users.Where(u => u.DeletedAt == null).AsQueryable();

            IQueryable<AdminUserModel> list = users.Select(u => new AdminUserModel()
            {
                firstName = u.FirstName,
                lastName = u.LastName,
                email = u.Email,
                PhoneNumber = u.PhoneNumber,
                employeeId = u.EmployeeId,
                department = u.Department,
                status = u.Status
            });

            return list.ToList();
        }

        public List<Country> GetCountries ()
        {
            return _db.Countries.ToList();
        }

        public List<City> GetCitiesByCountry (long countryId)
        {
            return _db.Cities.Where(c => c.CountryId == countryId).ToList();
        }

        public bool UserExistsForNew (string email)
        {
            return _db.Users.Any(u => u.Email == email);
        }

        public bool UserExistsForUpdate (string email, long? userId)
        {
            return _db.Users.Any(u => u.Email == email && u.UserId != userId);
        }

        public void AddNewUser (AdminUserModel user)
        {
            User newUser = new User()
            {
                FirstName = user.firstName,
                LastName = user.lastName,
                Email = user.email,
                Password = user.password,
                PhoneNumber = user.PhoneNumber,
                EmployeeId = user.employeeId,
                Department = user.department,
                CityId = user.cityId,
                CountryId = user.countryId,
                ProfileText = user.profileText,
                Status = user.status
            };

            if(user.avatar is not null)
            {
                var fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + user.avatar.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "UserProfile", fileName);

                newUser.Avatar = fileName;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    user.avatar.CopyTo(stream);
                }
            }

            _db.Users.Add(newUser);
            _db.SaveChanges();
        }

        public void UpdateUser (AdminUserModel user)
        {
            User existingUser = _db.Users.Where(u => u.UserId == user.userId).FirstOrDefault();

            existingUser.FirstName = user.firstName;
            existingUser.LastName = user.lastName;
            existingUser.Email = user.email;
            existingUser.Password = user.password;
            existingUser.PhoneNumber = user.PhoneNumber;
            //if(user.avatarName != null)
            //{
            //    existingUser.Avatar = user.avatarName;
            //}
            //else
            //{
            //    existingUser.Avatar = null;
            //}   
            existingUser.EmployeeId= user.employeeId;
            existingUser.Department = user.department;
            existingUser.CityId = user.cityId;
            existingUser.CountryId = user.countryId;
            existingUser.ProfileText = user.profileText;
            existingUser.Status = user.status;
        }

    }
}
