using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class VolunteeringTimesheetViewModel
    {
        [Required(ErrorMessage = "Mission is requred.")]

        public long MissionId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime dateVolunteered { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string message { get; set; }
        
        [Range(0, 23,ErrorMessage = "Hours must be between 0 and 23 only.")]
        public int? hours { get; set; }
        
        [Range(0, 59, ErrorMessage = "Minutes must be between 0 and 59 only.")]
        public int? minutes { get; set; }    

        public int? actions { get; set; }
    }
}
