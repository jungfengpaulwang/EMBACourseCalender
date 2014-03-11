using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar.DataItems
{
    public class ClassItem
    {
        public ClassItem(string id, string name, string schoolYear)
        {
            ID = id;
            Name = name;
            this.SchoolYear = schoolYear;
        }

        public string ID { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 入學年度，對應到 grade_year 欄位
        /// </summary>
        public string SchoolYear { get; set; }
    }
}
