using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleFormer.Containers
{
    public class Group
    {
        public string Name { get; set; }

        public Group(string name)
        {
            Name = name;
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

        public override string ToString()
        {
            return Name;
        }
    }
}
