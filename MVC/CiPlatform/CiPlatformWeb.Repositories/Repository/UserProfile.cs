﻿using CiPlatformWeb.Entities.DataModels;
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
            User sessionUser = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            return sessionUser;
        }

        public UserProfileViewModel GetUserDetails (long userId)
        {
            User user = _db.Users.Where(u => u.UserId == userId).Include(u => u.UserSkills).FirstOrDefault();
            UserProfileViewModel vm = new UserProfileViewModel()
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
                Title = user.Title,
                Manager = user.Manager,
                Availability = user.Availability
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
            return _db.Skills.Where(s => s.DeletedAt == null && s.Status == 1).ToList();
        }

        public List<UserSkill> GetUserSkills (long userId)
        {
            return _db.UserSkills.Where(u => u.UserId == userId && u.DeletedAt == null && u.Skill.DeletedAt == null && u.Skill.Status == 1).Include(u => u.Skill).ToList();
        }


        public User CheckPassword (long userId, string oldPassoword)
        {
            User user = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();

            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(user.Password);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);

            if (result.Equals(oldPassoword))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public void UpdatePassword (User user, string newPassoword)
        {
            byte[] encData_byte = new byte[newPassoword.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(newPassoword);
            string encodedData = Convert.ToBase64String(encData_byte);

            user.Password = encodedData;
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
                user.Manager = viewmodel.Manager;
                user.Availability = viewmodel.Availability;
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

                    string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + viewmodel.AvatarImage.FileName;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "UserProfile", fileName);

                    user.Avatar = fileName;
                    viewmodel.AvatarName = fileName;

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
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
                    IQueryable<UserSkill> skills = _db.UserSkills.Where(s => s.UserId == viewmodel.UserId);
                    if (skills.Any())
                    {
                        foreach (UserSkill skill in skills)
                        {
                            skill.DeletedAt = DateTime.Now;
                        }
                        _db.SaveChanges();
                    }
                    string[] selectedSkillsStr = viewmodel.UserSelectedSkills.Split(',');
                    long[] selectedSkills = new long[selectedSkillsStr.Length];
                    for (int i = 0; i < selectedSkillsStr.Length; i++)
                    {
                        selectedSkills[i] = long.Parse(selectedSkillsStr[i]);
                        UserSkill newSkill = new UserSkill()
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
            ContactU data = new ContactU()
            {
                UserId = userId,
                Subject = subject,
                Message = message,
                CreatedAt = DateTime.Now,
            };
            _db.ContactUs.Add(data);
            _db.SaveChanges();
        }

        public List<CmsPage> GetPolicyPages ()
        {
            return _db.CmsPages.Where(c => c.DeletedAt == null && c.Status == 1).ToList();
        }
    }
}
