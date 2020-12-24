using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ScheduleFormer.Containers;
using ScheduleFormer.Enums;

namespace ScheduleFormer.Structures
{
    public class GlobalSchedule
    {
        //private List<Teacher> _teachers;

        //private List<Group> _groups;

        public Dictionary<string, GroupSchedule> GroupSchedules { get; set; } = new Dictionary<string, GroupSchedule>();

        private Dictionary<string, TeacherSchedule> _teacherSchedules = new Dictionary<string, TeacherSchedule>();

        public GlobalSchedule(IEnumerable<Group> groups, IEnumerable<Teacher> teachers)
        {
            foreach (var group in groups)
            {
                GroupSchedules.Add(group.Name, new GroupSchedule(group));
            }

            foreach (var teacher in teachers)
            {
                _teacherSchedules.Add(teacher.Name, new TeacherSchedule(teacher));
            }
        }

        public void AddLecture(string name, Group audience, Teacher lecturer)
        {
            var groupFreeTimes = GroupSchedules[audience.Name].GetFreeTimes();
            var teacherFreeTimes = _teacherSchedules[lecturer.Name].GetFreeTimes();

            var freeDays = groupFreeTimes.Where(a => teacherFreeTimes.ContainsKey(a.Key));
            Days day = Days.Monday;
            LectureTimes time = LectureTimes.First;
            bool isSuccess = false;
            foreach (var freeDay in freeDays)
            {
                var e = groupFreeTimes[freeDay.Key].FreeTimes.Intersect(teacherFreeTimes[freeDay.Key].FreeTimes);
                if (!e.Any()) continue;
                time = e.First();
                day = freeDay.Key;
                isSuccess = true;
                break;
            }

            if (isSuccess)
            {
                AddLecture(name, audience, lecturer, day, time);
            }
            else
            {
                throw new Exception();
            }
        }

        private void AddLecture(string name, Group audience, Teacher lecturer, Days day, LectureTimes time)
        {
            var lecture = new Lecture()
            {
                Audience = audience,
                Day = day,
                LectureTime = time,
                Lecturer = lecturer,
                Name = name
            };
            if (GroupSchedules[audience.Name].Lectures[day].Lectures[time] != null)
            {
                throw new Exception();
            }
            else
            {
                GroupSchedules[audience.Name].Lectures[day].Lectures[time] = lecture;
            }

            if (_teacherSchedules[lecturer.Name].Lectures[day].Lectures[time] != null)
            {
                throw new Exception();
            }
            else
            {
                _teacherSchedules[lecturer.Name].Lectures[day].Lectures[time] = lecture;
            }
        }

        public void Clear()
        {
            foreach (var groupSchedule in GroupSchedules)
            {
                GroupSchedules.Remove(groupSchedule.Key);
                GroupSchedules.Add(groupSchedule.Key, new GroupSchedule(groupSchedule.Value.Audience));
            }

            foreach (var teacherSchedule in _teacherSchedules)
            {
                _teacherSchedules.Remove(teacherSchedule.Key);
                _teacherSchedules.Add(teacherSchedule.Key, new TeacherSchedule(teacherSchedule.Value.Lecturer));
            }
        }

        //public List<Lecture> GetGroupByDay(Group group, Days day)
        //{
        //    return _lectures.Where(a => a.Day == day).ToList();
        //}

        //public bool Add(string name, Group audience, Teacher lecturer)
        //{
        //    var dayToFill = GetMinDay(audience);
        //    for (int i = 1; i <= 7; i++)
        //    {

        //    }
        //}

        //private Days GetMinDay(Group audience)
        //{
        //    var result = Days.Monday;
        //    var number = GetGroupByDay(audience, Days.Monday).Count;
        //    for (int i = 2; i <= 5; i++)
        //    {
        //        if (GetGroupByDay(audience, (Days) i).Count < number)
        //        {
        //            result = (Days) i;
        //        }
        //    }

        //    return result;
        //}

        //private LectureTimes GetFirstFreeTime(Group audience, Days day)
        //{
        //    for(int i)
        //}
    }
}
