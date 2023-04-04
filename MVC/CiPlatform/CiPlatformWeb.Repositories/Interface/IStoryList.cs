using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IStoryList
    {
        public (List<Story> stories, int count) GetStories (StoryListingViewModel viewmodel);

        public List<MissionApplication> GetMissions(long userId);

        public Story GetDraftedStory (long MissionId, long userId);

        public bool CheckPublishedStory (long MissionId, long userId);

        //public void DeleteVideoUrls (long storyId);

        //public void AddVideoUrls(long storyId, string[] url);

        public void UpdateDraftedStory (ShareStoryViewModel viewmodel, Story draftedStory);

        public void UpdateStoryImages (long storyId, List<IFormFile> images);

        public void UpdateStoryUrls (long storyId, string[] url);

        public void AddNewStory (ShareStoryViewModel viewmodel, long userId);

        public void SubmitStory (ShareStoryViewModel viewmodel, Story draftedStory);

        public void IncreaseViewCount (long MissionId, long UserId);

        public Story GetStoryDetails (long MissionId, long UserId);

        public List<User> GetUserList (long userId);

        public Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link);
    }
}
