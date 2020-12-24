using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleFormer.Containers;
using ScheduleFormer.Enums;

namespace ScheduleFormer.Structures
{
    public class TeacherSchedule : Schedule
    {
        public Teacher Lecturer { get; set; }

        public TeacherSchedule(Teacher lecturer) : base()
        {
            Lecturer = lecturer;
        }
    }
}
