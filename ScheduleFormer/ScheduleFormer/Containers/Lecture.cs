using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleFormer.Enums;

namespace ScheduleFormer.Containers
{
    public class Lecture
    {
        public string Name { get; set; }

        public Group Audience { get; set; }

        public Teacher Lecturer { get; set; }

        public Days Day { get; set; }

        public LectureTimes LectureTime { get; set; }

        public Lecture()
        {

        }

        public Lecture(Lecture lecture)
        {
            Name = lecture.Name;
            Audience = lecture.Audience;
            Lecturer = lecture.Lecturer;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Lecture))
            {
                return false;
            }

            return ((Lecture) obj).Name == Name && ((Lecture) obj).Lecturer.Equals(Lecturer) &&
                   ((Lecture) obj).Audience.Equals(Audience);
        }

        public override string ToString()
        {
            return $"{Name}\n{Lecturer}";
        }
    }
}
