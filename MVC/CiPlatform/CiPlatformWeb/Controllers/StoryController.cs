using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
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
        public IActionResult StoryListing ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _storyList.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;


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
        [HttpPost]
        public IActionResult StoryListing (StoryQueryParams viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _storyList.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;

                var vm = new StoryListingViewModel();

                var data = _storyList.GetStories(viewmodel);
                vm.StoryList = data.Item1;
                ViewBag.totalRecords = data.Item2;

                return PartialView("_StoryListing", vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public IActionResult ShareStory ()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _storyList.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;

                var vm = new ShareStoryViewModel();
                vm.MissionTitles = _storyList.GetMissions(userId);

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public IActionResult GetStory (long missionId)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            long userId = Convert.ToInt64(ViewBag.UserId);

            var story = _storyList.GetDraftedStory(missionId, userId);

            return Json(story);
        }

        [HttpPost]
        public IActionResult SaveStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _storyList.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;


                var story = _storyList.GetDraftedStory(viewmodel.MissionId, userId);
                if (story != null)
                {
                    _storyList.UpdateDraftedStory(viewmodel, story);

                    //if (viewmodel.VideoUrl != null)
                    //{
                    _storyList.UpdateStoryUrls(story.StoryId, viewmodel.VideoUrl);
                    //}

                    //if (viewmodel.Images != null)
                    //{
                    _storyList.UpdateStoryImages(story.StoryId, viewmodel.Images);
                    //}

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


        [HttpPost]
        public IActionResult SubmitStory (ShareStoryViewModel viewmodel)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _storyList.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;


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


        public IActionResult StoryDetail (long MissionId, long UserId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                //ViewBag.Email = HttpContext.Session.GetString("Email");
                //ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                long userId = Convert.ToInt64(ViewBag.UserId);
                User sessionUser = _storyList.sessionUser(userId);
                ViewBag.Email = sessionUser.Email;
                ViewBag.UserName = sessionUser.FirstName + " " + sessionUser.LastName;
                ViewBag.UserAvatar = sessionUser.Avatar;

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
