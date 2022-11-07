using Buisness_Soluiton.General;
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
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Buisness_Soluiton.Screens
{
    public partial class CustomerLedgerForm : TemplateForm
    {
        public CustomerLedgerForm()
        {
            InitializeComponent();
        }
        private void populateCusledgergrid()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select cl.LedgerId,c.Name,cl.Title,cl.Income,cl.Outgoing,cl.CreatedDate,cl.Date from tblCustomerLedger cl INNER JOIN tblCustomer c ON cl.CustomerId=c.CustomerId";//where Name like '%" + txtSearch.Text.Trim() + "%'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dtSup = new DataTable();
                        dtSup.Load(sdr);
                        GDVCusLedger.DataSource = dtSup;
                        GDVCusLedger.Columns[0].Visible = false;
                    }
                    else
                        GDVCusLedger.DataSource = null;
                }
            }
            FilteredCalculateSummary();
        }

        private void LoadCustomerComboBox()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select CustomerId,Name from tblCustomer";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(sdr);

                        CusNameBox.DataSource = dt;
                        CusNameBox.DisplayMember = "Name";
                        CusNameBox.ValueMember = "CustomerId";

                        FilterNameBox.DataSource = dt;
                        FilterNameBox.DisplayMember = "Name";
                        FilterNameBox.ValueMember = "CustomerId";
                    }
                }
            }
        }

        private void CustomerLedgerForm_Load(object sender, EventArgs e)
        {
            populateCusledgergrid();
            LoadCustomerComboBox();
            FilteredCalculateSummary();
        }



        private void btnAddCus_Click(object sender, EventArgs e)
        {
            new ManageCustomerForm().ShowDialog();
            LoadCustomerComboBox();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    string query = "INSERT INTO tblCustomerLedger(Date,CustomerId,Title,Income,Description,CreatedDate) VALUES(@Curdate,'" + CusNameBox.SelectedValue+"','" + txtTitle.Text.Trim() + "','" + txtDebit.Text.Trim()  + "','" + txtNotes.Text.Trim() + "',@Insertdate)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Curdate", datepicker.Value.Date);
                        cmd.Parameters.AddWithValue("@Insertdate", System.DateTime.Now);
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();
                       

                    }
                    query = "INSERT INTO tblDailySheet(Name,Title,Income,note,date) VALUES('"+CusNameBox.Text+"','" + txtTitle.Text.Trim() + "','" + txtDebit.Text.Trim() + "','" + txtNotes.Text.Trim() + "',@Curdate)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Curdate", datepicker.Value.Date);
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();

                    }
                    MessageBox.Show("Ledger Saved Successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormControls();
                }
            }
        }

        private void ResetFormControls()
        {
            datepicker.Value = System.DateTime.Now;
            txtTitle.Text = "";
            txtDebit.Text = "";
            txtNotes.Text = "";
            CusNameBox.SelectedIndex = 0;
            CusNameBox.Focus();

            populateCusledgergrid();
            FilteredCalculateSummary();
        }

        private bool isValid()
        {
            if (CusNameBox.SelectedIndex == -1)
            {
                MessageBox.Show("Customer Name is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CusNameBox.Focus();
                return false;
            }
            //if (txtTitle.Text == string.Empty)
            //{
            //    MessageBox.Show("Transaction Title is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    txtTitle.Focus();
            //    return false;
            //}
            if (txtDebit.Text == string.Empty)
            {
                MessageBox.Show("Recieved Amount is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDebit.Focus();
                return false;
            }
            return true;
        }

        private void UserRecords_Enter(object sender, EventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            populateCusledgergrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (CBdate.Checked && CBName.Checked)//both check boxes are checked
            {
                //filter by date and name
                FilterByNameDate();
            }
            else if (!CBdate.Checked && CBName.Checked)//Name checkbox is checked
            {
                //FilterName by name without date
                FilterByNameWithoutDate();
            }
            else if (CBdate.Checked & !CBName.Checked)//date check box is checked
            {
                //filter by date
                FilterByDate();
            }
            else
                MessageBox.Show("NO Selection", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (GDVCusLedger.Rows.Count > 0)
                FilteredCalculateSummary();
            else
            {
                lblDebit.Text = "Rs.0000";
                lblCredit.Text = "Rs.0000";
                lblBalance.Text = "Rs.0000";
            }
        }

        private void FilteredCalculateSummary()
        {

            decimal debit = 0, credit = 0, balance = 0;
            foreach(DataGridViewRow rows in GDVCusLedger.Rows)
            {
                debit += Convert.ToDecimal(rows.Cells["Outgoing"].Value);
                credit += Convert.ToDecimal(rows.Cells["Income"].Value);
            }
            balance = debit - credit;

            lblDebit.Text = "Rs."+debit.ToString();
            lblCredit.Text = "Rs."+credit.ToString();
            lblBalance.Text = "Rs."+balance.ToString();

        }

        private void FilterByDate()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select cl.LedgerId,c.Name,cl.Title,cl.Income,cl.Outgoing,cl.CreatedDate,cl.Date from tblCustomerLedger cl INNER JOIN tblCustomer c ON cl.CustomerId=c.CustomerId where cl.Date >= @fromdate and cl.Date <= @toDate" ;
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
                        GDVCusLedger.DataSource = dtSup;
                        GDVCusLedger.Columns[0].Visible = false;
                    }
                    else
                    {
                        GDVCusLedger.DataSource = null;
                        MessageBox.Show("No Record Exsist", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void FilterByNameWithoutDate()
        {

            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select cl.LedgerId,c.Name,cl.Title,cl.Income,cl.Outgoing,cl.CreatedDate,cl.Date from tblCustomerLedger cl INNER JOIN tblCustomer c ON cl.CustomerId=c.CustomerId where cl.CustomerId = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ID", FilterNameBox.SelectedValue);

                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dtSup = new DataTable();
                        dtSup.Load(sdr);
                        GDVCusLedger.DataSource = dtSup;
                        GDVCusLedger.Columns[0].Visible = false;
                    }
                    else
                    {
                        GDVCusLedger.DataSource = null;
                        MessageBox.Show("No Record Exsist", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void FilterByNameDate()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select cl.LedgerId,c.Name,cl.Title,cl.Income,cl.Outgoing,cl.CreatedDate,cl.Date from tblCustomerLedger cl INNER JOIN tblCustomer c ON cl.CustomerId=c.CustomerId where cl.Date >= @fromdate and cl.Date <= @toDate and cl.CustomerId = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@fromDate", FromDate.Value.Date);
                    cmd.Parameters.AddWithValue("@toDate", ToDate.Value.Date);
                    cmd.Parameters.AddWithValue("@ID", FilterNameBox.SelectedValue);

                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dtSup = new DataTable();
                        dtSup.Load(sdr);
                        GDVCusLedger.DataSource = dtSup;
                        GDVCusLedger.Columns[0].Visible = false;
                    }
                    else
                    {
                        GDVCusLedger.DataSource = null;
                        MessageBox.Show("No Record Exsist", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));

            DataRow dr;
            foreach(DataGridViewRow row in GDVCusLedger.Rows)
            {
                dr = dt.NewRow();
                
                dr[0] = row.Cells[6].Value.ToString().Substring(0,10);
                dr[1] = row.Cells[1].Value.ToString();
                dr[2] = row.Cells[2].Value.ToString();
                dr[3] = Convert.ToDecimal(row.Cells[3].Value);
                dr[4] = Convert.ToDecimal(row.Cells[4].Value);
                
                dt.Rows.Add(dr);
            }
            

            ReportScreen rp = new ReportScreen();
            rp.ReportName = @"F:\Buisness Solution Ledger\Buisness Soluiton\Reports\LedgerReport.rpt";
            rp.ReportData = dt;
            rp.ShowDialog();
        }

        private void txtDebit_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);
        }
    }
    
}
