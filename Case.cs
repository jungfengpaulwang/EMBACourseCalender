using System;
using FISCA.UDT;

namespace CourseCalendar
{
    /// <summary>
    /// 個案
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.case_management.case")]
    public class Case : ActiveRecord
    {
        /// <summary>
        /// 個案中文名稱
        /// </summary>
        [Field(Field = "name", Indexed = true)]
        public string Name { get; set; }

        /// <summary>
        /// 個案英文名稱
        /// </summary>
        [Field(Field = "english_name", Indexed = false)]
        public string EnglishName { get; set; }

        /// <summary>
        /// 個案文件連結
        /// </summary>
        [Field(Field = "url", Indexed = false)]
        public string URL { get; set; }

        /// <summary>
        /// 淺層複製物件
        /// </summary>
        /// <returns></returns>
        public Case Clone()
        {
            return this.MemberwiseClone() as Case;
        }
    }
}
