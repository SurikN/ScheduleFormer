using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleFormer.Enums;

namespace ScheduleFormer.Containers
{
    public class Teacher
    {
        public string Name { get; set; }

        public Teacher(string name)
        {
            Name = name;

            foreach (Days day in Enum.GetValues(typeof(Days)))
            {
                foreach (LectureTimes time in Enum.GetValues(typeof(LectureTimes)))
                {
                    ScheduleLectures.Add((day, time), null);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(Teacher))
            {
                return false;
            }

            return (((Teacher) obj).Name == Name);
        }

        public Dictionary<Group, (Lecture, int)> Load { get; set; } = new Dictionary<Group, (Lecture, int)>();

        public Dictionary<(Days, LectureTimes), Lecture> ScheduleLectures { get; set; } = new Dictionary<(Days, LectureTimes), Lecture>();

        public override string ToString()
        {
            return Name;
        }
    }
}
