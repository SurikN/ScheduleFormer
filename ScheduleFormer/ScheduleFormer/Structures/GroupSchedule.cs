using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleFormer.Containers;
using ScheduleFormer.Enums;

namespace ScheduleFormer.Structures
{
    public class GroupSchedule : Schedule
    {
        public Group Audience { get; set; }

        public GroupSchedule(Group audience) : base()
        {
            Audience = audience;
        }
    }
}
