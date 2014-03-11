using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.AccessControl;
using Google.GData.Extensions;
using System.ComponentModel;
using FISCA.UDT;
using FISCA.Permission;

namespace CourseCalendar
{
    class SyncSCAttend
    {
        public static void AddMenuButton()
        {
            //string googleAcc = "CourseSection@ischool.com.tw", googlePWD = "<Cg4&YYN";
            string googleAcc = "course@emba.ntu.edu.tw", googlePWD = "A123456&";

            var accessHelper = new FISCA.UDT.AccessHelper();
            var ribbonBarItem = K12.Presentation.NLDPanels.Course.RibbonBarItems["課程行事曆"];
            var syncButton = ribbonBarItem["同步修課學生"];


            Catalog button_syncCalendar = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_syncCalendar.Add(new RibbonFeature("Sync_Course_Calendar_Student", "同步修課學生"));
            bool isEnabled = UserAcl.Current["Sync_Course_Calendar_Student"].Executable;

            syncButton.Enable = isEnabled;
            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate(object sender, EventArgs e)
            {
                syncButton.Enable = ((K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0) && isEnabled);
            };


            //syncButton.Enable = false;
            //K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate(object sender, EventArgs e)
            //{
            //    syncButton.Enable = K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0;
            //};
            syncButton.Click += delegate
            {
                bool hasFaild = false;
                FISCA.Presentation.MotherForm.SetStatusBarMessage("修課學生行事曆同步中...", 0);
                List<string> selectedSource = new List<string>(K12.Presentation.NLDPanels.Course.SelectedSource);
                BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker() { WorkerReportsProgress = true };
                bkw.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("修課學生行事曆同步中...", e.ProgressPercentage);
                };
                bkw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    SectionSyncColumn.Reload();
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("修課學生行事曆同步完成");
                    if (hasFaild)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("修課學生行事曆同步完成，其中部分資同步失敗，請稍後再試。");
                    }
                };
                bkw.DoWork += delegate
                {
                    int count = 0;
                    int syncedSections = 0;
                    Dictionary<string, List<string>> courseAttend = new Dictionary<string, List<string>>();
                    
                    //foreach (var item in K12.Data.SCAttend.SelectByCourseIDs(selectedSource))

                    AccessHelper helper = new AccessHelper();
                    string condition2 = "ref_course_id in (";
                    foreach (string key in selectedSource)
                    {
                        if (condition2 != "ref_course_id in (")
                            condition2 += ",";
                        condition2 += "'" + key + "'";
                    }
                    condition2 += ")";

                    foreach (var item in helper.Select<SCAttendExt>(condition2))
                    {
                        if (!courseAttend.ContainsKey(item.CourseID.ToString()))
                            courseAttend.Add(item.CourseID.ToString(), new List<string>());
                        courseAttend[item.CourseID.ToString()].Add(item.StudentID.ToString());
                        count++;
                    }

                    Dictionary<string, Calendar> courseCalendar = new Dictionary<string, Calendar>();
                    string condition = "RefCourseID in (";
                    foreach (string key in selectedSource)
                    {
                        if (condition != "RefCourseID in (")
                            condition += ",";
                        condition += "'" + key + "'";
                    }
                    condition += ")";
                    foreach (Calendar cal in accessHelper.Select<Calendar>(condition))
                    {
                        if (!courseCalendar.ContainsKey(cal.RefCourseID))
                            courseCalendar.Add(cal.RefCourseID, cal);
                    }
                    bkw.ReportProgress(5);
                    CalendarService myService = new CalendarService("ischool.CourseCalendar");
                    myService.setUserCredentials(googleAcc, googlePWD);
                    bkw.ReportProgress(20);
                    foreach (K12.Data.CourseRecord course in K12.Data.Course.SelectByIDs(courseAttend.Keys))
                    {
                        Calendar targetCal = null;
                        try
                        {
                            if (!courseCalendar.ContainsKey(course.ID))
                            {
                                #region 建立新Calender
                                string[] colorLists = new string[]{"#A32929","#B1365F","#7A367A","#5229A3","#29527A","#2952A3","#1B887A",
                            "#28754E","#0D7813","#528800","#88880E","#AB8B00","#BE6D00","#B1440E",
                            "#865A5A","#705770","#4E5D6C","#5A6986","#4A716C","#6E6E41","#8D6F47"};
                                CalendarEntry newCal = new CalendarEntry();
                                newCal.Title.Text = course.Name;
                                newCal.Summary.Text = "科目：" + course.Subject
                                    + "\n學年度：" + course.SchoolYear
                                    + "\n學期：" + course.Semester
                                    + "\n學分數：" + course.Credit;
                                newCal.TimeZone = "Asia/Taipei";
                                //targetCalender.Hidden = false;
                                newCal.Color = colorLists[new Random(DateTime.Now.Millisecond).Next(0, colorLists.Length)];
                                Uri postUri = new Uri("http://www.google.com/calendar/feeds/default/owncalendars/full");
                                newCal = (CalendarEntry)myService.Insert(postUri, newCal);
                                #endregion
                                String calendarURI = newCal.Id.Uri.ToString();
                                String calendarID = calendarURI.Substring(calendarURI.LastIndexOf("/") + 1);
                                targetCal = new Calendar() { RefCourseID = course.ID, GoogleCalanderID = calendarID };
                                targetCal.Save();
                                courseCalendar.Add(course.ID, targetCal);
                            }
                            else
                            {
                                targetCal = courseCalendar[course.ID];
                            }
                        }
                        catch { hasFaild = true; }
                        if (targetCal != null)
                        {
                            List<string> aclList = new List<string>(targetCal.ACLList.Split("%".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                            foreach (var student in K12.Data.Student.SelectByIDs(courseAttend[course.ID]))
                            {
                                try
                                {
                                    if (student.SALoginName != "" && !aclList.Contains(student.SALoginName))
                                    {
                                        #region 新增分享
                                        AclEntry entry = new AclEntry();
                                        entry.Scope = new AclScope();
                                        entry.Scope.Type = AclScope.SCOPE_USER;
                                        entry.Scope.Value = student.SALoginName;
                                        entry.Role = AclRole.ACL_CALENDAR_READ;
                                        try
                                        {
                                            AclEntry insertedEntry = myService.Insert(new Uri("https://www.google.com/calendar/feeds/" + targetCal.GoogleCalanderID + "/acl/full"), entry);
                                        }
                                        catch (GDataRequestException gex)
                                        {
                                            if (!gex.InnerException.Message.Contains("(409)"))
                                                throw;
                                        }
                                        #endregion
                                        aclList.Add(student.SALoginName);
                                        targetCal.ACLList += (targetCal.ACLList == "" ? "" : "%") + student.SALoginName;
                                    }
                                }
                                catch
                                {
                                    hasFaild = true;
                                }
                                syncedSections++;
                                int p = syncedSections * 80 / count + 20;
                                if (p > 100) p = 100;
                                if (p < 0) p = 0;
                                bkw.ReportProgress(p);
                            }
                        }
                    }
                    courseCalendar.Values.SaveAll();
                };
                bkw.RunWorkerAsync();
            };
        }
    }
}
