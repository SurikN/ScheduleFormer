using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleFormer.Containers
{
    public class Teacher
    {
        public string Name { get; set; }

        public Teacher(string name)
        {
            Name = name;
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

        public override string ToString()
        {
            return Name;
        }
    }
}
