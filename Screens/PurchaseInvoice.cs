using Buisness_Soluiton.Screens.Templates;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Buisness_Soluiton.Screens
{
    public partial class PurchaseInvoice : TemplateForm
    {
        public PurchaseInvoice()
        {
            InitializeComponent();
        }

        int Stock = 0;
        //decimal cost = 0;
        decimal rate = 0;
        int itemqty = 0;
        decimal price = 0;
        decimal itemTotal = 0;
        decimal disRate = 0;
        decimal GrandTotal = 0;
        decimal GrandDiscount = 0;
        decimal NetAmount = 0;
        decimal cash = 0;
        decimal balance = 0;

        public int SupplierId { get; set; }
        public int PurchaseId { get; set; }
        public bool SaveInvoice { get; set; }
        private void SaleInvoice_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadItems();
            LoadSupplier();
            this.SupplierId = 0;
        }

        private void LoadCategories()
        {
            string query = "select * from tblCategories";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(sdr);

                    CBcat.DataSource = dt;
                    CBcat.DisplayMember = "Name";
                    CBcat.ValueMember = "CategoryId";
                }
            }
        }

        private void LoadItems()
        {
            string query = "select ProductId,Item from tblProducts where CategoryId='" + CBcat.SelectedValue + "'";
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
                        CBitem.DataSource = dt;
                        CBitem.DisplayMember = "Item";
                        CBitem.ValueMember = "ProductId";
                    }
                    else
                    {
                        CBitem.DataSource = null;
                        ClearValues();
                    }
                }
            }
            GetValues();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ManageProductFrom().ShowDialog();
            LoadCategories();
        }

        private void CBcat_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadItems();
            //ClearValues();
        }

        private void CBitem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBitem.SelectedIndex != -1)
            {
                ClearValues();
                GetValues();
            }
        }

        private void ClearValues()
        {
            itemqty = 0;
            itemTotal = 0;
            price = 0;
            rate = 0;
            //cost = 0;
            Stock = 0;
            disRate = 0;
            txtQty.Text = "";
            txtDisRate.Text = "";
            
            txtItemTotal.Text = "";
            lblStock.Text = "000";
            txtRate.Text = "";
            

            btnAdd.Text = "Add";
        }

        private void GetValues()
        {
            string query = "select Cost,Stock from tblProducts where ProductId='" + CBitem.SelectedValue + "'";
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
                        Stock = Convert.ToInt32(dt.Rows[0][1]);
                        //cost = Convert.ToDecimal(dt.Rows[0][1]);
                        rate = Convert.ToDecimal(dt.Rows[0][0]);

                        lblStock.Text = Stock.ToString();
                        //lblCost.Text = cost.ToString();
                        txtRate.Text = rate.ToString();
                    }
                }
            }
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            if (txtQty.Text != "")
            {
                itemqty = Convert.ToInt32(txtQty.Text);
            }
            else
                itemqty = 0;
            CalculateItemTotal();
        }

        private void CalculateItemTotal()
        {
            if (txtDisRate.Text == "")
                price = rate;
            else
                price = disRate;
            itemTotal = itemqty * price;
            txtItemTotal.Text = itemTotal.ToString();
        }

        private void txtDisRate_TextChanged(object sender, EventArgs e)
        {
            if (txtDisRate.Enabled == true)
            {
                if (txtDisRate.Text != "")
                {
                    disRate = Convert.ToDecimal(txtDisRate.Text);
                }
                else
                    disRate = 0;
                CalculateItemTotal();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int sr;
            if (ProdIsValid() && btnAdd.Text == "Add")
            {
                sr = GDVitemDetail.Rows.Add();
                GDVitemDetail.Rows[sr].Cells["sr"].Value = sr + 1;
                GDVitemDetail.Rows[sr].Cells["desc"].Value = CBitem.Text;
                GDVitemDetail.Rows[sr].Cells["qty"].Value = itemqty.ToString();
                GDVitemDetail.Rows[sr].Cells["uprice"].Value = price.ToString();
                GDVitemDetail.Rows[sr].Cells["total"].Value = itemTotal.ToString();

            }
            else if (ProdIsValid() && btnAdd.Text == "Update")
            {
                GDVitemDetail.SelectedRows[0].Cells["desc"].Value = CBitem.Text;
                GDVitemDetail.SelectedRows[0].Cells["qty"].Value = itemqty.ToString();
                GDVitemDetail.SelectedRows[0].Cells["uprice"].Value = price.ToString();
                GDVitemDetail.SelectedRows[0].Cells["total"].Value = itemTotal.ToString();
            }
            ClearValues();
            CalGrandTotal();
        }

        private bool ProdIsValid()
        {
            if (CBitem.SelectedIndex == -1)
            {
                MessageBox.Show("Select Product to Add!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CBitem.Focus();
                return false;
            }
            if (btnAdd.Text == "Add")
            {
                foreach (DataGridViewRow row in GDVitemDetail.Rows)
                {
                    if (row.Cells["desc"].Value.ToString() == CBitem.Text)
                    {
                        MessageBox.Show("Item Already Exsist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CBitem.Focus();
                        return false;
                    }
                }
            }
            if (itemqty <= 0)
            {
                MessageBox.Show("Enter Quantity!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtQty.Focus();
                return false;
            }

            return true;
        }


        void CalGrandTotal()
        {
            GrandTotal = 0;
            int x = 1;
            foreach (DataGridViewRow dr in GDVitemDetail.Rows)
            {
                dr.Cells["sr"].Value = x++.ToString();
                GrandTotal += Convert.ToDecimal(dr.Cells["total"].Value);
            }
            NetAmount = GrandTotal;
            lblTotal.Text = GrandTotal.ToString();
            lblNetAmount.Text = NetAmount.ToString();
            Calculation();
        }

        private void txtTotalDis_TextChanged(object sender, EventArgs e)
        {
            Calculation();
        }

        private void Calculation()
        {
            if (txtTotalDis.Text != "")
            {
                GrandDiscount = Convert.ToDecimal(txtTotalDis.Text);
            }
            else
            {
                GrandDiscount = 0;
            }
            if (txtCash.Text != "")
            {
                cash = Convert.ToDecimal(txtCash.Text);

            }
            else
            {
                cash = 0;
            }
            NetAmount = GrandTotal - GrandDiscount;
            lblNetAmount.Text = NetAmount.ToString();
            balance = NetAmount - cash;
            lblBalance.Text = balance.ToString();
        }

        private void txtCash_TextChanged(object sender, EventArgs e)
        {
            Calculation();
        }

        private void btnAddCus_Click(object sender, EventArgs e)
        {
            new ManageSupplierForm().ShowDialog();
            LoadSupplier();
        }

        private void LoadSupplier()
        {
            string query = "Select supplierId,Name from tblSupplier";
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
                        txtCusName.DataSource = dt;
                        txtCusName.DisplayMember = "Name";
                        txtCusName.ValueMember = "SupplierId";
                    }
                }
            }
        }

        private void txtCusName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(txtCusName.SelectedIndex != -1)
            GetPhone();
        }

        private void GetPhone()
        {
            SupplierId = Convert.ToInt32(txtCusName.SelectedValue.GetHashCode());
           // bool parseOK = Int32.TryParse(txtCusName.SelectedValue.ToString(), out SupplierId);
            string query = "select phone from tblSupplier where SupplierId='" + this.SupplierId + "'";
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        dt.Load(sdr);
                        txtCusPhone.Text = dt.Rows[0][0].ToString();
                        txtCusPhone.Enabled = false;
                    }
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                InsertInvoice();
                if (balance > 0)
                    AddCredit();
                InsertInvoiceDetail_SubStock();
                if(cash > 0)
                    AddCashToDailySheet();
                btnSave.Enabled = false;
                btnDelete.Enabled = true;
                btnAdd.Enabled = false;
                MessageBox.Show("Invoice Saved Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AddCashToDailySheet()
        {
            string query = "Insert Into tblDailySheet(Name,title,Expense,Date) Values('"+txtCusName.Text+"','Purchae#"+this.PurchaseId+"','"+cash.ToString()+"',@date)";
           using(SqlConnection con=new SqlConnection(General.General.ConnectionString))
            {
                using(SqlCommand cmd=new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@date", dateInvoice.Value.Date);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddCredit()
        {
            string query = "Insert INTO tblSupplierLedger(date,SupplierId,Title,Income) VALUES(@dateNow,'" + this.SupplierId + "','Invoice No " + this.PurchaseId + "','" + lblBalance.Text + "')";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@dateNow", System.DateTime.Now);
                    //cmd.Parameters.AddWithValue("@Crdate", dateInvoice.Value.Date);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertInvoiceDetail_SubStock()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "INSERT INTO tblPurchaseDetail Values('" + this.PurchaseId + "',@item,@price,@qty,@total)";
                foreach (DataGridViewRow row in GDVitemDetail.Rows)
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@item", row.Cells["desc"].Value.ToString());
                        cmd.Parameters.AddWithValue("@price", row.Cells["uprice"].Value.ToString());
                        cmd.Parameters.AddWithValue("@qty", row.Cells["qty"].Value.ToString());
                        cmd.Parameters.AddWithValue("@total", row.Cells["total"].Value.ToString());
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                foreach (DataGridViewRow row in GDVitemDetail.Rows)
                {
                    using (SqlCommand cmd = new SqlCommand("usp_tblProducts_AddStock", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@item", row.Cells["desc"].Value.ToString());
                        cmd.Parameters.AddWithValue("@qty", row.Cells["qty"].Value.ToString());
                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        private void InsertInvoice()
        {
            string query = "Insert INTO tblPurchase VALUES('" + this.SupplierId + "',@date,'" + GrandTotal.ToString() + "','";
            query += GrandDiscount.ToString() + "','" + NetAmount.ToString() + "','" + cash.ToString() + "','" + balance.ToString() + "','" + General.General.UserName + "')";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@date", dateInvoice.Value.Date);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                string que = "select max(PurchaseId) from tblPurchase";
                using (SqlCommand cmd = new SqlCommand(que, con))
                {
                    PurchaseId = Convert.ToInt32(cmd.ExecuteScalar());
                    //lblInvoiceId.Text = InvoiceId.ToString();
                }
            }
        }

        private bool IsValid()
        {
            //if grid is empty return false
            if (GDVitemDetail.Rows.Count == 0)
            {
                MessageBox.Show("Invoice is Empty", "Empty Invoice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //check cash 
            if (txtCash.Text == "" && !radioCredit.Checked)
            {
                MessageBox.Show("Enter Recieved Amount!", "Amount Recieved", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCash.Focus();
                return false;
            }
            // if balance available then check customer detial
            if (this.SupplierId == 0)
            {
                MessageBox.Show("Enter Customer Detail!", "Customer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCusName.Focus();
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            new SaleInvoice().Show();
        }

        private void GDVitemDetail_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CBitem.Text = GDVitemDetail.SelectedRows[0].Cells[1].Value.ToString();
            txtQty.Text = GDVitemDetail.SelectedRows[0].Cells[2].Value.ToString();
            txtDisRate.Text = GDVitemDetail.SelectedRows[0].Cells[3].Value.ToString();
            txtItemTotal.Text = GDVitemDetail.SelectedRows[0].Cells[4].Value.ToString();
            btnAdd.Text = "Update";
            GetValues();
        }

        private void GDVitemDetail_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?\n This Process Can't Rollback.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                e.Cancel = true;
        }

        private void GDVitemDetail_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            CalGrandTotal();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?\n This Process Can't Rollback.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                //delete form invoice table
                string query = "DELETE FROM tblPurchase WHERE PurchaseId='" + this.PurchaseId + "'";
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    //select item from invoicedetails and update stock
                    DataTable dt = new DataTable();
                    query = "select item,qty FROM tblPurchaseDetail WHERE PurchaseId='" + this.PurchaseId + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // con.Open();
                        SqlDataReader sdr = cmd.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            dt.Load(sdr);
                        }
                    }
                    query = "usp_tblProducts_SubStock";
                    foreach (DataRow row in dt.Rows)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@item", row["item"]);
                            cmd.Parameters.AddWithValue("@qty", row["qty"]);
                            // con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    query = "DELETE FROM tblPurchaseDetail WHERE PurchaseId ='" + this.PurchaseId + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    btnAdd.Enabled = true;
                    btnSave.Enabled = true;
                    btnDelete.Enabled = false;


                }

                MessageBox.Show("Invoice Deleted Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }

        private void txtDis_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }

        private void txtDisRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }

        private void txtTotalDis_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }

        private void txtCash_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }

    }
}
