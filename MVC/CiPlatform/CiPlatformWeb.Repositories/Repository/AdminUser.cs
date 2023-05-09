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
                userId = u.UserId,
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

        public List<City> GetCitiesByCountry (long? countryId)
        {
            return _db.Cities.Where(c => c.CountryId == countryId).ToList();
        }

        public AdminUserModel GetUserToEdit (long userId)
        {
            IQueryable<User> user = _db.Users.Where(u => u.UserId == userId && u.DeletedAt == null);

            AdminUserModel? list = user.Select(u => new AdminUserModel()
            {
                userId = u.UserId,
                firstName = u.FirstName,
                lastName = u.LastName,
                email = u.Email,
                PhoneNumber = u.PhoneNumber,
                password = u.Password,
                role = u.Role,
                employeeId = u.EmployeeId,
                department = u.Department,
                countryId = u.CountryId,
                cityId = u.CityId,
                status = u.Status
            }).FirstOrDefault();

            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(list.password);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            list.password = result;

            return list;
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
            byte[] encData_byte = new byte[user.password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(user.password);
            string encodedData = Convert.ToBase64String(encData_byte);

            User newUser = new User()
            {
                FirstName = user.firstName,
                LastName = user.lastName,
                Email = user.email,
                Password = encodedData,
                PhoneNumber = user.PhoneNumber,
                Role = user.role,
                EmployeeId = user.employeeId,
                Department = user.department,
                CityId = user.cityId,
                CountryId = user.countryId,
                Status = user.status,
                CreatedAt = DateTime.Now
            };
            _db.Users.Add(newUser);

            for (int i = 1; i <= 7; i++)
            {
                UserSetting userSetting = new UserSetting()
                {
                    SettingId = i,
                    UserId = newUser.UserId,
                    IsEnabled = true,
                };
                _db.UserSettings.Add(userSetting);
            }

            _db.SaveChanges();
        }

        public void UpdateUser (AdminUserModel user)
        {
            User existingUser = _db.Users.Where(u => u.UserId == user.userId).FirstOrDefault();
            byte[] encData_byte = new byte[user.password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(user.password);
            string encodedData = Convert.ToBase64String(encData_byte);

            existingUser.FirstName = user.firstName;
            existingUser.LastName = user.lastName;
            existingUser.Email = user.email;
            existingUser.Password = encodedData;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Role = user.role;
            existingUser.EmployeeId = user.employeeId;
            existingUser.Department = user.department;
            existingUser.CityId = user.cityId;
            existingUser.CountryId = user.countryId;
            existingUser.Status = user.status;
            existingUser.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
        }

        public void DeleteUser (long userId)
        {
            User user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            user.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
