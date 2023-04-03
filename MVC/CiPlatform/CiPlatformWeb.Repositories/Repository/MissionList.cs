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
            return _db.Countries.ToList ();
        }

        public List<City> GetCityList (long countryId)
        {
            return _db.Cities.Where(c => c.CountryId== countryId).ToList ();
        }

        public List<MissionTheme> GetThemeList ()
        {
            return _db.MissionThemes.ToList ();
        }

        public List<Skill> GetSkillList ()
        {
            return _db.Skills.ToList ();
        }

        public IEnumerable<Mission> GetMissions (List<long> MissionIds)
        {
            var MissionList = _db.Missions.Where(m => MissionIds.Contains(m.MissionId))
                    .Include(m => m.City)
                    .Include(m => m.Country)
                    .Include(m => m.MissionSkills).ThenInclude(ms => ms.Skill)
                    .Include(m => m.Theme)
                    .Include(m => m.MissionRatings)
                    .Include(m => m.GoalMissions)
                    .Include(m => m.MissionApplications)
                    .Include(m => m.FavouriteMissions)
                    .Include(m => m.MissionMedia)
                    .ToList().OrderBy(ml => MissionIds.IndexOf(ml.MissionId));

            return MissionList;
        }

        public List<User> GetUserList (long userId)
        {
            var recentVolunteers = _db.Users.Where(u => u.UserId != userId).ToList();

            //var recentVolunteers = _db.MissionApplications.Include(u => u.User).Where(u => u.MissionId == MissionId && u.UserId != userId && u.ApprovalStatus == "APPROVE").OrderByDescending(u => u.CreatedAt).ToList();


            return recentVolunteers;
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
