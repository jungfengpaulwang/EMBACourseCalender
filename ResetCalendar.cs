using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.AccessControl;
using Google.GData.Extensions;
using FISCA.Permission;

namespace CourseCalendar
{
    class ResetCalendar
    {
        //const string googleAcc = "CourseSection@ischool.com.tw";
        //const string googlePWD = "<Cg4&YYN";
        const string googleAcc = "course@emba.ntu.edu.tw";
        const string googlePWD = "A123456&";


        public static void AddMenuButton()
        {
            var ribbonBarItem = K12.Presentation.NLDPanels.Course.RibbonBarItems["課程行事曆"];

            Catalog button_syncCalendar = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_syncCalendar.Add(new RibbonFeature("Reset_Course_Calendar", "重置課程行事曆"));
            bool isEnabled = UserAcl.Current["Reset_Course_Calendar"].Executable;
            var btn = ribbonBarItem["重置課程行事曆"];
            if (isEnabled)
            {
                ribbonBarItem["重置課程行事曆"].Click += delegate
                {
                    if (System.Windows.Forms.MessageBox.Show("將會清空行事例曆中所有資料，\n以及系統內課程同步狀態。\n\nPS.不會影響輸入的上課時間資料。", "重置課程行事曆", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                    {
                        System.ComponentModel.BackgroundWorker bkw = new System.ComponentModel.BackgroundWorker();
                        bkw.WorkerReportsProgress = true;
                        bkw.RunWorkerCompleted += delegate
                        {
                            FISCA.Presentation.MotherForm.SetStatusBarMessage("重置課程行事曆完成。", 100);
                            System.Windows.Forms.MessageBox.Show("課程行事曆已重置，\n請上google calendar檢查，\n如有殘留資料請再執行此功能。");
                        };
                        bkw.ProgressChanged += delegate(object sender, System.ComponentModel.ProgressChangedEventArgs e)
                        {
                            FISCA.Presentation.MotherForm.SetStatusBarMessage("重置課程行事曆...", e.ProgressPercentage);
                        };
                        bkw.DoWork += delegate
                        {
                            bkw.ReportProgress(1);
                            var accessHelper = new FISCA.UDT.AccessHelper();
                            var l1 = accessHelper.Select<Section>();
                            foreach (Section section in l1)
                            {
                                section.IsPublished = false;
                            }
                            l1.SaveAll();
                            bkw.ReportProgress(5);
                            var l2 = accessHelper.Select<Calendar>();
                            foreach (Calendar cal in l2)
                            {
                                cal.Deleted = true;
                            }
                            l2.SaveAll();
                            bkw.ReportProgress(10);
                            #region 清空行事曆
                            CalendarService myService = new CalendarService("ischool.CourseCalendar");
                            myService.setUserCredentials(googleAcc, googlePWD);
                            CalendarQuery cq = new CalendarQuery();
                            cq.Uri = new Uri("http://www.google.com/calendar/feeds/default/owncalendars/full");
                            CalendarFeed resultFeed = myService.Query(cq);
                            foreach (CalendarEntry entry in resultFeed.Entries)
                            {
                                String calendarURI = entry.Id.Uri.ToString();
                                String calendarID = calendarURI.Substring(calendarURI.LastIndexOf("/") + 1);
                                clearAndDeleteCalender(calendarID);
                            }
                            bkw.ReportProgress(55);
                            deleteAllEvent("default");
                            #endregion
                            bkw.ReportProgress(100);
                        };
                        bkw.RunWorkerAsync();
                    }
                };
            }
            else
            {
                btn.Enable = false;
            }
        }

        #region 一大堆有得沒有的calendar同步功能
        private static void clearAndDeleteCalender(string calID)
        {
            deleteAllEvent(calID);
            CalendarService myService = new CalendarService("ischool.CourseCalendar");
            myService.setUserCredentials(googleAcc, googlePWD);
            //EventFeed feed = myService.Query(new EventQuery("https://www.google.com/calendar/feeds/" + calID + "/private/full"));
            CalendarFeed resultFeed = (CalendarFeed)myService.Query(new CalendarQuery("https://www.google.com/calendar/feeds/default/owncalendars/full"));

            foreach (CalendarEntry entry in resultFeed.Entries)
            {
                //Console.WriteLine("Deleting calendar: " + entry.Title.Text + "\n");
                try
                {
                    //entry.Delete();
                    if (entry.Id.AbsoluteUri.Contains(calID))
                        entry.Delete();
                }
                catch (GDataRequestException)
                {
                    //Console.WriteLine("Unable to delete primary calendar.\n");
                }
            }

        }
        private static void deleteAllEvent(string calID)
        {
            #region Delete
            {
                CalendarService myService = new CalendarService("ischool.CourseCalendar");
                myService.setUserCredentials(googleAcc, googlePWD);
                EventFeed feed = myService.Query(new EventQuery("https://www.google.com/calendar/feeds/" + calID + "/private/full"));
                AtomFeed batchFeed = new AtomFeed(feed);
                foreach (EventEntry toDelete in feed.Entries)
                {
                    if (toDelete != null)
                    {
                        toDelete.Id = new AtomId(toDelete.EditUri.ToString());
                        toDelete.BatchData = new GDataBatchEntryData("C", GDataBatchOperationType.delete);
                        batchFeed.Entries.Add(toDelete);
                    }
                }
                EventFeed batchResultFeed = (EventFeed)myService.Batch(batchFeed, new Uri(feed.Batch));
                foreach (EventEntry entry in batchResultFeed.Entries)
                {
                    if (entry.BatchData.Status.Code == 200)
                    {
                        //break;
                    }
                }
            }
            #endregion
        }
        private static void deleteEvent(string calID, string[] args)
        {
            #region Delete
            {
                CalendarService myService = new CalendarService("ischool.CourseCalendar");
                myService.setUserCredentials(googleAcc, googlePWD);
                EventFeed feed = myService.Query(new EventQuery("https://www.google.com/calendar/feeds/" + calID + "/private/full"));
                AtomFeed batchFeed = new AtomFeed(feed);
                foreach (string id in args)
                {
                    EventEntry toDelete = (EventEntry)feed.Entries.FindById(new AtomId(feed.Id.AbsoluteUri + "/" + id));
                    if (toDelete != null)
                    {
                        toDelete.Id = new AtomId(toDelete.EditUri.ToString());
                        toDelete.BatchData = new GDataBatchEntryData("C", GDataBatchOperationType.delete);
                        batchFeed.Entries.Add(toDelete);
                    }
                }

                EventFeed batchResultFeed = (EventFeed)myService.Batch(batchFeed, new Uri(feed.Batch));
                foreach (EventEntry entry in batchResultFeed.Entries)
                {
                    if (entry.BatchData.Status.Code == 200)
                    {
                        break;
                    }
                }
            }
            #endregion
        }
        private static void addACLShared(string calID, string acc)
        {
            #region 新增分享
            CalendarService myService = new CalendarService("ischool.CourseCalendar");
            myService.setUserCredentials(googleAcc, googlePWD);
            AclEntry entry = new AclEntry();
            entry.Scope = new AclScope();
            entry.Scope.Type = AclScope.SCOPE_USER;
            entry.Scope.Value = acc;
            entry.Role = AclRole.ACL_CALENDAR_READ;
            try
            {
                AclEntry insertedEntry = myService.Insert(new Uri("https://www.google.com/calendar/feeds/" + calID + "/acl/full"), entry);
            }
            catch (GDataRequestException gex)
            {
                if (!gex.InnerException.Message.Contains("(409)"))
                    throw;
            }
            #endregion
        }
        private static string addEvent(string calID)
        {
            string result = "";
            #region 新增Event
            {
                CalendarService myService = new CalendarService("ischool.CourseCalendar");
                myService.setUserCredentials(googleAcc, googlePWD);
                EventFeed feed = myService.Query(new EventQuery("https://www.google.com/calendar/feeds/" + calID + "/private/full"));
                AtomFeed batchFeed = new AtomFeed(feed);
                Google.GData.Calendar.EventEntry eventEntry = new Google.GData.Calendar.EventEntry();

                eventEntry.Title.Text = "TEST";
                //eventEntry
                Where eventLocation = new Where();
                eventLocation.ValueString = "地點";
                eventEntry.Locations.Add(eventLocation);
                eventEntry.Notifications = true;
                eventEntry.Times.Add(new When(DateTime.Parse("2012/4/5 15:00"), DateTime.Parse("2012/4/5 17:00")));
                eventEntry.Participants.Add(new Who()
                {
                    ValueString = googleAcc,
                    Attendee_Type = new Who.AttendeeType() { Value = Who.AttendeeType.EVENT_REQUIRED },
                    Attendee_Status = new Who.AttendeeStatus() { Value = Who.AttendeeStatus.EVENT_ACCEPTED },
                    Rel = Who.RelType.EVENT_ATTENDEE
                });
                eventEntry.BatchData = new GDataBatchEntryData("TEST", GDataBatchOperationType.insert);
                batchFeed.Entries.Add(eventEntry);
                EventFeed batchResultFeed = (EventFeed)myService.Batch(batchFeed, new Uri(feed.Batch));
                foreach (EventEntry entry in batchResultFeed.Entries)
                {
                    if (entry.BatchData.Status.Code == 201)
                    {
                        result = entry.EventId;
                        break;

                    }
                }
            }
            #endregion
            return result;
        }
        private static string addNewCalender(string refid)
        {
            CalendarService myService = new CalendarService("ischool.CourseCalendar");
            myService.setUserCredentials(googleAcc, googlePWD);
            #region 建立新Calender
            string[] colorLists = new string[]{"#A32929","#B1365F","#7A367A","#5229A3","#29527A","#2952A3","#1B887A",
                            "#28754E","#0D7813","#528800","#88880E","#AB8B00","#BE6D00","#B1440E",
                            "#865A5A","#705770","#4E5D6C","#5A6986","#4A716C","#6E6E41","#8D6F47"};
            CalendarEntry newCal = new CalendarEntry();
            newCal.Title.Text = "TEST" + DateTime.Now.ToShortTimeString();
            newCal.TimeZone = "Asia/Taipei";
            newCal.Color = colorLists[new Random(DateTime.Now.Millisecond).Next(0, colorLists.Length)];
            Uri postUri = new Uri("http://www.google.com/calendar/feeds/default/owncalendars/full");
            newCal = (CalendarEntry)myService.Insert(postUri, newCal);
            #endregion
            String calendarURI = newCal.Id.Uri.ToString();
            String calendarID = calendarURI.Substring(calendarURI.LastIndexOf("/") + 1);
            return calendarID;
        }
        #endregion
    }
}
