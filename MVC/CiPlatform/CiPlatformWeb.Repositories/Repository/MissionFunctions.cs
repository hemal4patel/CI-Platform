using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class MissionFunctions : IMissionFunctions
    {
        private readonly ApplicationDbContext _db;

        public MissionFunctions (ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddComment (long missionId, long userId, string comment)
        {
            var newComment = new Comment
            {
                MissionId = missionId,
                UserId = userId,
                CommentText = comment,
                CreatedAt = DateTime.Now
            };
            _db.Comments.Add(newComment);
            _db.SaveChanges();
        }
    }
}
