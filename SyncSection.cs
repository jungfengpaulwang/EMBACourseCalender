using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.AccessControl;
using Google.GData.Extensions;
using System.ComponentModel;
using FISCA.Permission;

namespace CourseCalendar
{
    class SyncSection
    {
        //const string googleAcc = "CourseSection@ischool.com.tw";
        //const string googlePWD = "<Cg4&YYN";
        const string googleAcc = "course@emba.ntu.edu.tw";
        const string googlePWD = "A123456&";

        public static void AddMenuButton()
        {
            var accessHelper = new FISCA.UDT.AccessHelper();
            var ribbonBarItem = K12.Presentation.NLDPanels.Course.RibbonBarItems["課程行事曆"];
            var syncButton = ribbonBarItem["同步行事曆"];

            Catalog button_syncCalendar = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_syncCalendar.Add(new RibbonFeature("Sync_Course_Calendar", "同步課程行事曆"));
            bool isEnabled = UserAcl.Current["Sync_Course_Calendar"].Executable;

            syncButton.Enable = ((K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0) && isEnabled);
            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate(object sender, EventArgs e)
            {

                syncButton.Enable = ((K12.Presentation.NLDPanels.Course.SelectedSource.Count > 0) && isEnabled);
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
                    Dictionary<string, Calendar> calendars = new Dictionary<string, Calendar>();
                    Dictionary<string, List<string>> courseAttend = new Dictionary<string, List<string>>();
                    Dictionary<string, List<Section>> publishItems = new Dictionary<string, List<Section>>();
                    Dictionary<string, string> studentLoginAccount = new Dictionary<string, string>();
                    List<string> syncCourses = new List<string>();
                    int count = 0;
                    string condition = "RefCourseID in (";
                    foreach (string key in selectedSource)
                    {
                        if (condition != "RefCourseID in (")
                            condition += ",";
                        condition += "'" + key + "'";
                    }
                    condition += ")";
                    string condition2 = "ref_course_id in (";
                    foreach (string key in selectedSource)
                    {
                        if (condition2 != "ref_course_id in (")
                            condition2 += ",";
                        condition2 += "'" + key + "'";
                    }
                    condition2 += ")";
                    bkw.ReportProgress(3);
                    foreach (Section section in accessHelper.Select<Section>(condition))
                    {
                        if (!section.IsPublished || section.Removed)
                        {
                            if (!publishItems.ContainsKey(section.RefCourseID))
                                publishItems.Add(section.RefCourseID, new List<Section>());
                            publishItems[section.RefCourseID].Add(section);
                            count++;
                        }
                    }
                    foreach (Calendar cal in accessHelper.Select<Calendar>(condition))
                    {
                        if (!calendars.ContainsKey(cal.RefCourseID))
                            calendars.Add(cal.RefCourseID, cal);
                    }
                    syncCourses.AddRange(publishItems.Keys);
                    foreach (var item in accessHelper.Select<SCAttendExt>(condition2))
                    {
                        if (!courseAttend.ContainsKey(item.CourseID.ToString()))
                            courseAttend.Add(item.CourseID.ToString(), new List<string>());
                        courseAttend[item.CourseID.ToString()].Add(item.StudentID.ToString());
                        if (!studentLoginAccount.ContainsKey(item.StudentID.ToString()))
                            studentLoginAccount.Add(item.StudentID.ToString(), "");
                        count++;
                    }
                    foreach (string key in selectedSource)
                    {
                        if (!courseAttend.ContainsKey(key))
                            courseAttend.Add(key, new List<string>());
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
                        if (calendars.ContainsKey(calid))
                        {
                            Calendar cal = calendars[calid];
                            List<string> aclList = new List<string>(cal.ACLList.Split("%".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                            List<string> attentAccounts = new List<string>();
                            foreach (string sid in courseAttend[calid])
                            {
                                if (studentLoginAccount[sid] != "")
                                    attentAccounts.Add(studentLoginAccount[sid]);
                            }
                            if (aclList.Count != attentAccounts.Count)
                            {
                                if (!syncCourses.Contains(calid))
                                    syncCourses.Add(calid);
                            }
                            else
                            {
                                foreach (string acc in aclList)
                                {
                                    if (!attentAccounts.Contains(acc.ToLower()))
                                    {
                                        if (!syncCourses.Contains(calid))
                                            syncCourses.Add(calid);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    bkw.ReportProgress(5);
                    CalendarService myService = new CalendarService("ischool.CourseCalendar");
                    myService.setUserCredentials(googleAcc, googlePWD);
                    bkw.ReportProgress(20);
                    List<Section> syncedSections = new List<Section>();
                    foreach (K12.Data.CourseRecord course in K12.Data.Course.SelectByIDs(syncCourses))
                    {
                        //CalendarEntry targetCalender = null;
                        Calendar targetCal = null;
                        try
                        {
                            if (!calendars.ContainsKey(course.ID))
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
                            }
                            else
                            {
                                targetCal = calendars[course.ID];
                            }
                        }
                        catch
                        {
                            hasFaild = true;
                        }
                        if (targetCal != null)
                        {
                            try
                            {
                                #region ACL
                                if (courseAttend.ContainsKey(course.ID))
                                {
                                    List<string> aclList = new List<string>(targetCal.ACLList.Split("%".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                                    for (int i = 0; i < aclList.Count; i++)
                                    {
                                        aclList[i] = aclList[i].ToLower();
                                    }
                                    List<string> attentAccounts = new List<string>();
                                    foreach (string sid in courseAttend[course.ID])
                                    {
                                        if (studentLoginAccount[sid] != "")
                                            attentAccounts.Add(studentLoginAccount[sid]);
                                    }
                                    foreach (string acc in attentAccounts)
                                    {
                                        if (!aclList.Contains(acc))
                                        {
                                            try
                                            {
                                                #region 新增分享
                                                AclEntry entry = new AclEntry();
                                                entry.Scope = new AclScope();
                                                entry.Scope.Type = AclScope.SCOPE_USER;
                                                entry.Scope.Value = acc;
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
                                                aclList.Add(acc);
                                            }
                                            catch
                                            {
                                                hasFaild = true;
                                            }
                                        }
                                    }
                                    List<string> removeList = new List<string>();
                                    if (aclList.Count != attentAccounts.Count)
                                    {
                                        #region 移除分享
                                        AtomFeed calFeed = myService.Query(new FeedQuery("https://www.google.com/calendar/feeds/" + targetCal.GoogleCalanderID + "/acl/full"));
                                        foreach (string acc in aclList)
                                        {
                                            if (!attentAccounts.Contains(acc))
                                            {
                                                try
                                                {
                                                    foreach (AtomEntry atomEntry in calFeed.Entries)
                                                    {
                                                        if (atomEntry is AtomEntry)
                                                        {
                                                            AclEntry aclEntry = (AclEntry)atomEntry;
                                                            if (aclEntry.Scope.Value.ToLower() == acc)
                                                            {
                                                                aclEntry.Delete();
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    removeList.Add(acc);
                                                }
                                                catch
                                                {
                                                    hasFaild = true;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    foreach (string acc in removeList)
                                    {
                                        if (aclList.Contains(acc)) aclList.Remove(acc);
                                    }
                                    targetCal.ACLList = "";
                                    foreach (string acc in aclList)
                                    {
                                        targetCal.ACLList += (targetCal.ACLList == "" ? "" : "%") + acc;
                                    }
                                }
                                #endregion
                                #region Events
                                if (publishItems.ContainsKey(course.ID))
                                {
                                    EventFeed feed = myService.Query(new EventQuery("https://www.google.com/calendar/feeds/" + targetCal.GoogleCalanderID + "/private/full"));
                                    AtomFeed batchFeed = new AtomFeed(feed);
                                    foreach (Section section in publishItems[course.ID])
                                    {
                                        if (!section.Removed)
                                        {
                                            #region 新增Event
                                            Google.GData.Calendar.EventEntry eventEntry = new Google.GData.Calendar.EventEntry();

                                            eventEntry.Title.Text = course.Name;
                                            //eventEntry
                                            Where eventLocation = new Where();
                                            eventLocation.ValueString = section.Place;
                                            eventEntry.Locations.Add(eventLocation);
                                            eventEntry.Notifications = true;
                                            eventEntry.Times.Add(new When(section.StartTime, section.EndTime));
                                            eventEntry.Participants.Add(new Who()
                                            {
                                                ValueString = googleAcc,
                                                Attendee_Type = new Who.AttendeeType() { Value = Who.AttendeeType.EVENT_REQUIRED },
                                                Attendee_Status = new Who.AttendeeStatus() { Value = Who.AttendeeStatus.EVENT_ACCEPTED },
                                                Rel = Who.RelType.EVENT_ATTENDEE
                                            });
                                            eventEntry.BatchData = new GDataBatchEntryData(section.UID, GDataBatchOperationType.insert);
                                            batchFeed.Entries.Add(eventEntry);
                                            #endregion
                                        }
                                        else
                                        {
                                            #region 刪除Event

                                            EventEntry toDelete = (EventEntry)feed.Entries.FindById(new AtomId(feed.Id.AbsoluteUri + "/" + section.EventID));
                                            if (toDelete != null)
                                            {
                                                toDelete.Id = new AtomId(toDelete.EditUri.ToString());
                                                toDelete.BatchData = new GDataBatchEntryData(section.UID, GDataBatchOperationType.delete);
                                                batchFeed.Entries.Add(toDelete);
                                            }
                                            else
                                            {
                                                section.Deleted = true;
                                                syncedSections.Add(section);
                                            }
                                            #endregion
                                        }
                                        int p = syncedSections.Count * 80 / count + 20;
                                        if (p > 100) p = 100;
                                        if (p < 0) p = 0;
                                        bkw.ReportProgress(p);
                                    }
                                    EventFeed batchResultFeed = (EventFeed)myService.Batch(batchFeed, new Uri(feed.Batch));
                                    foreach (Section section in publishItems[course.ID])
                                    {
                                        if (syncedSections.Contains(section)) continue;
                                        #region 儲存Section狀態
                                        bool match = false;
                                        if (section.Removed)
                                        {
                                            foreach (EventEntry entry in batchResultFeed.Entries)
                                            {
                                                if (entry.BatchData.Status.Code == 200)
                                                {
                                                    if (section.UID == entry.BatchData.Id)
                                                    {
                                                        section.Deleted = true;
                                                        match = true;
                                                        syncedSections.Add(section);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (EventEntry entry in batchResultFeed.Entries)
                                            {
                                                if (entry.BatchData.Status.Code == 201)
                                                {
                                                    if (section.UID == entry.BatchData.Id)
                                                    {
                                                        section.IsPublished = true;
                                                        match = true;
                                                        section.EventID = entry.EventId;
                                                        syncedSections.Add(section);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (!match)
                                            hasFaild = true;
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                            catch
                            {
                                hasFaild = true;
                            }
                            targetCal.Save();
                        }
                    }
                    syncedSections.SaveAll();
                };
                bkw.RunWorkerAsync();
            };
        }
    }
}
