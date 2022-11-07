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
    public partial class SaleInvoiceRecord :TemplateForm
    {
        public SaleInvoiceRecord()
        {
            InitializeComponent();
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

                        FilterNameBox.DataSource = dt;
                        FilterNameBox.DisplayMember = "Name";
                        FilterNameBox.ValueMember = "CustomerId";
                    }
                }
            }
        }
        private void FilterByDate()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "SELECT i.InvoiceId,C.Name,i.Date,i.Total,i.Discount,i.Net,i.Cash,i.Balance,i.[User] FROM tblInvoice i INNER JOIN tblCustomer C ON i.CustomerId=C.CustomerId where i.Date >= @fromdate and i.Date <= @toDate";
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
                    }
                    else
                    {
                        GDVInvoice.DataSource = null;
                        MessageBox.Show("No Record Exsist", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void FilterByNameWithoutDate()
        {

            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "SELECT i.InvoiceId,C.Name,i.Date,i.Total,i.Discount,i.Net,i.Cash,i.Balance,i.[User] FROM tblInvoice i INNER JOIN tblCustomer C ON i.CustomerId=C.CustomerId where i.CustomerId = @Id";
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
                        GDVInvoice.DataSource = dtSup;
                        GDVInvoice.Columns[0].Visible = false;
                    }
                    else
                    {
                        GDVInvoice.DataSource = null;
                        MessageBox.Show("No Record Exsist", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void FilterByNameDate()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "SELECT i.InvoiceId,C.Name,i.Date,i.Total,i.Discount,i.Net,i.Cash,i.Balance,i.[User] FROM tblInvoice i INNER JOIN tblCustomer C ON i.CustomerId=C.CustomerId where i.Date >= @fromdate and i.Date <= @toDate and i.CustomerId = @Id";
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
                        GDVInvoice.DataSource = dtSup;
                        GDVInvoice.Columns[0].Visible = false;
                    }
                    else
                    {
                        GDVInvoice.DataSource = null;
                        MessageBox.Show("No Record Exsist", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void UserRecords_Enter(object sender, EventArgs e)
        {

        }

        private void SaleInvoiceRecord_Load(object sender, EventArgs e)
        {
            PopulateGrid();
            LoadCustomerComboBox();
        }

        private void PopulateGrid()
        {

            string query = "SELECT i.InvoiceId,C.Name,i.Date,i.Total,i.Discount,i.Net,i.Cash,i.Balance,i.[User] FROM tblInvoice i INNER JOIN tblCustomer C ON i.CustomerId=C.CustomerId";
            using(SqlConnection con=new SqlConnection(General.General.ConnectionString))
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
                    }
                    else
                        GDVInvoice.DataSource = null;
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            /*if (GDVInvoice.Rows.Count > 0)
            {
                if (GDVInvoice.SelectedRows.Count == 0)
                    e.Cancel = true;
            }
            else
            {
                e.Cancel = true;
            }*/
            if (GDVInvoice.SelectedRows.Count == 0)
                e.Cancel = true;
        }

        private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View_Details vd = new View_Details();
            vd.Invoice = "Sale";
            vd.InvoiceId = Convert.ToInt32(GDVInvoice.SelectedRows[0].Cells[0].Value);
            vd.User = GDVInvoice.SelectedRows[0].Cells[1].Value.ToString();
            vd.Date = GDVInvoice.SelectedRows[0].Cells[2].Value.ToString();
            vd.Total = Convert.ToDecimal(GDVInvoice.SelectedRows[0].Cells[3].Value);
            vd.Amount = Convert.ToDecimal(GDVInvoice.SelectedRows[0].Cells[5].Value);
            vd.Cash = Convert.ToDecimal(GDVInvoice.SelectedRows[0].Cells[6].Value);
            vd.Balance = Convert.ToDecimal(GDVInvoice.SelectedRows[0].Cells[7].Value);
            vd.ShowDialog();
        }
    }
}
