using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Secure_Website.Models
{
    public class TaskViewModel 
    {
        public List<ScheduleTaskModel> ScheduleTasks { get; set; }
    }
}
