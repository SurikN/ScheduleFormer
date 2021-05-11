using System;
using System.Collections.Generic;
using System.Linq;
using ScheduleFormer.Containers;
using ScheduleFormer.Enums;
using ScheduleFormer.Models;

namespace ScheduleFormer.Structures
{
    public class GlobalSchedule
    {
        public Dictionary<string, GroupSchedule> GroupSchedules { get; set; } = new Dictionary<string, GroupSchedule>();

        private Dictionary<string, TeacherSchedule> _teacherSchedules = new Dictionary<string, TeacherSchedule>();

        private LecturesStorageModel _lecturesStorageModel;
    
        private int _iterationCounter = 0;

        public GlobalSchedule(LecturesStorageModel lectureStorageModel)
        {
            _lecturesStorageModel = lectureStorageModel;
            foreach (var group in _lecturesStorageModel.UniqueGroups)
            {
                GroupSchedules.Add(group.Name, new GroupSchedule(group));
            }
            
            foreach (var teacher in _lecturesStorageModel.UniqueTeachers)
            {
                _teacherSchedules.Add(teacher.Name, new TeacherSchedule(teacher));
            }
        }
        
        public int CreateScheduleOnLecture()
        {
            _iterationCounter = 0;
            var tempLectures = new List<Lecture>(_lecturesStorageModel.Lectures);

            Clear();

            foreach (var group in _lecturesStorageModel.UniqueGroups)
            {
                GroupSchedules.Add(group.Name, new GroupSchedule(group));
            }

            foreach (var teacher in _lecturesStorageModel.UniqueTeachers)
            {
                _teacherSchedules.Add(teacher.Name, new TeacherSchedule(teacher));
            }


            Lecture tempLecture;
            var rng = new Random();
            while (tempLectures.Count > 0)
            {
                var index = rng.Next(0, tempLectures.Count);
                tempLecture = tempLectures.ElementAt(index);

                AddLectureOnLecture(tempLecture.Name, tempLecture.Audience, tempLecture.Lecturer);

                tempLectures.RemoveAt(index);
            }

            return _iterationCounter;
        }

        public int CreateScheduleOnGroup()
        {
            _iterationCounter = 0;

            Clear();

            foreach (var group in _lecturesStorageModel.UniqueGroups)
            {
                GroupSchedules.Add(group.Name, new GroupSchedule(group));
            }

            foreach (var teacher in _lecturesStorageModel.UniqueTeachers)
            {
                _teacherSchedules.Add(teacher.Name, new TeacherSchedule(teacher));
            }
            var groupsBase = new List<Group>(_lecturesStorageModel.UniqueGroups);
            Random rng = new Random();
            foreach (var group in groupsBase)
            {
                var lectures = _lecturesStorageModel.Lectures.Where(a => a.Audience.Equals(group)).ToList();
                while (lectures.Count > 0)
                {
                    var lecture = lectures[rng.Next(0, lectures.Count)];
                    var emptyTimes = group.ScheduleLectures.Keys.Where(a =>
                        group.ScheduleLectures[a] == null);
                    foreach (var scheduleLecture in emptyTimes)
                    {
                        _iterationCounter++;
                        if (lecture.Lecturer.ScheduleLectures[scheduleLecture] == null)
                        {
                            lecture.Day = scheduleLecture.Item1;
                            lecture.LectureTime = scheduleLecture.Item2;
                            group.ScheduleLectures[scheduleLecture] = lecture;
                            lecture.Lecturer.ScheduleLectures[scheduleLecture] = lecture;
                            AddLecture(lecture.Name, group, lecture.Lecturer, lecture.Day, lecture.LectureTime);
                            break;
                        }
                    }

                    lectures.Remove(lecture);
                }
            }

            return _iterationCounter;
        }

        public int CreateScheduleOnTeacher()
        {
            _iterationCounter = 0;
            
            Clear();

            foreach (var group in _lecturesStorageModel.UniqueGroups)
            {
                GroupSchedules.Add(group.Name, new GroupSchedule(group));
            }

            foreach (var teacher in _lecturesStorageModel.UniqueTeachers)
            {
                _teacherSchedules.Add(teacher.Name, new TeacherSchedule(teacher));
            }
            var teacherBase = new List<Teacher>(_lecturesStorageModel.UniqueTeachers);
            Random rng = new Random();
            foreach (var teacher in teacherBase)
            {
                var lectures = _lecturesStorageModel.Lectures.Where(a => a.Lecturer.Equals(teacher)).ToList();
                while (lectures.Count > 0)
                {
                    var lecture = lectures[rng.Next(0, lectures.Count)];
                    foreach (var scheduleLecture in teacher.ScheduleLectures.Keys.Where(a =>
                        teacher.ScheduleLectures[a] == null))
                    {
                        _iterationCounter++;
                        if (lecture.Audience.ScheduleLectures[scheduleLecture] == null)
                        {
                            lecture.Day = scheduleLecture.Item1;
                            lecture.LectureTime = scheduleLecture.Item2;
                            teacher.ScheduleLectures[scheduleLecture] = lecture;
                            lecture.Lecturer.ScheduleLectures[scheduleLecture] = lecture;
                            AddLecture(lecture.Name, lecture.Audience, teacher, lecture.Day, lecture.LectureTime);
                            break;
                        }
                    }

                    lectures.Remove(lecture);
                }
            }

            return _iterationCounter;
        }

        public void AddLectureOnLecture(string name, Group audience, Teacher lecturer)
        {
            var groupFreeTimes = GroupSchedules[audience.Name].GetFreeTimes();
            var teacherFreeTimes = _teacherSchedules[lecturer.Name].GetFreeTimes();

            var day = SelectDay(groupFreeTimes, teacherFreeTimes);
            var time = groupFreeTimes[day].FirstSpareFreeTime(teacherFreeTimes[day]);
            AddLecture(name, audience, lecturer, day, time);
            _iterationCounter++;
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
            GroupSchedules.Clear();
            _teacherSchedules.Clear();
        }

        private Days SelectDay(Dictionary<Days, Day> days1, Dictionary<Days, Day> days2)
        {
            var freeDays = days1.Where(a => days2.ContainsKey(a.Key)).Select(a => a.Key)
                .Where(a => days1[a].SpareFreeTime(days2[a]).Any());
            var comparison = new Dictionary<(Day, Day), int>(); //days1 Day, days2 Day, 
            foreach (var freeDay in freeDays)
            {
                comparison.Add((days1[freeDay], days2[freeDay]), Math.Min(days1[freeDay].FreeTimes.Count(), days2[freeDay].FreeTimes.Count()));
            }

            return comparison.FirstOrDefault(a => a.Value == comparison.Max(i => i.Value)).Key.Item1.Today;
        }
    }
}
