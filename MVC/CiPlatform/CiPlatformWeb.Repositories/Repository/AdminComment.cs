using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var comments = _db.Comments.Where(c => c.DeletedAt == null);

            var list = comments.Select(c => new AdminCommentModel()
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
                comment.ApprovalStatus = "DECLINED";
            }
            else
            {
                comment.ApprovalStatus = "PUBLISHED";
            }
            comment.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
        }
    }
}
