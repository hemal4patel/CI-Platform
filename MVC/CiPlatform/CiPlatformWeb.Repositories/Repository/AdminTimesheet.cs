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
    public class AdminTimesheet : IAdminTimesheet
    {
        private readonly ApplicationDbContext _db;

        public AdminTimesheet (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AdminTimesheetModel> GetTimesheets ()
        {
            IQueryable<Timesheet> timesheets = _db.Timesheets.Where(t => t.DeletedAt == null).AsQueryable();

            IQueryable<AdminTimesheetModel> list = timesheets.Select(t => new AdminTimesheetModel()
            {
                timesheetId = t.TimesheetId,
                mission = t.Mission.Title,
                user = t.User.FirstName + " " + t.User.LastName,
                missionType = t.Mission.MissionType,
                dateVolunteered = t.DateVolunteered,
                action = t.Action,
                timeVolunteered = t.Time,
                status = t.Status
            });

            return list.ToList();
        }

        public void ChangeTimesheetStatus (long timesheetId, int status)
        {
            Timesheet timesheet = _db.Timesheets.Where(m => m.TimesheetId == timesheetId).FirstOrDefault();

            if (status == 0)
            {
                timesheet.Status = "DECLINED";
            }
            else
            {
                timesheet.Status = "APPROVED";
            }
            timesheet.UpdatedAt = DateTime.Now;

            //UserNotification notification = new UserNotification()
            //{
            //    ToUserId = timesheet.UserId,
            //    TimesheetId = timesheetId,
            //    Status = false,
            //    CreatedAt = DateTime.Now,
            //    UserSettingId = (long) notifications.timesheet
            //};
            //_db.UserNotifications.Add(notification);

            _db.SaveChanges();
        }

    }
}
