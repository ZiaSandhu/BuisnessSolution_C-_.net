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
    public partial class StockValue : TemplateForm
    {
        public StockValue()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void StockValue_Load(object sender, EventArgs e)
        {
            PopulateStock();
        }

        private void PopulateStock()
        {
            string query = "select Item,Stock,Cost,stock*cost as Total from tblProducts";
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

                        Calculation();
                    }
                    else
                        GDVInvoice.DataSource = null;
                }
            }
        }

        private void Calculation()
        {
            decimal total = 0;
            foreach(DataGridViewRow row in GDVInvoice.Rows)
            {
                total += Convert.ToDecimal(row.Cells["Total"].Value);
            }
            lblTotal.Text = total.ToString();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
