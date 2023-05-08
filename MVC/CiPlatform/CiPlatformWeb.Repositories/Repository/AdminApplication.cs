using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CiPlatformWeb.Repositories.EnumStats;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminApplication : IAdminApplication
    {
        private readonly ApplicationDbContext _db;

        public AdminApplication (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminApplicationModel> GetApplications ()
        {
            IQueryable<MissionApplication> appplications = _db.MissionApplications.Where(a => a.DeletedAt == null).AsQueryable();

            IQueryable<AdminApplicationModel> list = appplications.Select(a => new AdminApplicationModel()
            {
                applicationId = a.MissionApplicationId,
                missionTitle = a.Mission.Title,
                missionId = a.MissionId,
                userId = a.UserId,
                userName = a.User.FirstName + " " + a.User.LastName,
                appliedAt = a.CreatedAt.Value.ToShortDateString(),
                status = a.ApprovalStatus
            });

            return list.ToList();
        }

        public void ChangeApplicationStatus (long applicationId, int status)
        {
            MissionApplication missionApplication = _db.MissionApplications.Where(m => m.MissionApplicationId == applicationId).FirstOrDefault();

            if (status == 0)
            {
                missionApplication.ApprovalStatus = "DECLINE";
            }
            else
            {
                missionApplication.ApprovalStatus = "APPROVE";
            }
            missionApplication.UpdatedAt = DateTime.Now;

            //UserNotification notification = new UserNotification()
            //{
            //    ToUserId = missionApplication.UserId,
            //    MissionApplicationId = applicationId,
            //    Status = false,
            //    CreatedAt = DateTime.Now,
            //    UserSettingId = (long) notifications.missionApplication
            //};
            //_db.UserNotifications.Add(notification);

            _db.SaveChanges();
        }

    }
}
