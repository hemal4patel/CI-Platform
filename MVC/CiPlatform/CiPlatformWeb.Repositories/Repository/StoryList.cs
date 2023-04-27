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
            User sessionUser = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            return sessionUser;
        }

        public (List<StoryListModel> stories, int count) GetStories (StoryQueryParams viewmodel)
        {
            IQueryable<Story> stories = _db.Stories.Where(s => s.Status == "PUBLISHED").AsQueryable();

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
            .Skip(Math.Max((viewmodel.pageNo - 1) * viewmodel.pagesize, 0))
            .Take(viewmodel.pagesize);


            IQueryable<StoryListModel> list = stories.Select(s => new StoryListModel()
            {
                story = s,
                mediaPath = s.StoryMedia.Where(s => s.DeletedAt == null && s.Type == "img").Select(s => s.Path).FirstOrDefault(),
                themeName = s.Mission.Theme.Title,
                storyUserAvatar = s.User.Avatar,
                storyUserName = s.User.FirstName + " " + s.User.LastName
            });
            return (list.ToList(), count);
        }

        public List<MissionApplication> GetMissions (long userId)
        {
            return _db.MissionApplications.Where(u => u.UserId == userId && u.ApprovalStatus == "APPROVE").Include(u => u.Mission).ToList();
        }

        public Story GetDraftedStory (long missionId, long userId)
        {
            return _db.Stories.Where(s => s.MissionId == missionId && s.UserId == userId && s.Status == "DRAFT")
                .Include(s => s.StoryMedia.Where(s => s.DeletedAt == null))
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
            IQueryable<StoryMedium> media = _db.StoryMedia.Where(s => s.StoryId == storyId && s.Type == "video");
            if (media.Any())
            {
                foreach (StoryMedium u in media)
                {
                    if (u != null)
                    {
                        u.DeletedAt = DateTime.Now;
                    }
                }
            }
            //add records
            foreach (string u in url)
            {
                if (u != null)
                {
                    StoryMedium newMedia = new StoryMedium()
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
            IQueryable<StoryMedium> media = _db.StoryMedia.Where(s => s.StoryId == storyId && s.Type == "img");
            if (media.Any())
            {
                foreach (StoryMedium m in media)
                {
                    if (m != null)
                    {
                        string fileName = m.Path;
                        File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "StoryPhotos", fileName));
                        m.DeletedAt = DateTime.Now;
                    }
                }
            }

            //add records
            if (images != null)
            {
                foreach (IFormFile u in images)
                {
                    if (u != null)
                    {
                        string fileName = Guid.NewGuid().ToString("N").Substring(0, 5) + "_" + u.FileName;
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "StoryPhotos", fileName);
                        StoryMedium newMedia = new StoryMedium()
                        {
                            StoryId = storyId,
                            Type = "img",
                            Path = fileName,
                            CreatedAt = DateTime.Now,
                        };
                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
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
            Story newStory = new Story()
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


        public int IncreaseViewCount (long MissionId, long UserId, long sessionUser)
        {
            int viewCount = 0;
            long storyId = _db.Stories.Where(s => s.MissionId == MissionId && s.UserId == UserId && s.Status == "PUBLISHED").Select(s=>s.StoryId).FirstOrDefault();

            //if (story != null)
            //{
            //    story.StoryViews = story.StoryViews + 1;
            //    story.UpdatedAt = DateTime.Now;
            //    _db.Update(story);
            //    _db.SaveChanges();
            //}

            if (storyId != 0)
            {
                bool view = _db.StoryViews.Any(s => s.StoryId == storyId && s.UserId == sessionUser);
                if(!view)
                {
                    StoryView storyView = new StoryView()
                    {
                        StoryId = storyId,
                        UserId = sessionUser
                    };
                    _db.StoryViews.Add(storyView);
                    _db.SaveChanges();
                }

                viewCount = _db.StoryViews.Count(s => s.StoryId == storyId);
            }

            return viewCount;
        }


        public Story GetStoryDetails (long MissionId, long UserId)
        {
            Story storyDetails = _db.Stories.Where(s => s.MissionId == MissionId && s.UserId == UserId)
                    .Include(s => s.StoryMedia.Where(s => s.DeletedAt == null))
                    .Include(s => s.Mission)
                    .Include(s => s.User).FirstOrDefault();
            return storyDetails;
        }


        public List<User> GetUserList (long userId)
        {
            List<User> list = _db.Users.Where(u => u.UserId != userId).ToList();
            return list;
        }

        public StoryInvite HasAlreadyInvited (long ToUserId, long StoryId, long FromUserId)
        {
            return _db.StoryInvites.Where(m => m.StoryId == StoryId && m.ToUserId == ToUserId && m.FromUserId == FromUserId).FirstOrDefault();
        }

        public void InviteToStory (long FromUserId, long ToUserId, long StoryId)
        {
            StoryInvite storyInvite = new StoryInvite()
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
            User Email = await _db.Users.Where(u => u.UserId == ToUserId).FirstOrDefaultAsync();

            User Sender = await _db.Users.Where(s => s.UserId == FromUserId).FirstOrDefaultAsync();

            MailAddress fromEmail = new MailAddress("ciplatformdemo@gmail.com");
            MailAddress toEmail = new MailAddress(Email.Email);
            string fromEmailPassword = "pdckerdmuutmdzhz";
            string subject = "Story Invitation";
            string body = "You Have Recieved Story Invitation From " + Sender.FirstName + " " + Sender.LastName + " For:\n\n" + link;

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            MailMessage message = new MailMessage(fromEmail, toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            await smtp.SendMailAsync(message);
        }
    }
}
