using Buisness_Soluiton.Screens.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buisness_Soluiton.Screens
{
    public partial class DashboardForm : TemplateForm
    {
        public DashboardForm()
        {
            InitializeComponent();
        }
        //public string user { get; set; }

        async void RunTime()
        {
            while (true)
            {
                lblTime.Text = DateTime.Now.ToString("T");
                await Task.Delay(1000);
            }
        }
        private void SettingLabel()
        {
            
            lblUser.Text = "Welcome "+General.General.UserName+"";
            lblDate.Text = System.DateTime.Now.ToString("D");
            RunTime();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void manageUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Manage_Users().Show();
        }

        private void manageCustomersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ManageCustomerForm().Show();
        }

        private void manageSuppliersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ManageSupplierForm().Show();
        }

        private void customerLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CustomerLedgerForm().Show();
        }

        private void backupDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackupDataForm backupDataForm = new BackupDataForm();
            backupDataForm.Show();
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreDateForm rdf = new RestoreDateForm();
            rdf.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaleInvoice si = new SaleInvoice();
            si.Show();
        }

        private void manageProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageProductFrom mpf = new ManageProductFrom();
            mpf.Show();
        }

        private void purchaseInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseInvoice pi = new PurchaseInvoice();
            pi.Show();
        } 

        private void manageSaleInvoicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SaleInvoiceRecord().Show();
        }

        private void supplierLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierLedgerForm spf = new SupplierLedgerForm();
            spf.Show();
        }

        private void managePurchaseRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseRecord pr=new PurchaseRecord();
            pr.Show();
        }
        private void DashboardForm_Load(object sender, EventArgs e)
        {
            SettingLabel();
            QuickInfo();
            PopulateItemLowOnStock();
        }

        private void PopulateItemLowOnStock()
        {
            string query = "select Item,Stock from tblProducts WHERE Stock<=alert";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(sdr);
                        gridItem.DataSource = dt;
                    }
                    else
                        gridItem.DataSource = null;
                }
            }
        }

        private void QuickInfo()
        {
            TodaySale();
            TodayIncome();
            TodayExpense();
            lblBalance.Text = (Convert.ToDecimal(lblTdSale.Text) + Convert.ToDecimal(lblIncome.Text) - Convert.ToDecimal(lblExpense.Text)).ToString();
        }

        private void TodayExpense()
        {
            string query = "select Sum(Expense) from tblDailySheet WHERE  CAST(date AS DATE) = CAST( getdate() AS DATE)";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    string value = cmd.ExecuteScalar().ToString();
                    if (value == "")
                        lblExpense.Text = "0";
                    else
                        lblExpense.Text = value;
                }
            }
        }

        private void TodayIncome()
        {
            string query = "select Sum(Income) from tblCustomerLedger WHERE  CAST(date AS DATE) = CAST( getdate() AS DATE)";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    string value = cmd.ExecuteScalar().ToString();
                    if (value == "")
                        lblIncome.Text = "0";
                    else
                        lblIncome.Text = value;
                }
            }
        }

        private void TodaySale()
        {
            string query = "select Sum(cash) from tblInvoice WHERE  CAST(date AS DATE) = CAST( getdate() AS DATE)";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    string value = cmd.ExecuteScalar().ToString();
                    if (value == "")
                        lblTdSale.Text = "0";
                    else
                        lblTdSale.Text = value;
                }
            }
            string query1 = "UPDATE tblDailySheet SET INCOME="+lblTdSale.Text+" WHERE  (CAST(date AS DATE) = CAST( getdate() AS DATE)) AND Title='Today_Sale'";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query1, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DashboardForm_MouseEnter(object sender, EventArgs e)
        {
            PopulateItemLowOnStock();
            QuickInfo();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BackUpDataBase();
            Application.Exit();
        }

        private void BackUpDataBase()
        {
            string path;
            string query = "select top 1 path from BackUpTable Order By Id Desc";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            { 
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    path=cmd.ExecuteScalar().ToString();

                   // MessageBox.Show("Successfully BackedUp DataBase", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            if (path != string.Empty)
            {
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                     query = "BACKUP DATABASE Buisness_Solution TO DISK='" + path + "\\Buisness_Solution " + DateTime.Now.Ticks.ToString() + ".bak'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.CommandTimeout = 0;
                       // MessageBox.Show("Plz Wait While Data is being Backed Up","Waiting",MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Successfully BackedUp DataBase", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaleInvoice si = new SaleInvoice();
            si.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PurchaseInvoice pi = new PurchaseInvoice();
            pi.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddExpenses ad = new AddExpenses();
            ad.Show();
        }

        private void cashBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DailyTransactionRecord dtr = new DailyTransactionRecord();
            dtr.Show();
        }

        private void manageSaleInvoicesRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaleInvoiceRecord si = new SaleInvoiceRecord();
            si.Show();
        }

        private void managePurchaseRecordToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PurchaseRecord pr = new PurchaseRecord();
            pr.Show();
        }

        private void stockValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockValue sv = new StockValue();
            sv.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AddIncome ai = new AddIncome();
            ai.ShowDialog();
        }
    }
}
