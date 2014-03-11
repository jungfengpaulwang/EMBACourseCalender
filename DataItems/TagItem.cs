using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar.DataItems
{
    class TagItem
    {
        public string ID { get; set; }
        public string Prefix { get; set; }
        public string TagName { get; set; }

        public TagItem(string tagID, string prefix, string name)
        {
            this.ID = tagID;
            this.Prefix = prefix;
            this.TagName = name;                 
        }

        public String TagFullName
        {
            get { return string.Format("{0}:{1}", this.Prefix, this.TagName); }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Prefix, this.TagName);
        }

    }
}
