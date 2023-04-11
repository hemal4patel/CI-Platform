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
using System.Reflection;

namespace CiPlatformWeb.Repositories.Repository
{
    public class StoryList : IStoryList
    {
        private readonly ApplicationDbContext _db;

        public StoryList (ApplicationDbContext db)
        {
            _db = db;
        }

        public User sessionUser (long userId)
        {
            var sessionUser = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            return sessionUser;
        }

        public (List<Story> stories, int count) GetStories (StoryListingViewModel viewmodel)
        {
            var stories = _db.Stories.Where(s => s.Status == "PUBLISHED").AsNoTracking();

            if (viewmodel.CountryId != null)
            {
                stories = stories.Where(s => s.Mission.CountryId == viewmodel.CountryId);
            }

            if (viewmodel.CityId != null)
            {
                stories = stories.Where(s => viewmodel.CityId.Contains(s.Mission.CityId));
            }

            if (viewmodel.ThemeId != null)
            {
                stories = stories.Where(s => viewmodel.ThemeId.Contains(s.Mission.ThemeId));
            }

            if (viewmodel.SkillId != null)
            {
                stories = stories.Where(s => s.Mission.MissionSkills.Any(skill => viewmodel.SkillId.Contains(skill.SkillId)));
            }

            if (viewmodel.searchText != null)
            {
                stories = stories.Where(s => s.Title.ToLower().Replace(" ", "").Contains(viewmodel.searchText) || s.Description.ToLower().Contains(viewmodel.searchText));
            }

            int count = stories.Count();

            stories = stories
                .Include(s => s.StoryMedia)
                .Include(s => s.Mission)
                .ThenInclude(s => s.Theme)
                .Include(s => s.User)
                .Skip(Math.Max((viewmodel.pageNo - 1) * 3, 0))
                .Take(3);

            return (stories.ToList(), count);
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
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "StoryPhotos", fileName));
                    _db.Remove(m);
                }
            }

            //add records
            if (images != null)
            {
                foreach (var u in images)
                {
                    if (u != null)
                    {
                        var fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + u.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "StoryPhotos", fileName);

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
            Story storyDetails = _db.Stories.Where(s => s.MissionId == MissionId && s.UserId == UserId)
                    .Include(s => s.StoryMedia)
                    .Include(s => s.Mission)
                    .Include(s => s.User).FirstOrDefault();
            return storyDetails;
        }


        public List<User> GetUserList (long userId)
        {
            var list = _db.Users.Where(u => u.UserId != userId).ToList();
            return list;
        }

        public StoryInvite HasAlreadyInvited (long ToUserId, long StoryId, long FromUserId)
        {
            return _db.StoryInvites.Where(m => m.StoryId == StoryId && m.ToUserId == ToUserId && m.FromUserId == FromUserId).FirstOrDefault();
        }

        public void InviteToStory (long FromUserId, long ToUserId, long StoryId)
        {
            var storyInvite = new StoryInvite()
            {
                FromUserId = FromUserId,
                ToUserId = ToUserId,
                StoryId = StoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _db.StoryInvites.Add(storyInvite);
            _db.SaveChanges();
        }

        public void ReInviteToStory (StoryInvite storyInvite)
        {
            storyInvite.UpdatedAt = DateTime.Now;
            _db.Update(storyInvite);
            _db.SaveChanges();
        }

        public async Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link)
        {
            var Email = await _db.Users.Where(u => u.UserId == ToUserId).FirstOrDefaultAsync();

            var Sender = await _db.Users.Where(s => s.UserId == FromUserId).FirstOrDefaultAsync();

            var fromEmail = new MailAddress("ciplatformdemo@gmail.com");
            var toEmail = new MailAddress(Email.Email);
            var fromEmailPassword = "oretveqrckcgcoog";
            string subject = "Story Invitation";
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
