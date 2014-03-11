using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CourseCalendar
{
    public partial class RepeatSectionDialog : FISCA.Presentation.Controls.BaseForm
    {
        public RepeatSectionDialog()
        {
            InitializeComponent();
        }
        public int Span { get { return intRepeatWeeks.Value; } set { intRepeatWeeks.Value = value; } }
        public int Times { get { return intTimes.Value; } set { intTimes.Value = value; } }
        public DateTime StartDate { get; set; }

        private void UpdateEndDate(object sender, EventArgs e)
        {
            lblEndTime.Text = "至 " + StartDate.AddDays(7 * Span * Times).ToString("yyyy/MM/dd") + " 。";
            intRepeatWeeks.Focus();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
