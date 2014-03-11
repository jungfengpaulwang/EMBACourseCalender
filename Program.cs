using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FISCA.Permission;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.AccessControl;
using Google.GData.Extensions;
using System.ComponentModel;
using Aspose.Cells;
using System.IO;
using FISCA.UDT;

namespace CourseCalendar
{
    public static class Program
    {
        const string googleAcc = "course@emba.ntu.edu.tw";
        const string googlePWD = "A123456&";
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [FISCA.MainMethod]
        public static void Main()
        {
            if (UserAcl.Current[typeof(SectionDetail)].Viewable)
                K12.Presentation.NLDPanels.Course.AddDetailBulider<SectionDetail>();

            Catalog detail = RoleAclSource.Instance["課程"]["資料項目"];
            detail.Add(new DetailItemFeature(typeof(SectionDetail)));

            SectionSyncColumn.SetupColumn();
            SyncSection.AddMenuButton();
            ResetCalendar.AddMenuButton();

            //return;
            //這是另一個隱藏版功能，需要製做牧牛杖
            var accessHelper = new FISCA.UDT.AccessHelper();
            #region 重建ACLlist
            var ribbonBarItem = K12.Presentation.NLDPanels.Course.RibbonBarItems["課程行事曆"];
            var syncButton = ribbonBarItem["重建修課學生同步狀態"];
            syncButton.Enable = ((K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0));
            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate(object sender, EventArgs e)
            {

                syncButton.Enable = ((K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0));
            };
            syncButton.Click += delegate
            {
                bool hasFaild = false;
                FISCA.Presentation.MotherForm.SetStatusBarMessage("課程行事曆同步中...", 0);
                List<string> selectedSource = new List<string>(K12.Presentation.NLDPanels.Course.SelectedSource);
                BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker() { WorkerReportsProgress = true };
                bkw.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("課程行事曆同步中...", e.ProgressPercentage);
                };
                bkw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    SectionSyncColumn.Reload();
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("課程行事曆同步完成");
                    if (hasFaild)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("課程行事曆同步完成，其中部分資料同步失敗，請稍後再試。");
                    }
                };
                bkw.DoWork += delegate(object sender, DoWorkEventArgs e)
                {
                    string condition = "RefCourseID in (";
                    foreach (string key in selectedSource)
                    {
                        if (condition != "RefCourseID in (")
                            condition += ",";
                        condition += "'" + key + "'";
                    }
                    condition += ")";
                    bkw.ReportProgress(3);
                    CalendarService myService = new CalendarService("ischool.CourseCalendar");
                    var calendarRecords = accessHelper.Select<Calendar>(condition);
                    myService.setUserCredentials(googleAcc, googlePWD);
                    foreach (Calendar targetCal in calendarRecords)
                    {
                        try
                        {
                            List<string> aclList = new List<string>();
                            AtomFeed calFeed = myService.Query(new FeedQuery("https://www.google.com/calendar/feeds/" + targetCal.GoogleCalanderID + "/acl/full"));
                            foreach (AtomEntry atomEntry in calFeed.Entries)
                            {
                                if (atomEntry is AtomEntry)
                                {
                                    AclEntry aclEntry = (AclEntry)atomEntry;
                                    if (aclEntry.Scope.Type == AclScope.SCOPE_USER && aclEntry.Role.Value == AclRole.ACL_CALENDAR_READ.Value)
                                        aclList.Add(aclEntry.Scope.Value);
                                    //if (aclEntry.Scope.Value == acc)
                                    //{
                                    //    aclEntry.Delete();
                                    //}
                                }
                            }
                            targetCal.ACLList = "";
                            foreach (string acc in aclList)
                            {
                                targetCal.ACLList += (targetCal.ACLList == "" ? "" : "%") + acc;
                            }
                        }
                        catch
                        {
                            hasFaild = true;
                        }
                    }
                    calendarRecords.SaveAll();
                    bkw.ReportProgress(20);
                };
                bkw.RunWorkerAsync();
            };
            #endregion
            #region 產生ACL清單
            var syncButton2 = ribbonBarItem["列出行事曆現況"];
            syncButton2.Enable = ((K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0));
            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate(object sender, EventArgs e)
            {
                syncButton2.Enable = ((K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0));
            };
            syncButton2.Click += delegate
            {
                bool hasFaild = false;
                FISCA.Presentation.MotherForm.SetStatusBarMessage("課程行事曆現況產生中...", 0);
                List<string> selectedSource = new List<string>(K12.Presentation.NLDPanels.Course.SelectedSource);
                BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker() { WorkerReportsProgress = true };
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
                wb.Worksheets[0].Cells[0, 0].PutValue("課程編號");
                wb.Worksheets[0].Cells[0, 1].PutValue("課程名稱");
                wb.Worksheets[0].Cells[0, 2].PutValue("修課學生帳號");
                wb.Worksheets[0].Cells[0, 3].PutValue("系統註記分享帳號");
                wb.Worksheets[0].Cells[0, 4].PutValue("行事曆上實際分享帳號");
                wb.Worksheets[0].Cells[0, 5].PutValue("修課學生學號");
                wb.Worksheets[0].Cells[0, 6].PutValue("修課學生姓名");
                bkw.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("課程行事曆現況產生中...", e.ProgressPercentage);
                };
                bkw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    SectionSyncColumn.Reload();
                    Completed("課程行事曆現況", wb);
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("課程行事曆現況產生完成");
                    if (hasFaild)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("課程行事曆現況產生完成，其中部分資料發生錯誤，請稍後再試。");
                    }
                };
                bkw.DoWork += delegate(object sender, DoWorkEventArgs e)
                {
                    int rowIndex = 1;
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
                    Dictionary<string, List<string>> courseAttend = new Dictionary<string, List<string>>();
                    string condition2 = "ref_course_id in (";
                    foreach (string key in selectedSource)
                    {
                        if (condition2 != "ref_course_id in (")
                            condition2 += ",";
                        condition2 += "'" + key + "'";
                    }
                    condition2 += ")";
                    foreach (var item in accessHelper.Select<SCAttendExt>(condition2))
                    {
                        if (!courseAttend.ContainsKey(item.CourseID.ToString()))
                            courseAttend.Add(item.CourseID.ToString(), new List<string>());
                        courseAttend[item.CourseID.ToString()].Add(item.StudentID.ToString());
                    }
                    bkw.ReportProgress(3);
                    CalendarService myService = new CalendarService("ischool.CourseCalendar");
                    //var calendarRecords = accessHelper.Select<Calendar>(condition);
                    myService.setUserCredentials(googleAcc, googlePWD);
                    Dictionary<string, K12.Data.StudentRecord> studentMail = new Dictionary<string, K12.Data.StudentRecord>();
                    foreach (K12.Data.CourseRecord courseRec in K12.Data.Course.SelectByIDs(selectedSource))
                    {
                        List<string> realACLList = new List<string>();
                        List<string> markedACLList = new List<string>();
                        List<string> attendACLList = new List<string>();
                        if (courseAttend.ContainsKey(courseRec.ID))
                        {
                            foreach (var student in K12.Data.Student.SelectByIDs(courseAttend[courseRec.ID]))
                            {
                                if (student.SALoginName != "")
                                {
                                    attendACLList.Add(student.SALoginName);
                                    if (!studentMail.ContainsKey(student.SALoginName.ToLower()))
                                    {
                                        studentMail.Add(student.SALoginName.ToLower(), student);
                                    }
                                }
                            }
                        }
                        if (courseCalendar.ContainsKey(courseRec.ID))
                        {
                            Calendar targetCal = courseCalendar[courseRec.ID];
                            markedACLList.AddRange(targetCal.ACLList.Split("%".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                            try
                            {
                                AtomFeed calFeed = myService.Query(new FeedQuery("https://www.google.com/calendar/feeds/" + targetCal.GoogleCalanderID + "/acl/full"));
                                foreach (AtomEntry atomEntry in calFeed.Entries)
                                {
                                    if (atomEntry is AtomEntry)
                                    {
                                        AclEntry aclEntry = (AclEntry)atomEntry;
                                        if (aclEntry.Scope.Type == AclScope.SCOPE_USER && aclEntry.Role.Value == AclRole.ACL_CALENDAR_READ.Value)
                                            realACLList.Add(aclEntry.Scope.Value);
                                    }
                                }
                            }
                            catch
                            {
                                hasFaild = true;
                            }
                        }
                        if (realACLList.Count == 0 && markedACLList.Count == 0 && attendACLList.Count == 0)
                        {
                            wb.Worksheets[0].Cells[rowIndex, 0].PutValue(courseRec.ID);
                            wb.Worksheets[0].Cells[rowIndex, 1].PutValue(courseRec.Name);
                            rowIndex++;
                        }
                        else
                        {
                            foreach (string mail in attendACLList)
                            {
                                string realMail = "";
                                string markedMail = "";
                                foreach (var item in realACLList)
                                {
                                    if (item.ToLower() == mail.ToLower())
                                    {
                                        realMail = item;
                                        break;
                                    }
                                }
                                foreach (var item in markedACLList)
                                {
                                    if (item.ToLower() == mail.ToLower())
                                    {
                                        markedMail = item;
                                        break;
                                    }
                                }
                                wb.Worksheets[0].Cells[rowIndex, 0].PutValue(courseRec.ID);
                                wb.Worksheets[0].Cells[rowIndex, 1].PutValue(courseRec.Name);
                                wb.Worksheets[0].Cells[rowIndex, 2].PutValue(mail);
                                if (markedMail != "") wb.Worksheets[0].Cells[rowIndex, 3].PutValue(markedMail);
                                if (realMail != "") wb.Worksheets[0].Cells[rowIndex, 4].PutValue(realMail);
                                if (studentMail.ContainsKey(mail.ToLower()))
                                {
                                    wb.Worksheets[0].Cells[rowIndex, 5].PutValue(studentMail[mail.ToLower()].StudentNumber);
                                    wb.Worksheets[0].Cells[rowIndex, 6].PutValue(studentMail[mail.ToLower()].Name);
                                }
                                if (realMail != "") realACLList.Remove(realMail);
                                if (markedMail != "") markedACLList.Remove(markedMail);
                                rowIndex++;
                            }
                            foreach (string mail in markedACLList)
                            {
                                string realMail = "";
                                foreach (var item in realACLList)
                                {
                                    if (item.ToLower() == mail.ToLower())
                                    {
                                        realMail = item;
                                        break;
                                    }
                                }
                                wb.Worksheets[0].Cells[rowIndex, 0].PutValue(courseRec.ID);
                                wb.Worksheets[0].Cells[rowIndex, 1].PutValue(courseRec.Name);
                                //wb.Worksheets[0].Cells[rowIndex, 2].PutValue("");
                                wb.Worksheets[0].Cells[rowIndex, 3].PutValue(mail);
                                if (realMail != "") wb.Worksheets[0].Cells[rowIndex, 4].PutValue(realMail);
                                if (studentMail.ContainsKey(mail.ToLower()))
                                {
                                    wb.Worksheets[0].Cells[rowIndex, 5].PutValue(studentMail[mail.ToLower()].StudentNumber);
                                    wb.Worksheets[0].Cells[rowIndex, 6].PutValue(studentMail[mail.ToLower()].Name);
                                }
                                if (realMail != "") realACLList.Remove(realMail);
                                rowIndex++;
                            }
                            foreach (string mail in realACLList)
                            {
                                wb.Worksheets[0].Cells[rowIndex, 0].PutValue(courseRec.ID);
                                wb.Worksheets[0].Cells[rowIndex, 1].PutValue(courseRec.Name);
                                //wb.Worksheets[0].Cells[rowIndex, 2].PutValue("");
                                //wb.Worksheets[0].Cells[rowIndex, 3].PutValue("");
                                wb.Worksheets[0].Cells[rowIndex, 4].PutValue(mail);
                                if (studentMail.ContainsKey(mail.ToLower()))
                                {
                                    wb.Worksheets[0].Cells[rowIndex, 5].PutValue(studentMail[mail.ToLower()].StudentNumber);
                                    wb.Worksheets[0].Cells[rowIndex, 6].PutValue(studentMail[mail.ToLower()].Name);
                                }
                                rowIndex++;
                            }
                            //if (realACLList.Count > 0)
                            //{
                            //    foreach (var acc in realACLList)
                            //    {
                            //        wb.Worksheets[0].Cells[rowIndex, 0].PutValue(courseRec.ID);
                            //        wb.Worksheets[0].Cells[rowIndex, 1].PutValue(courseRec.Name);
                            //        wb.Worksheets[0].Cells[rowIndex, 2].PutValue(acc);
                            //        rowIndex++;
                            //    }
                            //}
                            //else
                            //{
                            //    wb.Worksheets[0].Cells[rowIndex, 0].PutValue(courseRec.ID);
                            //    wb.Worksheets[0].Cells[rowIndex, 1].PutValue(courseRec.Name);
                            //    rowIndex++;
                            //}
                        }
                    }
                    bkw.ReportProgress(20);
                };
                bkw.RunWorkerAsync();
            };
            #endregion
        }
        private static void Completed(string inputReportName, Workbook inputWorkbook)
        {
            string reportName = inputReportName;

            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xls");

            Workbook wb = inputWorkbook;

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                wb.Save(path, FileFormatType.Excel2003);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, FileFormatType.Excel2003);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

    }
}
