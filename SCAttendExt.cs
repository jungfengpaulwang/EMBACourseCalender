using FISCA.UDT;

namespace CourseCalendar
{
    [FISCA.UDT.TableName("ischool.emba.scattend_ext")]
    class SCAttendExt : ActiveRecord
    {
        /// <summary>
        /// 學生系統編號
        /// </summary>
        [Field(Field = "ref_student_id", Indexed = true)]
        public int StudentID { get; set; }

        /// <summary>
        /// 課程系統編號
        /// </summary>
        [Field(Field = "ref_course_id", Indexed = true)]
        public int CourseID { get; set; }

        /// <summary>
        /// 報告小組
        /// </summary>
        [Field(Field = "report_group", Indexed = false)]
        public string Group { get; set; }

        /// <summary>
        /// true：停修；預設：false;
        /// </summary>
        [Field(Field = "is_cancel", Indexed = false)]
        public bool IsCancel { get; set; }
    }
}