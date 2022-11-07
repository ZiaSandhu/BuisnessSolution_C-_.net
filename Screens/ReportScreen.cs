using Buisness_Soluiton.Screens.Templates;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buisness_Soluiton.Screens
{
    public partial class ReportScreen :TemplateForm
    {
        public string ReportName { get; set; }
        public DataTable ReportData { get; set; }
        public ReportScreen()
        {
            InitializeComponent();
        }

        private void ReportScreen_Load(object sender, EventArgs e)
        {
            crystalReportViewer1.SelectionMode=CrystalDecisions.Windows.Forms.SelectionMode.None;

            ReportDocument doc = new ReportDocument();
            doc.Load(ReportName);
            doc.SetDataSource(ReportData);
            crystalReportViewer1.ReportSource = doc;

        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
