using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IUserRepository 
    {
        void RegisterUser (User obj);

        User CheckUser (User obj);
    }
}
