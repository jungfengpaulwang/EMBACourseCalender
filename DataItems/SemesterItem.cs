using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar.DataItems
{
    public class SemesterItem
    {
        private SemesterItem(string value, string Name)
        {
            this.Name = Name;
            this.Value = value;
        }
        public string Value { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return this.Name;
        }

        private static Dictionary<string, SemesterItem> items;
        public static List<SemesterItem> GetSemesterList()
        {
            if (items == null)
            {
                items = new Dictionary<string, SemesterItem>();
                items.Add("0", new SemesterItem("0", "夏季學期"));
                items.Add("1", new SemesterItem("1", "第1學期"));
                items.Add("2", new SemesterItem("2", "第2學期"));
            }
            return items.Values.ToList<SemesterItem>();
        }

        /// <summary>
        /// 根據代碼取得學期物件。
        /// EMBA 可能的學期代碼值為： 0 (暑假), 1:上學期  , 2:下學期
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SemesterItem GetSemesterByCode(string code)
        {
            SemesterItem result = null;
            if (items == null)
                GetSemesterList();

            if (items.ContainsKey(code))
                result = items[code];

            return result;
        }

        /// <summary>
        /// 根據學期別取得學期物件。
        /// EMBA 可能的學期別為： 夏季學期, 第1學期  , 第2學期
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SemesterItem GetSemesterByName(string name)
        {
            SemesterItem result = null;
            if (items == null)
                GetSemesterList();

            foreach (SemesterItem semester in items.Values)
            {
                if (name == semester.Name)
                    return semester;
            }

            return result;
        }
    }

}
