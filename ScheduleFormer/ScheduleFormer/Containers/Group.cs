using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleFormer.Enums;

namespace ScheduleFormer.Containers
{
    public class Group
    {
        public string Name { get; set; }

        public Group(string name)
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
            if (obj.GetType() != typeof(Group))
            {
                return false;
            }

            return (((Group)obj).Name == Name);
        }

        public Dictionary<Teacher, (Lecture, int)> Load { get; set; } = new Dictionary<Teacher, (Lecture, int)>();

        public Dictionary<(Days, LectureTimes), Lecture> ScheduleLectures { get; set; } = new Dictionary<(Days, LectureTimes), Lecture>();

        public override string ToString()
        {
            return Name;
        }
    }
}
