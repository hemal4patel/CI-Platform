using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class UserProfile : IUserProfile
    {
        private readonly ApplicationDbContext _db;

        public UserProfile (ApplicationDbContext db)
        {
            _db = db;
        }

        public User sessionUser (long userId)
        {
            var sessionUser = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            return sessionUser;
        }

        public UserProfileViewModel GetUserDetails (long userId)
        {
            User user = _db.Users.Where(u => u.UserId == userId).Include(u => u.UserSkills).FirstOrDefault();
            var vm = new UserProfileViewModel()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarName = user.Avatar,
                WhyIVolunteer = user.WhyIVolunteer,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                CityId = user.CityId,
                CountryId = user.CountryId,
                ProfileText = user.ProfileText,
                LinkedInUrl = user.LinkedInUrl,
                Title = user.Title
            };
            return vm;
        }

        public List<Country> GetCountryList ()
        {
            return _db.Countries.ToList();
        }


        public List<City> GetCityList (long countryId)
        {
            return _db.Cities.Where(c => c.CountryId == countryId).ToList();
        }

        public List<Skill> GetSkillList ()
        {
            return _db.Skills.ToList();
        }

        public List<UserSkill> GetUserSkills (long userId)
        {
            return _db.UserSkills.Where(u => u.UserId == userId).Include(u => u.Skill).ToList();
        }


        public User CheckPassword (long userId, string oldPassoword)
        {
            return _db.Users.Where(u => u.UserId == userId && u.Password == oldPassoword).FirstOrDefault();
        }

        public void UpdatePassword (User user, string newPassoword)
        {
            user.Password = newPassoword;
            user.UpdatedAt = DateTime.Now;
            _db.SaveChanges();
        }

        public UserProfileViewModel UpdateUserProfile (UserProfileViewModel viewmodel)
        {
            User user = _db.Users.Where(u => u.UserId == viewmodel.UserId).FirstOrDefault();

            if (user != null)
            {
                user.FirstName = viewmodel.FirstName;
                user.LastName = viewmodel.LastName;
                user.EmployeeId = viewmodel.EmployeeId;
                user.Title = viewmodel.Title;
                user.Department = viewmodel.Department;
                user.ProfileText = viewmodel.ProfileText;
                user.WhyIVolunteer = viewmodel.WhyIVolunteer;
                user.CountryId = viewmodel.CountryId;
                user.LinkedInUrl = viewmodel.LinkedInUrl;
                user.UpdatedAt = DateTime.Now;

                if (viewmodel.CityId != 0)
                {
                    user.CityId = viewmodel.CityId;
                }

                if (viewmodel.AvatarImage != null)
                {
                    if (user.Avatar is not null)
                    {
                        File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "UserProfile", user.Avatar));
                    }

                    var fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + viewmodel.AvatarImage.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "UserProfile", fileName);

                    user.Avatar = fileName;
                    viewmodel.AvatarName = fileName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        viewmodel.AvatarImage.CopyTo(stream);
                    }
                }
                else
                {
                    viewmodel.AvatarName = user.Avatar;
                }

                if (viewmodel.UserSelectedSkills != null)
                {
                    //remove skills
                    var skills = _db.UserSkills.Where(s => s.UserId == viewmodel.UserId);
                    if (skills.Any())
                    {
                        _db.RemoveRange(skills);
                    }
                    //long[] userSkills = viewmodel.UserSelectedSkills.Split(',');

                    string[] selectedSkillsStr = viewmodel.UserSelectedSkills.Split(',');

                    long[] selectedSkills = new long[selectedSkillsStr.Length];

                    for (int i = 0; i < selectedSkillsStr.Length; i++)
                    {
                        selectedSkills[i] = long.Parse(selectedSkillsStr[i]);
                        var newSkill = new UserSkill()
                        {
                            UserId = viewmodel.UserId,
                            SkillId = selectedSkills[i],
                            CreatedAt = DateTime.Now,
                        };
                        _db.UserSkills.Add(newSkill);
                    }
                }

                _db.SaveChanges();
            }

            return viewmodel;
        }

        public void ContactUs (long userId, string subject, string message)
        {
            var data = new ContactU()
            {
                UserId = userId,
                Subject = subject,
                Message = message,
                CreatedAt = DateTime.Now,
            };
            _db.ContactUs.Add(data);
            _db.SaveChanges();
        }


    }
}
