using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.LogAgent;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using FISCA.Data;

namespace CourseCalendar
{
    [FCode("Course.CourseCalendar", "上課時間")]
    public partial class SectionDetail : FISCA.Presentation.DetailContent
    {
        private static int _RepeatWeeks = 1;
        private static int _RepeatTimes = 19;

        private string _LoadingKey = null;
        private BackgroundWorker _Loader = new BackgroundWorker();
        private FISCA.UDT.AccessHelper _AccessHelper = new FISCA.UDT.AccessHelper();
        private List<Section> _DataSource = new List<Section>();
        private Calendar _Calendar = null;
        private List<DataItems.TeacherItem> TeacherRowSource = new List<DataItems.TeacherItem>();
        private List<DataItems.CaseItem> CaseRowSource = new List<DataItems.CaseItem>();

        //accessHelper.Select<Calendar>(condition)
        public SectionDetail()
        {
            InitializeComponent();
            this.dataGridViewX1.DataError += new DataGridViewDataErrorEventHandler(dataGridViewX1_DataError);
            this.Load += new EventHandler(SectionDetail_Load);
            _Loader.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                _DataSource = _AccessHelper.Select<Section>("RefCourseID='" + _LoadingKey + "'");
                _DataSource.Sort(delegate(Section s1, Section s2)
                {
                    return s1.StartTime.CompareTo(s2.StartTime);
                });
                var calList = _AccessHelper.Select<Calendar>("RefCourseID='" + _LoadingKey + "'");
                if (calList.Count > 0)
                {
                    _Calendar = calList[0];
                }
                else
                {
                    _Calendar = null;
                }
            };
            _Loader.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if (_LoadingKey != PrimaryKey)
                {
                    _LoadingKey = PrimaryKey;
                    this.Loading = true;
                    _Loader.RunWorkerAsync();
                }
                else
                {
                    if (_Calendar == null)
                    {
                        linkCOurseCalendar.Enabled = false;
                    }
                    else
                    {
                        linkCOurseCalendar.Enabled = true;
                    }
                    foreach (var section in _DataSource)
                    {
                        if (section.Removed) continue;
                        string dow = "";
                        switch (section.StartTime.DayOfWeek.ToString())
                        {
                            case "Monday": dow = "星期一"; break;
                            case "Tuesday": dow = "星期二"; break;
                            case "Wednesday": dow = "星期三"; break;
                            case "Thursday": dow = "星期四"; break;
                            case "Friday": dow = "星期五"; break;
                            case "Saturday": dow = "星期六"; break;
                            case "Sunday": dow = "星期日"; break;
                            default: dow = ""; break;
                        }
                        int idx = this.dataGridViewX1.Rows.Add(
                            section.StartTime.ToShortDateString()
                            , dow
                            , section.StartTime.ToString("HH:mm")
                            , section.EndTime.ToString("HH:mm")
                            , section.Place
                            , null
                            , null
                        );
                        if (TeacherRowSource.Where(x=>x.ID == section.RefTeacherID.ToString()).Count() > 0)
                            dataGridViewX1.Rows[idx].Cells[5].Value = section.RefTeacherID.ToString();
                        ////dataGridViewX1.Rows[idx].Cells[6].Value = section.RefCaseID.ToString();
                        dataGridViewX1.Rows[idx].Tag = section;
                    }
                    if (_DataSource.Count > 0)
                    {
                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[2].Value = _DataSource[_DataSource.Count - 1].StartTime.ToString("HH:mm");
                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[3].Value = _DataSource[_DataSource.Count - 1].EndTime.ToString("HH:mm");
                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[4].Value = _DataSource[_DataSource.Count - 1].Place;
                        //dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[5].Value = _DataSource[_DataSource.Count - 1].RefTeacherID.ToString();
                        //dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[6].Value = _DataSource[_DataSource.Count - 1].RefCaseID.ToString();

                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Tag = _DataSource[_DataSource.Count - 1];
                    }
                    this.Loading = false;
                    this.dataGridViewX1.CurrentCell = null;
                    CancelButtonVisible = false;
                    SaveButtonVisible = false;
                }
            };
            CancelButtonClick += new EventHandler(LoadData);
        }

        private void dataGridViewX1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void SectionDetail_Load(object sender, System.EventArgs e)
        {
        }

        private void InitTeacherRowSource()
        {
            DataTable dataTable = (new QueryHelper()).Select(string.Format(@"SELECT teacher.id, teacher.teacher_name, teacher.status FROM course
            LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id
            LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id
            LEFT JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id
            LEFT JOIN tag ON tag.id = tag_teacher.ref_tag_id
            WHERE tag.category = 'Teacher' AND tag.prefix = '教師' AND course.id={0}
			Group by teacher.id, teacher.teacher_name, teacher.status", PrimaryKey));

            TeacherRowSource.Clear();
            TeacherRowSource.Add(new DataItems.TeacherItem(null, "", "1"));
            foreach (DataRow row in dataTable.Rows)
            {
                DataItems.TeacherItem item = new DataItems.TeacherItem(row["id"] + "", row["teacher_name"] + "", row["status"] + "");
                TeacherRowSource.Add(item);
            }
        }

        private void InitCaseRowSource()
        {
            List<Case> Cases = _AccessHelper.Select<Case>();
            CaseRowSource.Clear();
            CaseRowSource.Add(new DataItems.CaseItem(null, ""));
            foreach (Case caze in Cases)
            {
                DataItems.CaseItem item = new DataItems.CaseItem(caze.UID, caze.Name);
                CaseRowSource.Add(item);
            }
        }

        private void LoadData(object sender, EventArgs e)
        {
            this.SaveButtonVisible = this.CancelButtonVisible = false;
            this._Calendar = null;
            this.dataGridViewX1.EndEdit();
            this.dataGridViewX1.Rows.Clear();
            if (!_Loader.IsBusy)
            {
                _LoadingKey = PrimaryKey;
                this.Loading = true;
                this.InitTeacherRowSource();
                //this.InitCaseRowSource();
                this.Teacher.DataSource = this.TeacherRowSource;
                this.Teacher.DisplayMember = "Name";
                this.Teacher.ValueMember = "ID";
                //this.Cazz.DataSource = this.CaseRowSource;
                //this.Cazz.DisplayMember = "Name";
                //this.Cazz.ValueMember = "ID";
                _Loader.RunWorkerAsync();
            }
        }

        private void dataGridViewX1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            bool err = false;
            this.dataGridViewX1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;
                for (int i = 0; i < dataGridViewX1.Columns.Count; i++)
                {
                    DateTime d;
                    var value = "" + row.Cells[i].Value;
                    switch (i)
                    {
                        case 0:
                            if (value.Trim() == "")
                            {
                                err = true;
                                row.Cells[1].Value = "";
                                row.Cells[0].ErrorText = "請填入日期(Ex: 2011/03/04)。";
                            }
                            else if ((!DateTime.TryParse(value, out d)) || value.Split('/').Length != 3)
                            {
                                err = true;
                                row.Cells[1].Value = "";
                                row.Cells[0].ErrorText = "日期格式不正確。";
                            }
                            else
                            {
                                row.Cells[0].ErrorText = "";
                                switch (d.DayOfWeek.ToString())
                                {
                                    case "Monday": row.Cells[1].Value = "星期一"; break;
                                    case "Tuesday": row.Cells[1].Value = "星期二"; break;
                                    case "Wednesday": row.Cells[1].Value = "星期三"; break;
                                    case "Thursday": row.Cells[1].Value = "星期四"; break;
                                    case "Friday": row.Cells[1].Value = "星期五"; break;
                                    case "Saturday": row.Cells[1].Value = "星期六"; break;
                                    case "Sunday": row.Cells[1].Value = "星期日"; break;
                                    default: row.Cells[1].Value = ""; break;
                                }
                            }
                            break;
                        case 2:
                            if (value.Trim() == "")
                            {
                                err = true;
                                row.Cells[i].ErrorText = "請填入時間(Ex: 14:00:00)。";
                            }
                            else if (!DateTime.TryParse(DateTime.Now.ToShortDateString() + " " + value, out d))
                            {
                                err = true;
                                row.Cells[i].ErrorText = "時間格式不正確。";
                            }
                            else
                            {
                                row.Cells[i].ErrorText = "";
                            }
                            break;
                        case 3:
                            if (value.Trim() == "")
                            {
                                err = true;
                                row.Cells[i].ErrorText = "請填入時間(Ex: 15:00:00)。";
                            }
                            else if (!DateTime.TryParse(DateTime.Now.ToShortDateString() + " " + value, out d))
                            {
                                err = true;
                                row.Cells[i].ErrorText = "時間格式不正確。";
                            }
                            else
                            {
                                row.Cells[i].ErrorText = "";
                            }
                            break;
                    }
                    dataGridViewX1.UpdateCellErrorText(i, row.Index);
                }
            }
            if (!err)
            {
                List<string> cl = new List<string>();
                foreach (var item in _DataSource)
                {
                    cl.Add(item.StartTime.ToString() + "||" + item.EndTime.ToString() + "||" + item.Place);
                }
                string bt = "", et = "", pla = "", tea = "";
                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    if (row.IsNewRow)
                    {
                        row.Cells[2].Value = bt;
                        row.Cells[3].Value = et;
                        row.Cells[4].Value = pla;
                        row.Cells[5].Value = tea;
                        continue;
                    }
                    bt = "" + row.Cells[2].Value;
                    et = "" + row.Cells[3].Value;
                    pla = "" + row.Cells[4].Value;
                    tea = "" + row.Cells[5].Value;
                    DateTime startTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[2].Value);
                    DateTime endTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[3].Value);
                    string place = "" + row.Cells[4].Value;
                    string key = "" + startTime.ToString() + "||" + endTime.ToString() + "||" + place + "||" + tea;
                    if (cl.Contains(key))
                    {
                        cl.Remove(key);
                    }
                    else
                    {
                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[2].Value = bt;
                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[3].Value = et;
                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[4].Value = pla;
                        dataGridViewX1.Rows[dataGridViewX1.NewRowIndex].Cells[5].Value = tea;
                        CancelButtonVisible = true;
                        if (FISCA.Permission.UserAcl.Current[this.GetType()].Editable)
                            SaveButtonVisible = true;
                        return;
                    }
                }
                if (cl.Count > 0)
                {
                    CancelButtonVisible = true;
                    if (FISCA.Permission.UserAcl.Current[this.GetType()].Editable)
                        SaveButtonVisible = true;
                }
                else
                {
                    CancelButtonVisible = false;
                    SaveButtonVisible = false;
                }
            }
            else
            {
                CancelButtonVisible = true;
                SaveButtonVisible = false;
            }
        }

        private void SaveData(object sender, EventArgs e)
        {
            K12.Data.CourseRecord cr = K12.Data.Course.SelectByID(this.PrimaryKey);
            //prepare for log data
            StringBuilder sb = new StringBuilder(string.Format("課程『{0}』變更上課時間：\n 新增：\n",cr.Name ));
            
            Dictionary<string, List<Section>> cl = new Dictionary<string, List<Section>>();
            foreach (var item in _DataSource)
            {
                if (item.Removed) continue;
                string key = item.StartTime.ToString() + "||" + item.EndTime.ToString() + "||" + item.Place + "||" + item.RefTeacherID;
                if (!cl.ContainsKey(key))
                    cl.Add(key, new List<Section>());
                cl[key].Add(item);
            }
            List<Section> sections = new List<Section>();
            List<Section> update_sections = new List<Section>();
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;

                Section s = new Section();
                s.RefCourseID = _LoadingKey;
                s.StartTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[2].Value);
                s.EndTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[3].Value);
                s.Place = "" + row.Cells[4].Value;
                int teacher_id = 0;
                //int case_id = 0;
                if (int.TryParse(row.Cells[5].Value + "", out teacher_id))
                    s.RefTeacherID = teacher_id;
                //if (int.TryParse(row.Cells[6].Value + "", out case_id))
                //    s.RefCaseID = case_id;

                s.IsPublished = false;

                string key = s.StartTime.ToString() + "||" + s.EndTime.ToString() + "||" + s.Place + "||" + s.RefTeacherID;
                if (cl.ContainsKey(key) && cl[key].Count > 0)
                {
                    s=cl[key].ElementAt(0);
                    s.RefTeacherID = teacher_id;
                    //s.RefCaseID = case_id;
                    cl[key].RemoveAt(0);
                }
                else
                {
                    sections.Add(s);
                    //prepare log data

                }
                //update_sections.Add(s);
                    
            }
            sb.Append("刪除：");
            foreach (var list in cl.Values)
            {
                foreach (var item in list)
                {
                    if (item.EventID == "")
                        item.Deleted = true;
                    else
                        item.Removed = true;

                    sections.Add(item);
                    //prepare for log data
                    sb.Append(string.Format("StartTime :", item.StartTime.ToString("yyyy/MM/dd HH:mm:ss")));
                    sb.Append(string.Format(",  EndTime :", item.EndTime.ToString("yyyy/MM/dd HH:mm:ss")));
                    sb.Append(string.Format(",  Place :", "" + item.Place));
                    //sb.Append(string.Format(",  Teacher :", "" + item.RefTeacherID));
                    sb.Append("\n");
                }
            }
            sections.SaveAll();
            //update_sections.SaveAll();
            FISCA.LogAgent.ApplicationLog.Log("上課時間.課程", "修改", "course", this.PrimaryKey, sb.ToString());

            LoadData(null, null);
            SectionSyncColumn.Reload();
        }

        private void dataGridViewX1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            List<string> cl = new List<string>();
            foreach (var item in _DataSource)
            {
                cl.Add(item.StartTime.ToString() + "||" + item.EndTime.ToString() + "||" + item.Place + "||" + item.RefTeacherID);
            }
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;
                DateTime startTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[2].Value);
                DateTime endTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[3].Value);
                string place = "" + row.Cells[4].Value;
                var teacher_id = row.Cells[5].Value;
                string key = "" + startTime.ToString() + "||" + endTime.ToString() + "||" + place + "||" + teacher_id;
                if (cl.Contains(key))
                {
                    cl.Remove(key);
                }
                else
                {
                    if (FISCA.Permission.UserAcl.Current[this.GetType()].Editable)
                        SaveButtonVisible = true;

                    CancelButtonVisible = true;

                    return;
                }
            }
            if (cl.Count > 0)
            {
                CancelButtonVisible = true;
                if (FISCA.Permission.UserAcl.Current[this.GetType()].Editable)
                    SaveButtonVisible = true;
            }
            else
            {
                CancelButtonVisible = false;
                SaveButtonVisible = false;
            }
        }

        private void linkCOurseCalendar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_Calendar != null)
                System.Diagnostics.Process.Start("https://www.google.com/calendar/b/0/embed?src=" + _Calendar.GoogleCalanderID);
        }
        class RowSorter : IComparer<DataGridViewRow>, System.Collections.IComparer
        {
            #region IComparer<DataGridViewRow> 成員

            public int Compare(DataGridViewRow x, DataGridViewRow y)
            {
                try
                {
                    DateTime d1 = DateTime.Parse("" + x.Cells[0].Value + " " + x.Cells[2].Value);
                    DateTime d2 = DateTime.Parse("" + y.Cells[0].Value + " " + y.Cells[2].Value);
                    return d1.CompareTo(d2);
                }
                catch { }
                return 0;
            }

            #endregion

            #region IComparer 成員

            public int Compare(object x, object y)
            {
                return Compare((DataGridViewRow)x, (DataGridViewRow)y);
            }

            #endregion
        }


        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == colRepeat.Index && !dataGridViewX1.Rows[e.RowIndex].IsNewRow)
            {
                if (this.CancelButtonVisible && !this.SaveButtonVisible)
                {
                    MessageBox.Show("目前輸入資料有誤，請先修正。");
                }
                else
                {
                    RepeatSectionDialog dialog = new RepeatSectionDialog()
                    {
                        StartDate = DateTime.Parse("" + dataGridViewX1.Rows[e.RowIndex].Cells[0].Value),
                        Span = _RepeatWeeks,
                        Times = _RepeatTimes
                    };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        _RepeatTimes = dialog.Times;
                        _RepeatWeeks = dialog.Span;
                        List<string> rows = new List<string>();
                        foreach (DataGridViewRow row in dataGridViewX1.Rows)
                        {
                            var bt = "" + row.Cells[2].Value;
                            var et = "" + row.Cells[3].Value;
                            DateTime startTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[2].Value);
                            DateTime endTime = DateTime.Parse("" + row.Cells[0].Value + " " + row.Cells[3].Value);
                            string key = "" + startTime.ToString() + "||" + endTime.ToString();
                            rows.Add(key);
                        }

                        DateTime startDate = DateTime.Parse("" + dataGridViewX1.Rows[e.RowIndex].Cells[0].Value + " " + dataGridViewX1.Rows[e.RowIndex].Cells[2].Value);
                        DateTime endDate = DateTime.Parse("" + dataGridViewX1.Rows[e.RowIndex].Cells[0].Value + " " + dataGridViewX1.Rows[e.RowIndex].Cells[3].Value);
                        string place = "" + dataGridViewX1.Rows[e.RowIndex].Cells[4].Value;
                        var teacher_id = dataGridViewX1.Rows[e.RowIndex].Cells[5].Value;
                        for (int i = _RepeatTimes; i > 0; i--)
                        {
                            startDate = startDate.AddDays(_RepeatWeeks * 7);
                            endDate = endDate.AddDays(_RepeatWeeks * 7);
                            string key = "" + startDate.ToString() + "||" + endDate.ToString();
                            if (!rows.Contains(key))
                            {
                                string dow = "";
                                switch (startDate.DayOfWeek.ToString())
                                {
                                    case "Monday": dow = "星期一"; break;
                                    case "Tuesday": dow = "星期二"; break;
                                    case "Wednesday": dow = "星期三"; break;
                                    case "Thursday": dow = "星期四"; break;
                                    case "Friday": dow = "星期五"; break;
                                    case "Saturday": dow = "星期六"; break;
                                    case "Sunday": dow = "星期日"; break;
                                    default: dow = ""; break;
                                }
                                this.dataGridViewX1.Rows.Add(
                                    startDate.ToShortDateString()
                                    , dow
                                    , startDate.ToString("HH:mm")
                                    , endDate.ToString("HH:mm")
                                    , place
                                    , teacher_id
                                );
                            }
                        }
                        dataGridViewX1.Sort(new RowSorter());
                        dataGridViewX1_CurrentCellDirtyStateChanged(null, null);
                    }
                }
            }
        }

        private void dataGridViewX1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 5 && dataGridViewX1.SelectedCells.Count == 1)
            {
                dataGridViewX1.BeginEdit(true);  //Raise EditingControlShowing Event !
                if (dataGridViewX1.CurrentCell != null && dataGridViewX1.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell")
                {
                    ComboBox comboBox = dataGridViewX1.EditingControl as ComboBox;
                    if (comboBox != null)
                        comboBox.DroppedDown = true;  //自動拉下清單
                }
            }
        }

        private void dataGridViewX1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dataGridViewX1.CurrentCell.ColumnIndex == 5)
            {
                if (e.Control is DataGridViewComboBoxEditingControl)
                {
                    ComboBox comboBox = e.Control as ComboBox;
                    comboBox.DropDownClosed += new EventHandler(comboBox_DropDownClosed);
                }
            }
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            (sender as ComboBox).DropDownClosed -= new EventHandler(comboBox_DropDownClosed);

            this.dataGridViewX1.CurrentRow.Cells[6].Selected = true;
            //this.dataGridViewX1.EndEdit();
        }

        private void dataGridViewX1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridViewX1.CurrentCell = null;
            dataGridViewX1.Rows[e.RowIndex].Selected = true;
        }

        private void dataGridViewX1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridViewX1.CurrentCell = null;
        }

        private void dataGridViewX1_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dataGridViewX1.CurrentCell = null;
                dataGridViewX1.SelectAll();
            }
        }
    }
}
