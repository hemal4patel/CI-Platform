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

        public AdminUser(ApplicationDbContext db) {
            _db = db;
        }

        public List<AdminUserModel> GetUsers ()
        {
            IQueryable<User> users = _db.Users.AsQueryable();

            IQueryable<AdminUserModel> list = users.Select(u => new AdminUserModel()
            {
                firstName = u.FirstName,
                lastName = u.LastName,
                email = u.Email,
                employeeId = u.EmployeeId,   
                department = u.Department,
                status = u.Status
            });

            return list.ToList();
        }
    }
}
