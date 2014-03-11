using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseCalendar
{
    static class SectionSyncColumn
    {
        static private Dictionary<string, bool> _Values = new Dictionary<string, bool>();
        static private FISCA.UDT.AccessHelper _AccessHelper = new FISCA.UDT.AccessHelper();
        static private FISCA.Presentation.ListPaneField _ListPaneField = new FISCA.Presentation.ListPaneField("同步行事曆");
        static public void SetupColumn()
        {
            K12.Presentation.NLDPanels.Course.AddListPaneField(_ListPaneField);

            _ListPaneField.PreloadVariableBackground += delegate(object sender, FISCA.Presentation.PreloadVariableEventArgs e)
            {
                _Values.Clear();
                Dictionary<string, bool> values = new Dictionary<string, bool>();

                string condition = "RefCourseID in (";
                foreach (string key in e.Keys)
                {
                    if (condition != "RefCourseID in (")
                        condition += ",";
                    condition += "'" + key + "'";
                }
                condition += ")";

                string condition2 = "ref_course_id in (";
                foreach (string key in e.Keys)
                {
                    if (condition2 != "ref_course_id in (")
                        condition2 += ",";
                    condition2 += "'" + key + "'";
                }
                condition2 += ")";

                if (e.Keys.Length > 0)
                {
                    #region eventStatus
                    foreach (Section section in _AccessHelper.Select<Section>(condition))
                    {
                        if (!values.ContainsKey(section.RefCourseID))
                            values.Add(section.RefCourseID, true);
                        if (!section.IsPublished || section.Removed)
                            values[section.RefCourseID] = false;
                    }
                    #endregion
                    #region shardStatus
                    Dictionary<string, Calendar> courseCalendar = new Dictionary<string, Calendar>();
                    foreach (Calendar cal in _AccessHelper.Select<Calendar>(condition))
                    {
                        if (!courseCalendar.ContainsKey(cal.RefCourseID))
                            courseCalendar.Add(cal.RefCourseID, cal);
                    }
                    Dictionary<string, List<string>> courseAttend = new Dictionary<string, List<string>>();
                    Dictionary<string, string> studentLoginAccount = new Dictionary<string, string>();
                    foreach (var item in _AccessHelper.Select<SCAttendExt>(condition2))
                    {
                        if (!courseAttend.ContainsKey(item.CourseID.ToString()))
                            courseAttend.Add(item.CourseID.ToString(), new List<string>());
                        courseAttend[item.CourseID.ToString()].Add(item.StudentID.ToString());
                        if (!studentLoginAccount.ContainsKey(item.StudentID.ToString()))
                            studentLoginAccount.Add(item.StudentID.ToString(), "");
                    }
                    foreach (var student in K12.Data.Student.SelectByIDs(studentLoginAccount.Keys))
                    {
                        if (student.SALoginName != "")
                        {
                            studentLoginAccount[student.ID] = student.SALoginName.ToLower();
                        }
                    }
                    foreach (string calid in courseAttend.Keys)
                    {
                        if (courseCalendar.ContainsKey(calid))
                        {
                            Calendar cal = courseCalendar[calid];
                            List<string> aclList = new List<string>(cal.ACLList.Split("%".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                            List<string> attentAccounts = new List<string>();
                            foreach (string sid in courseAttend[calid])
                            {
                                if (studentLoginAccount[sid] != "")
                                    attentAccounts.Add(studentLoginAccount[sid]);
                            }
                            if (aclList.Count != attentAccounts.Count)
                            {
                                if (!values.ContainsKey(calid))
                                    values.Add(calid, false);
                                else
                                    values[calid] = false;
                            }
                            else
                            {
                                foreach (string acc in aclList)
                                {
                                    if (!attentAccounts.Contains(acc.ToLower()))
                                    {
                                        if (!values.ContainsKey(calid))
                                            values.Add(calid, false);
                                        else
                                            values[calid] = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                _Values = values;
            };
            _ListPaneField.GetVariable += delegate(object sender, FISCA.Presentation.GetVariableEventArgs e)
            {
                if (_Values.ContainsKey(e.Key))
                {
                    e.Value = _Values[e.Key] ? "已同步" : "未同步";
                }
                else
                {
                    e.Value = "未設上課時間";
                }
            };
        }
        static public void Reload()
        {
            _ListPaneField.Reload();
        }
    }
}
