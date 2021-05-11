using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleFormer.Containers
{
    public class CombinedLecture : Lecture
    {
        public int Quantity { get; set; }

        public CombinedLecture(Lecture lecture, int qunatity) : base(lecture)
        {
            Quantity = qunatity;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is CombinedLecture combinedLecture)
            {
                return combinedLecture.Name == Name && combinedLecture.Lecturer.Equals(Lecturer) &&
                       combinedLecture.Audience.Equals(Audience) && combinedLecture.Quantity == Quantity;
            }

            if (obj is Lecture lecture)
            {
                return lecture.Name == Name && lecture.Lecturer.Equals(Lecturer) &&
                       lecture.Audience.Equals(Audience);
            }

            return false;
        }
    }
}
