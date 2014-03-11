using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar
{

    [FISCA.UDT.TableName("ischool.course_calendar.calendar")]
    public class Calendar : FISCA.UDT.ActiveRecord
    {
        public Calendar()
        {
            RefCourseID = GoogleCalanderID = Color = ACLList = "";
        }
        [FISCA.UDT.Field]
        public string RefCourseID { get; set; }
        [FISCA.UDT.Field]
        public string GoogleCalanderID { get; set; }
        [FISCA.UDT.Field]
        public string Color { get; set; }
        [FISCA.UDT.Field]
        public string ACLList { get; set; }
    }
}
