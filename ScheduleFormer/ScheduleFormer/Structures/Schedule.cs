using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using ScheduleFormer.Containers;
using ScheduleFormer.Enums;

namespace ScheduleFormer.Structures
{
    public abstract class Schedule
    {
        public Dictionary<Days, Day> Lectures { get; set; } = new Dictionary<Days, Day>();

        public Schedule()
        {
            for (int i = 1; i <= 5; i++)
            {
                Lectures.Add((Days) i, new Day((Days) i));
            }
        }

        public Days GetMinimumLecturesDay()
        {
            var result = Days.Monday;
            var numOfLectures = 8;
            int temp;
            foreach (Days day in Enum.GetValues(typeof(Days)))
            {
                if (Lectures[day].FreeTimes.Count() >= numOfLectures) continue;
                numOfLectures = Lectures[day].FreeTimes.Count();
                result = day;
            }

            return result;
        }

        public LectureTimes GetFirstFreeTime(Days day)
        {
            return GetFreeTimes(day).FirstOrDefault().Value.FirstFreeTime;
        }

        public Dictionary<Days, Day> GetFreeTimes(Days day)
        {
            return GetFreeTimes().Where(a => a.Key == day).ToDictionary(a => a.Key, a => a.Value);
        }

        public Dictionary<Days, Day> GetFreeTimes()
        {
            return Lectures.Where(a => a.Value.FreeTimes.Any()).ToDictionary(lecture => lecture.Key, lecture => lecture.Value);
        }

        public Days GetMinimumDay()
        {
            var num = 8;
            int temp;
            var result = Days.Monday;
            foreach (Days day in Enum.GetValues(typeof(Days)))
            {
                if ((temp = GetFreeTimes().Count(a => a.Key == day)) >= num) continue;
                num = temp;
                result = day;
            }

            return result;
        }
    }
}
