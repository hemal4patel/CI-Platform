using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class GoalBasedSheetViewModel
    {
        public long? timeSheetId { get; set; }

        [Required(ErrorMessage = "Mission is requred.")]
        public long goalMissions { get; set; }

        [Required(ErrorMessage = "Actions is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Actions must contain only digits.")]
        public int actions { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime dateVolunteered { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string message { get; set; }
    }
}
