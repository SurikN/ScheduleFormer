using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleFormer.Enums;
using ScheduleFormer.Models;

namespace ScheduleFormer.Containers
{
    public class Day
    {
        public Days Today { get; set; }

        public Dictionary<LectureTimes, Lecture> Lectures { get; set; } = new Dictionary<LectureTimes, Lecture>();

        public IEnumerable<LectureTimes> FreeTimes
        {
            get
            {
                return Lectures.Where(a => a.Value == null).Select(a=>a.Key);
            }
        }

        public LectureTimes FirstFreeTime => FreeTimes.FirstOrDefault();

        public Day(Days day)
        {
            Today = day;

            foreach (LectureTimes value in typeof(LectureTimes).GetEnumValues())
            {
                Lectures.Add(value, null);
            }
        }

        public LectureTimes FirstSpareFreeTime(Day day)
        {
            return SpareFreeTime(day).FirstOrDefault();
        }

        public IEnumerable<LectureTimes> SpareFreeTime(Day day)
        {
            return FreeTimes.Intersect(day.FreeTimes);
        }


        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Day))
            {
                return false;
            }

            return ((Day) obj).Today == Today && ((Day)obj).FreeTimes.Any(a => FreeTimes.Contains(a));
        }
    }
}
