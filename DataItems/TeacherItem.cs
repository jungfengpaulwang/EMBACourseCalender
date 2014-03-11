using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar.DataItems
{
    class TeacherItem
    {
        public TeacherItem(string id, string name, string status)
        {
            ID = id;
            Name = name;
            this.Status = status;
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public bool IsDeleted
        {
            get { return this.Status == "256" ;}
        }

        public override string ToString()
        {
            string result = this.Name ;
            if (this.IsDeleted)
                result = string.Format("{0}--*", this.Name);
            return result;
        }
    }
}
