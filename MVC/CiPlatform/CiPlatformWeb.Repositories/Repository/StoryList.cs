using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class StoryList : IStoryList
    {
        private readonly ApplicationDbContext _db;

        public StoryList (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Story> GetStories (List<long> StoryIds)
        {
            return _db.Stories.Where(s => StoryIds.Contains(s.StoryId))
                .Include(s => s.StoryMedia)
                .Include(s => s.Mission)
                .ThenInclude(s => s.Theme)
                .Include(s => s.User)
                .ToList();
        }

        public List<MissionApplication> GetMissions (long userId)
        {
            return _db.MissionApplications.Where(u => u.UserId == userId && u.ApprovalStatus == "APPROVE").Include(u => u.Mission).ToList();
        }

        public Story GetDraftedStory (long missionId, long userId)
        {
            return _db.Stories.Where(s => s.MissionId == missionId && s.UserId == userId && s.Status == "DRAFT")
                .Include(s => s.StoryMedia)
                .FirstOrDefault();
        }

        public bool CheckPublishedStory (long MissionId, long userId)
        {
            return _db.Stories.Any(s => s.MissionId == MissionId && s.UserId == userId && s.Status != "DRAFT");
        }

        public void UpdateDraftedStory (ShareStoryViewModel viewmodel, Story draftedStory)
        {
            draftedStory.Title = viewmodel.StoryTitle;
            draftedStory.Description = viewmodel.StoryDescription;
            draftedStory.Status = "DRAFT";
            draftedStory.UpdatedAt = DateTime.Now;
            _db.Update(draftedStory);
            _db.SaveChanges();
        }


        public void UpdateStoryUrls (long storyId, string[] url)
        {
            //delete records
            var media = _db.StoryMedia.Where(s => s.StoryId == storyId && s.Type == "video");
            if (media.Any())
            {
                _db.RemoveRange(media);
            }

            //add records
            foreach (var u in url)
            {
                if (u != null)
                {
                    var newMedia = new StoryMedium()
                    {
                        StoryId = storyId,
                        Type = "video",
                        Path = u,
                        CreatedAt = DateTime.Now,
                    };
                    _db.StoryMedia.Add(newMedia);
                }
            }

            _db.SaveChanges();
        }

        public void UpdateStoryImages (long storyId, List<IFormFile> images)
        {
            //delete records
            var media = _db.StoryMedia.Where(s => s.StoryId == storyId && s.Type == "img");
            foreach (var m in media)
            {
                if (m != null)
                {
                    var fileName = m.Path;
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", fileName));
                    _db.Remove(m);
                }
            }

            //add records
            foreach (var u in images)
            {
                if (u != null)
                {
                    var fileName = Guid.NewGuid().ToString("N") + "_" + u.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", fileName);

                    var newMedia = new StoryMedium()
                    {
                        StoryId = storyId,
                        Type = "img",
                        Path = fileName,
                        CreatedAt = DateTime.Now,
                    };

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        u.CopyTo(stream);
                    }

                    _db.StoryMedia.Add(newMedia);
                }
            }

            _db.SaveChanges();
        }


        public void AddNewStory (ShareStoryViewModel viewmodel, long userId)
        {
            var newStory = new Story()
            {
                MissionId = viewmodel.MissionId,
                UserId = userId,
                Title = viewmodel.StoryTitle,
                Description = viewmodel.StoryDescription,
                Status = "DRAFT",
                CreatedAt = DateTime.Now,
            };
            _db.Stories.Add(newStory);
            _db.SaveChanges();

            Story story = GetDraftedStory(viewmodel.MissionId, userId);

            if (viewmodel.VideoUrl != null)
            {
                UpdateStoryUrls(story.StoryId, viewmodel.VideoUrl);
            }

            if (viewmodel.Images != null)
            {
                UpdateStoryImages(story.StoryId, viewmodel.Images);
            }
        }

        public void SubmitStory (ShareStoryViewModel viewmodel, Story draftedStory)
        {
            draftedStory.Title = viewmodel.StoryTitle;
            draftedStory.Description = viewmodel.StoryDescription;
            draftedStory.Status = "PENDING";
            draftedStory.UpdatedAt = DateTime.Now;
            _db.Update(draftedStory);
            _db.SaveChanges();
        }


        public void IncreaseViewCount (long MissionId, long UserId)
        {
            Story story = _db.Stories.Where(s => s.MissionId == MissionId && s.UserId == UserId && s.Status == "PUBLISHED").FirstOrDefault();

            if (story != null)
            {
                story.StoryViews = story.StoryViews + 1;
                story.UpdatedAt = DateTime.Now;
                _db.Update(story);
                _db.SaveChanges();
            }
        }


        public Story GetStoryDetails (long MissionId, long UserId)
        {
            return _db.Stories.Where(s => s.MissionId == MissionId && s.UserId == UserId)
                    .Include(s => s.StoryMedia)
                    .Include(s => s.Mission)
                    .Include(s => s.User).FirstOrDefault();
        }


        public List<User> GetUserList (long userId)
        {
            var list = _db.Users.Where(u => u.UserId != userId).ToList();
            return list;
        }


        public async Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link)
        {
            var Email = await _db.Users.Where(u => u.UserId == ToUserId).FirstOrDefaultAsync();

            var Sender = await _db.Users.Where(s => s.UserId == FromUserId).FirstOrDefaultAsync();

            var fromEmail = new MailAddress("akshayghadiya28@gmail.com");
            var toEmail = new MailAddress(Email.Email);
            var fromEmailPassword = "dmsmefwcumhbtthp";
            string subject = "Mission Invitation";
            string body = "You Have Recieved Story Invitation From " + Sender.FirstName + " " + Sender.LastName + " For:\n\n" + link;

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
