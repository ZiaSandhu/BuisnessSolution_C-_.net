using Buisness_Soluiton.Screens.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Buisness_Soluiton.Screens
{
    public partial class DailyTransactionRecord :TemplateForm
    {
        public DailyTransactionRecord()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
                FilterByDate();
        }
       
        private void FilterByDate()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "SELECT * from tblDailySheet where Date>=@fromDate and Date<= @toDate";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@fromDate", FromDate.Value.Date);
                    cmd.Parameters.AddWithValue("@toDate", ToDate.Value.Date);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dtSup = new DataTable();
                        dtSup.Load(sdr);
                        GDVInvoice.DataSource = dtSup;
                        GDVInvoice.Columns[0].Visible = false;

                        CalculateSummary();
                    }
                    else
                    {
                        GDVInvoice.DataSource = null;
                        MessageBox.Show("No Record Exsist", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void PurchaseRecord_Load(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void PopulateGrid()
        {

            string query = "SELECT * from tblDailySheet";
            using (SqlConnection con=new SqlConnection(General.General.ConnectionString))
            {
                using(SqlCommand cmd=new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(sdr);

                        GDVInvoice.DataSource = dt;

                        CalculateSummary();
                    }
                    else
                        GDVInvoice.DataSource = null;
                }
            }
        }

        private void CalculateSummary()
        {
            decimal Income = 0;
            decimal Expense = 0;
            foreach(DataGridViewRow row in GDVInvoice.Rows)
            {
                Income += Convert.ToDecimal(row.Cells["Income"].Value);
                Expense+= Convert.ToDecimal(row.Cells["Expense"].Value);
            }
            decimal balance = Income - Expense;

            lblIncome.Text = Income.ToString();
            lblExpense.Text = Expense.ToString();
            lblBalance.Text = balance.ToString();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }
}
