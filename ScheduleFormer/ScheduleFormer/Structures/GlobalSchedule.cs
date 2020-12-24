using System;
using System.CodeDom;
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

            var day = SelectDay(groupFreeTimes, teacherFreeTimes);
            var time = groupFreeTimes[day].FirstSpareFreeTime(teacherFreeTimes[day]);
            AddLecture(name, audience, lecturer, day, time);


            //var freeDays = groupFreeTimes.Where(a => teacherFreeTimes.ContainsKey(a.Key));

            //Days day = Days.Monday;
            //LectureTimes time = LectureTimes.First;
            //bool isSuccess = false;
            //foreach (var freeDay in freeDays)
            //{
            //    var e = groupFreeTimes[freeDay.Key].FreeTimes.Intersect(teacherFreeTimes[freeDay.Key].FreeTimes);
            //    if (!e.Any()) continue;
            //    time = e.First();
            //    day = freeDay.Key;
            //    isSuccess = true;
            //    break;
            //}

            //if (isSuccess)
            //{
            //    AddLecture(name, audience, lecturer, day, time);
            //}
            //else
            //{
            //    throw new Exception();
            //}
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

        private Days SelectDay(Dictionary<Days, Day> days1, Dictionary<Days, Day> days2)
        {
            var freeDays = days1.Where(a => days2.ContainsKey(a.Key)).Select(a => a.Key)
                .Where(a => days1[a].SpareFreeTime(days2[a]).Any());
            var comparison = new Dictionary<(Day, Day), int>(); //days1 Day, days2 Day, 
            foreach (var freeDay in freeDays)
            {
                comparison.Add((days1[freeDay],days2[freeDay]), Math.Min(days1[freeDay].FreeTimes.Count(), days2[freeDay].FreeTimes.Count()));
            }

            return comparison.FirstOrDefault(a => a.Value == comparison.Max(i => i.Value)).Key.Item1.Today;
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
