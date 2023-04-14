﻿using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                missionTitle = a.Mission.Title,
                missionId = a.MissionId,
                userId = a.UserId,
                userName = a.User.FirstName + " " + a.User.LastName,
                appliedAt = a.CreatedAt.Value.ToShortDateString(),
                status = a.ApprovalStatus
            });

            return list.ToList();
        }
    }
}
