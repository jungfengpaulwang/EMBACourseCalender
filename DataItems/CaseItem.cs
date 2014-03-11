using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar.DataItems
{
    public class CaseItem
    {
        public CaseItem(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}
