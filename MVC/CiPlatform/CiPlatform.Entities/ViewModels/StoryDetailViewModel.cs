﻿using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class StoryDetailViewModel
    {
        public Story storyDetail { get; set; }

        public User userDetail { get; set; }
    }
}
