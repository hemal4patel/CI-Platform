using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace CiPlatformWeb.Controllers
{
    public class StoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IStoryList _storyList;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly long userId;

        public StoryController (ApplicationDbContext db, IStoryList storyList, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _storyList = storyList;

            _httpContextAccessor = httpContextAccessor;
            string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            if (token is not null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadJwtToken(token);
                var claims = decodedToken.Claims;
                var customClaimString = decodedToken.Claims.FirstOrDefault(c => c.Type == "CustomClaimForUser")?.Value;
                var customClaimValue = JsonSerializer.Deserialize<User>(customClaimString);
                userId = customClaimValue.UserId;
            }
        }

        //GET
        [Authorize(Roles = "user")]
        public IActionResult StoryListing ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
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
        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult StoryListing (StoryQueryParams viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
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


        [Authorize(Roles = "user")]
        public IActionResult ShareStory ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var vm = new ShareStoryViewModel();
                vm.MissionTitles = _storyList.GetMissions(userId);

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize(Roles = "user")]
        public IActionResult GetStory (long missionId)
        {
            var story = _storyList.GetDraftedStory(missionId, userId);

            return Json(story);
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult SaveStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                Story story = _storyList.GetDraftedStory(viewmodel.MissionId, userId);
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


        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult SubmitStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                Story story = _storyList.GetDraftedStory(viewmodel.MissionId, userId);
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


        [AllowAnonymous]
        public IActionResult StoryDetail (long MissionId, long UserId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                _storyList.IncreaseViewCount(MissionId, UserId);

                StoryDetailViewModel vm = new StoryDetailViewModel();
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



        [Authorize(Roles = "user")]
        [HttpPost]
        public async Task<IActionResult> StoryInvite (long ToUserId, long StoryId, long FromUserId, long storyUserId, long storyMissionId)
        {
            StoryInvite storyInvite = _storyList.HasAlreadyInvited(ToUserId, StoryId, FromUserId);
            if (storyInvite != null)
            {
                _storyList.ReInviteToStory(storyInvite);
            }
            else
            {
                _storyList.InviteToStory(FromUserId, ToUserId, StoryId);
            }
            string StoryLink = Url.Action("StoryDetail", "Story", new { MissionId = storyMissionId, UserId = storyUserId }, Request.Scheme);
            string link = StoryLink;
            await _storyList.SendInvitationToCoWorker(ToUserId, FromUserId, link);

            return Json(new { success = true });
        }

    }
}
