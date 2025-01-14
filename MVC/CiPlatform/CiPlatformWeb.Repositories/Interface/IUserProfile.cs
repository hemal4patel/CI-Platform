﻿using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IUserProfile
    {
        public User sessionUser (long userId);

        public UserProfileViewModel GetUserDetails (long userId);

        public List<Country> GetCountryList ();

        public List<City> GetCityList (long GetCityList);

        public List<Skill> GetSkillList ();

        public List<UserSkill> GetUserSkills (long userId); 

        public User CheckPassword (long userId, string oldPassoword);

        public void UpdatePassword (User user, string newPassoword);

        public UserProfileViewModel UpdateUserProfile (UserProfileViewModel user);

        public void ContactUs (long userId, string subject, string message);

        public List<CmsPage> GetPolicyPages ();
    }
}
