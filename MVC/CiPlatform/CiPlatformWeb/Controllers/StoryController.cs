using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CiPlatformWeb.Controllers
{
    public class StoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IStoryList _storyList;

        public StoryController (ApplicationDbContext db, IStoryList storyList)
        {
            _db = db;
            _storyList = storyList;
        }

        //GET
        [Authorize(Roles = "admin, user")]
        public IActionResult StoryListing ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

                var vm = new StoryListingViewModel();
                vm.CountryList = _db.Countries.ToList();
                vm.ThemeList = _db.MissionThemes.ToList();
                vm.SkillList = _db.Skills.ToList();

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //POST
        [Authorize(Roles = "admin, user")]
        [HttpPost]
        public IActionResult StoryListing (StoryQueryParams viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

                var vm = new StoryListingViewModel();

                var data = _storyList.GetStories(viewmodel);
                vm.StoryList = data.Item1;
                vm.StoryCount = data.Item2;

                return PartialView("_StoryListing", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize(Roles = "admin, user")]
        public IActionResult ShareStory ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);
                ViewBag.UserId = userId;

                var vm = new ShareStoryViewModel();
                vm.MissionTitles = _storyList.GetMissions(userId);

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize(Roles = "admin, user")]
        public IActionResult GetStory (long missionId)
        {
            string userIdStr = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(userIdStr);

            var story = _storyList.GetDraftedStory(missionId, userId);

            return Json(story);
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost]
        public IActionResult SaveStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

                var story = _storyList.GetDraftedStory(viewmodel.MissionId, userId);
                if (story != null)
                {
                    _storyList.UpdateDraftedStory(viewmodel, story);
                    _storyList.UpdateStoryUrls(story.StoryId, viewmodel.VideoUrl);
                    _storyList.UpdateStoryImages(story.StoryId, viewmodel.Images);
                    return Ok(new { icon = "warning", message = "Story saved as draft!!!", published = 0 });
                }
                else
                {
                    if (_storyList.CheckPublishedStory(viewmodel.MissionId, userId) == true)
                    {
                        return Ok(new { icon = "error", message = "Only one story for a mission is allowed!!!", published = 1 });
                    }
                    else
                    {
                        _storyList.AddNewStory(viewmodel, userId);
                        return Ok(new { icon = "success", message = "New story saved as draft!!!", published = 0 });
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize(Roles = "admin, user")]
        [HttpPost]
        public IActionResult SubmitStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);

                var story = _storyList.GetDraftedStory(viewmodel.MissionId, userId);
                if (story != null)
                {
                    _storyList.SubmitStory(viewmodel, story);

                    if (viewmodel.VideoUrl != null)
                    {
                        _storyList.UpdateStoryUrls(story.StoryId, viewmodel.VideoUrl);
                    }

                    if (viewmodel.Images != null)
                    {
                        _storyList.UpdateStoryImages(story.StoryId, viewmodel.Images);
                    }

                    return Ok(new { icon = "success", message = "Story submitted!!! Approval is pending!!!" });
                }
                else
                {
                    return Ok(new { icon = "error", message = "Only one story for a mission is allowed!!!" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize(Roles = "admin, user")]
        public IActionResult StoryDetail (long MissionId, long UserId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string userIdStr = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(userIdStr);
                ViewBag.UserId = userId;

                _storyList.IncreaseViewCount(MissionId, UserId);

                var vm = new StoryDetailViewModel();
                vm.storyDetail = _storyList.GetStoryDetails(MissionId, UserId);
                vm.userDetail = _storyList.GetUserList(userId);

                return View(vm);
            }
            else
            {
                HttpContext.Session.SetString("StoryMissionId", MissionId.ToString());
                HttpContext.Session.SetString("StoryUserId", UserId.ToString());

                return RedirectToAction("Index", "Home");
            }
        }



        [Authorize(Roles = "admin, user")]
        [HttpPost]
        public async Task<IActionResult> StoryInvite (long ToUserId, long StoryId, long FromUserId, long storyUserId, long storyMissionId)
        {
            var storyInvite = _storyList.HasAlreadyInvited(ToUserId, StoryId, FromUserId);
            if (storyInvite != null)
            {
                _storyList.ReInviteToStory(storyInvite);
            }
            else
            {
                _storyList.InviteToStory(FromUserId, ToUserId, StoryId);
            }
            var StoryLink = Url.Action("StoryDetail", "Story", new { MissionId = storyMissionId, UserId = storyUserId }, Request.Scheme);
            string link = StoryLink;
            await _storyList.SendInvitationToCoWorker(ToUserId, FromUserId, link);

            return Json(new { success = true });
        }

    }
}
