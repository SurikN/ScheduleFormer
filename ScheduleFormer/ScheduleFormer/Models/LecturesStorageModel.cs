using System.Collections.Generic;
using ScheduleFormer.Containers;

namespace ScheduleFormer.Models
{
    public class LecturesStorageModel
    {
        private static LecturesStorageModel _instance;

        private LecturesStorageModel()
        {

        }

        public static LecturesStorageModel GetInstance()
        {
            return _instance ?? (_instance = new LecturesStorageModel());
        }

        public List<Lecture> Lectures { get; } = new List<Lecture>();

        public List<Group> UniqueGroups { get; } = new List<Group>();

        public List<Teacher> UniqueTeachers { get; } = new List<Teacher>();

        public List<CombinedLecture> CombinedLectures { get; set; } = new List<CombinedLecture>();

        public void Add(Lecture lecture)
        {
            Lectures.Add(lecture);
            if (!UniqueGroups.Contains(lecture.Audience))
            {
                UniqueGroups.Add(lecture.Audience);
            }

            if (!UniqueTeachers.Contains(lecture.Lecturer))
            {
                UniqueTeachers.Add(lecture.Lecturer);
            }
        }
    }
}
