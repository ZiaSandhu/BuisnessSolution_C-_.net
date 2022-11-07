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

namespace Buisness_Soluiton.Screens
{
    public partial class View_Details : TemplateForm
    {
        public View_Details()
        {
            InitializeComponent();
        }
        public string Invoice { get; set; }
        public  int InvoiceId { get; set; }
        public string User { get; set; }
        public  string Date { get; set; }
        public  decimal Total { get; set; } 
        public  decimal Amount { get; set; } 
        public  decimal Cash { get; set; } 
        public  decimal Balance { get; set; } 
        private void View_Details_Load(object sender, EventArgs e)
        {
            SetLabel();
            if (Invoice == "Sale")
            {
                PopulateSaleGrid();
            }
            else if (Invoice == "Purchase")
                PopulatePurchaseGrid();
        }
        
        private void SetLabel()
        {
            lblName.Text = User.ToString();
            lblDate.Text = Date.Substring(0,10);
            lblId.Text = "Invoice # " + InvoiceId.ToString();
            lblTotal.Text = Total.ToString();
            lblAmount.Text = Amount.ToString();
            lblCash.Text = Cash.ToString();
            lblBalance.Text = Balance.ToString();
        }
        private void PopulateSaleGrid()
        {
            String query = "Select * from tblInvoiceDetail where InvoiceId='"+InvoiceId+"'";
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
                        GDVInvoice.Columns[0].Visible = false;
                        GDVInvoice.Columns[1].Visible = false;
                    }
                }
            }
        }

        private void PopulatePurchaseGrid()
        {
            String query = "Select * from tblPurchaseDetail where PurchaseId='" + InvoiceId + "'";
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
                        GDVInvoice.DataSource = dt;
                        GDVInvoice.Columns[0].Visible = false;
                        GDVInvoice.Columns[1].Visible = false;
                    }
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
