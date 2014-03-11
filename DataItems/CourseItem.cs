using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar.DataItems
{
    class CourseItem
    {
        public CourseItem(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
