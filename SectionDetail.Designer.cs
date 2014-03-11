namespace CourseCalendar
{
    partial class SectionDetail
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDayOfWeek = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPlace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Teacher = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colRepeat = new System.Windows.Forms.DataGridViewButtonColumn();
            this.linkCOurseCalendar = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDate,
            this.colDayOfWeek,
            this.colStartTime,
            this.colEndTime,
            this.colPlace,
            this.Teacher,
            this.colRepeat});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.dataGridViewX1.HighlightSelectedColumnHeaders = false;
            this.dataGridViewX1.Location = new System.Drawing.Point(14, 25);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowHeadersWidth = 25;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(523, 282);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellContentClick);
            this.dataGridViewX1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellEnter);
            this.dataGridViewX1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewX1_ColumnHeaderMouseClick);
            this.dataGridViewX1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewX1_CurrentCellDirtyStateChanged);
            this.dataGridViewX1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridViewX1_EditingControlShowing);
            this.dataGridViewX1.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewX1_RowHeaderMouseClick);
            this.dataGridViewX1.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridViewX1_RowsRemoved);
            this.dataGridViewX1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridViewX1_MouseClick);
            // 
            // colDate
            // 
            this.colDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDate.HeaderText = "日期";
            this.colDate.Name = "colDate";
            this.colDate.Width = 59;
            // 
            // colDayOfWeek
            // 
            this.colDayOfWeek.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDayOfWeek.HeaderText = "星期";
            this.colDayOfWeek.Name = "colDayOfWeek";
            this.colDayOfWeek.ReadOnly = true;
            this.colDayOfWeek.Width = 59;
            // 
            // colStartTime
            // 
            this.colStartTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colStartTime.HeaderText = "開始時間";
            this.colStartTime.Name = "colStartTime";
            this.colStartTime.Width = 85;
            // 
            // colEndTime
            // 
            this.colEndTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colEndTime.HeaderText = "結束時間";
            this.colEndTime.Name = "colEndTime";
            this.colEndTime.Width = 85;
            // 
            // colPlace
            // 
            this.colPlace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colPlace.HeaderText = "地點";
            this.colPlace.Name = "colPlace";
            this.colPlace.Width = 59;
            // 
            // Teacher
            // 
            this.Teacher.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Teacher.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Teacher.HeaderText = "教師";
            this.Teacher.Name = "Teacher";
            this.Teacher.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Teacher.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Teacher.Width = 59;
            // 
            // colRepeat
            // 
            this.colRepeat.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            this.colRepeat.DefaultCellStyle = dataGridViewCellStyle1;
            this.colRepeat.HeaderText = "重複";
            this.colRepeat.Name = "colRepeat";
            this.colRepeat.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colRepeat.Text = "...";
            this.colRepeat.UseColumnTextForButtonValue = true;
            this.colRepeat.Width = 42;
            // 
            // linkCOurseCalendar
            // 
            this.linkCOurseCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkCOurseCalendar.AutoSize = true;
            this.linkCOurseCalendar.Location = new System.Drawing.Point(468, 5);
            this.linkCOurseCalendar.Name = "linkCOurseCalendar";
            this.linkCOurseCalendar.Size = new System.Drawing.Size(73, 17);
            this.linkCOurseCalendar.TabIndex = 1;
            this.linkCOurseCalendar.TabStop = true;
            this.linkCOurseCalendar.Text = "課程行事歷";
            this.linkCOurseCalendar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkCOurseCalendar_LinkClicked);
            // 
            // SectionDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkCOurseCalendar);
            this.Controls.Add(this.dataGridViewX1);
            this.Group = "上課時間";
            this.Name = "SectionDetail";
            this.Size = new System.Drawing.Size(550, 315);
            this.PrimaryKeyChanged += new System.EventHandler(this.LoadData);
            this.SaveButtonClick += new System.EventHandler(this.SaveData);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.LinkLabel linkCOurseCalendar;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDayOfWeek;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEndTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPlace;
        private System.Windows.Forms.DataGridViewComboBoxColumn Teacher;
        private System.Windows.Forms.DataGridViewButtonColumn colRepeat;
    }
}
