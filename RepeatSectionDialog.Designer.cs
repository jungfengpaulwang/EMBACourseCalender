namespace CourseCalendar
{
    partial class RepeatSectionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.btnAccept = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.intRepeatWeeks = new DevComponents.Editors.IntegerInput();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.intTimes = new DevComponents.Editors.IntegerInput();
            this.lblEndTime = new DevComponents.DotNetBar.LabelX();
            ((System.ComponentModel.ISupportInitialize)(this.intRepeatWeeks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intTimes)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(148, 98);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            // 
            // btnAccept
            // 
            this.btnAccept.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.BackColor = System.Drawing.Color.Transparent;
            this.btnAccept.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnAccept.Location = new System.Drawing.Point(67, 98);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "確定";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(13, 13);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(74, 21);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "連續間隔：";
            // 
            // intRepeatWeeks
            // 
            this.intRepeatWeeks.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intRepeatWeeks.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intRepeatWeeks.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intRepeatWeeks.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intRepeatWeeks.Location = new System.Drawing.Point(87, 11);
            this.intRepeatWeeks.MinValue = 0;
            this.intRepeatWeeks.Name = "intRepeatWeeks";
            this.intRepeatWeeks.ShowUpDown = true;
            this.intRepeatWeeks.Size = new System.Drawing.Size(54, 25);
            this.intRepeatWeeks.TabIndex = 0;
            this.intRepeatWeeks.Value = 1;
            this.intRepeatWeeks.ValueChanged += new System.EventHandler(this.UpdateEndDate);
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(141, 13);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(34, 21);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "週。";
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(13, 42);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(74, 21);
            this.labelX3.TabIndex = 1;
            this.labelX3.Text = "重複開設：";
            // 
            // labelX4
            // 
            this.labelX4.AutoSize = true;
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(141, 42);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(34, 21);
            this.labelX4.TabIndex = 1;
            this.labelX4.Text = "次。";
            // 
            // intTimes
            // 
            this.intTimes.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intTimes.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intTimes.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intTimes.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intTimes.Location = new System.Drawing.Point(87, 40);
            this.intTimes.MinValue = 0;
            this.intTimes.Name = "intTimes";
            this.intTimes.ShowUpDown = true;
            this.intTimes.Size = new System.Drawing.Size(54, 25);
            this.intTimes.TabIndex = 1;
            this.intTimes.Value = 19;
            this.intTimes.ValueChanged += new System.EventHandler(this.UpdateEndDate);
            // 
            // lblEndTime
            // 
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblEndTime.BackgroundStyle.Class = "";
            this.lblEndTime.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblEndTime.Location = new System.Drawing.Point(13, 69);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(93, 21);
            this.lblEndTime.TabIndex = 1;
            this.lblEndTime.Text = "至2011/12/21";
            // 
            // RepeatSectionDialog
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(235, 133);
            this.Controls.Add(this.intTimes);
            this.Controls.Add(this.intRepeatWeeks);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.lblEndTime);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnCancel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RepeatSectionDialog";
            this.Text = "連續開課";
            this.Load += new System.EventHandler(this.UpdateEndDate);
            ((System.ComponentModel.ISupportInitialize)(this.intRepeatWeeks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intTimes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.ButtonX btnAccept;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.Editors.IntegerInput intRepeatWeeks;
        private DevComponents.Editors.IntegerInput intTimes;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX lblEndTime;
    }
}