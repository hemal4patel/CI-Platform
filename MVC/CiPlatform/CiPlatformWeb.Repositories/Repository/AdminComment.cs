using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CiPlatformWeb.Repositories.EnumStats;
using static System.Net.Mime.MediaTypeNames;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminComment : IAdminComment
    {
        private readonly ApplicationDbContext _db;

        public AdminComment (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminCommentModel> GetComments ()
        {
            IQueryable<Comment> comments = _db.Comments.Where(c => c.DeletedAt == null);

            IQueryable<AdminCommentModel> list = comments.Select(c => new AdminCommentModel()
            {
                commentId = c.CommentId,
                mission = c.Mission.Title,
                user = c.User.FirstName + " " + c.User.LastName,
                status = c.ApprovalStatus
            });

            return list.ToList();
        }

        public void ChangeCommentStatus (long commentId, int status)
        {
            Comment comment = _db.Comments.FirstOrDefault(c => c.CommentId == commentId);

            if (status == 0)
            {
                comment.ApprovalStatus = commentStatus.declined.ToString().ToUpper();
            }
            else
            {
                comment.ApprovalStatus = commentStatus.published.ToString().ToUpper();
            }
            comment.UpdatedAt = DateTime.Now;

            UserSetting userSettingId = _db.UserSettings.Where(u => u.UserId == comment.UserId && u.SettingId == (long) notifications.comment).FirstOrDefault();

            if (_db.UserNotifications.Any(u => u.UserSettingId == userSettingId.UserSettingId && u.DeletedAt == null && u.CommentId == commentId))
            {
                UserNotification notification = _db.UserNotifications.FirstOrDefault(u => u.UserSettingId == userSettingId.UserSettingId && u.DeletedAt == null && u.CommentId == commentId);
                notification.Status = false;
                notification.CreatedAt = DateTime.Now;
            }
            else
            {
                UserNotification notification = new UserNotification()
                {
                    ToUserId = comment.UserId,
                    CommentId = commentId,
                    Status = false,
                    CreatedAt = DateTime.Now,
                    UserSettingId = userSettingId.UserSettingId
                };
                _db.UserNotifications.Add(notification);
            }

            _db.SaveChanges();
        }
    }
}
