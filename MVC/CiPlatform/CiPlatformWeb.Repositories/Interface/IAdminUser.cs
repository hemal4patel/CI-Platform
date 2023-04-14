using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminUser
    {
        public List<AdminUserModel> GetUsers ();

        public List<Country> GetCountries ();

        public List<City> GetCitiesByCountry (long countryId);

        public User GetUserToEdit(long userId);

        public bool UserExistsForNew (string email);

        public bool UserExistsForUpdate (string email, long? userId);

        public void AddNewUser (AdminUserModel user);

        public void UpdateUser (AdminUserModel user);
    }
}
