using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleFormer.Containers;
using ScheduleFormer.Structures;

namespace ScheduleFormer.Models
{
    public static class LecturesStorageModel
    {
        public static List<Lecture> Lectures { get; } = new List<Lecture>();

        public static List<Group> UniqueGroups { get; } = new List<Group>();

        public static List<Teacher> UniqueTeachers { get; } = new List<Teacher>();

        public static void Add(Lecture lecture)
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
