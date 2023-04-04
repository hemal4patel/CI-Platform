using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CiPlatformWeb.Entities.ViewModels;

namespace CiPlatformWeb.Repositories.Repository
{
    public class MissionList : IMissionList
    {
        private readonly ApplicationDbContext _db;

        public MissionList (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Country> GetCountryList ()
        {
            return _db.Countries.ToList();
        }

        public List<City> GetCityList (long countryId)
        {
            return _db.Cities.Where(c => c.CountryId == countryId).ToList();
        }

        public List<MissionTheme> GetThemeList ()
        {
            return _db.MissionThemes.ToList();
        }

        public List<Skill> GetSkillList ()
        {
            return _db.Skills.ToList();
        }

        public (List<Mission> missions, int count) GetMissions (DisplayMissionCards viewmodel, long userId)
        {
            var missions = _db.Missions.AsNoTracking();

            if (viewmodel.CountryId != null)
            {
                missions = missions.Where(m => m.CountryId == viewmodel.CountryId);
            }

            if (viewmodel.CityId != null)
            {
                missions = missions.Where(m => viewmodel.CityId.Contains(m.CityId));
            }

            if (viewmodel.ThemeId != null)
            {
                missions = missions.Where(m => viewmodel.ThemeId.Contains(m.ThemeId));
            }

            if (viewmodel.SkillId != null)
            {
                missions = missions.Where(m => m.MissionSkills.Any(s => viewmodel.SkillId.Contains(s.SkillId)));
            }

            if (viewmodel.searchText != null)
            {
                missions = missions.Where(m => m.Title.ToLower().Contains(viewmodel.searchText) || m.ShortDescription.ToLower().Contains(viewmodel.searchText) || m.Description.ToLower().Contains(viewmodel.searchText));
            }


            switch (viewmodel.sortCase)
            {
                case 1:
                    missions = missions.OrderByDescending(m => m.CreatedAt);
                    break;

                case 2:
                    missions = missions.OrderBy(m => m.CreatedAt);
                    break;

                case 3:
                    missions = missions.OrderByDescending(m => (m.TotalSeats - m.MissionApplications.Count(ma => ma.ApprovalStatus == "APPROVE")));
                    break;

                case 4:
                    missions = missions.OrderBy(m => (m.TotalSeats - m.MissionApplications.Count(ma => ma.ApprovalStatus == "APPROVE")));
                    break;

                case 5:
                    missions = missions.OrderByDescending(m => m.EndDate);
                    break;

                case 6:
                    missions = missions.Where(m => m.FavouriteMissions.Any(f => f.UserId == userId));
                    break;
            }

            int count = missions.Count();

            missions = missions
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.MissionRatings)
                .Include(m => m.Theme)
                .Include(m => m.MissionSkills).ThenInclude(m => m.Skill)
                .Include(m => m.MissionApplications)
                .Include(m => m.GoalMissions)
                .Include(m => m.FavouriteMissions)
                .Include(m => m.MissionMedia)
                .Skip(Math.Max((viewmodel.pageNo - 1) * 6, 0))
                .Take(6);


            return (missions.ToList(), count);
        }

        public List<User> GetUserList (long userId)
        {
            var recentVolunteers = _db.Users.Where(u => u.UserId != userId).ToList();

            return recentVolunteers;
        }

        public MissionInvite HasAlreadyInvited (long ToUserId, long MissionId, long FromUserId)
        {
            return _db.MissionInvites.Where(m => m.MissionId == MissionId && m.ToUserId == ToUserId && m.FromUserId == FromUserId).FirstOrDefault();
        }

        public async Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link)
        {
            var Email = await _db.Users.Where(u => u.UserId == ToUserId).FirstOrDefaultAsync();

            var Sender = await _db.Users.Where(s => s.UserId == FromUserId).FirstOrDefaultAsync();

            var fromEmail = new MailAddress("ciplatformdemo@gmail.com");
            var toEmail = new MailAddress(Email.Email);
            var fromEmailPassword = "oretveqrckcgcoog";
            string subject = "Mission Invitation";
            string body = "You Have Recieved Mission Invitation From " + Sender.FirstName + " " + Sender.LastName + " For:\n\n" + link;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            var message = new MailMessage(fromEmail, toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            await smtp.SendMailAsync(message);


        }
    }
}
