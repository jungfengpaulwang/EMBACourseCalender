using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar
{
    [FISCA.UDT.TableName("ischool.course_calendar.section")]
    public class Section : FISCA.UDT.ActiveRecord
    {
        [FISCA.UDT.Field]
        public string RefCourseID { get; set; }
        [FISCA.UDT.Field]
        public DateTime StartTime { get; set; }
        [FISCA.UDT.Field]
        public DateTime EndTime { get; set; }
        [FISCA.UDT.Field]
        public string Place { get; set; }
        [FISCA.UDT.Field]
        public bool IsPublished { get; set; }
        [FISCA.UDT.Field]
        public string EventID { get; set; }
        [FISCA.UDT.Field]
        public bool Removed { get; set; }
        [FISCA.UDT.Field]
        public int RefTeacherID { get; set; }
        [FISCA.UDT.Field]
        public int RefCaseID { get; set; }
        //[FISCA.UDT.Field]
        //public string EventID2 { get; set; }
    }
}
