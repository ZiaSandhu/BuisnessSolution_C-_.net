using Buisness_Soluiton.General;
using Buisness_Soluiton.Screens.Templates;
using Microsoft.VisualBasic;
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
    public partial class ManageProductFrom : TemplateForm
    {
        public ManageProductFrom()
        {
            InitializeComponent();
        }

        public int ProductId { get; set; }

        private void btnAddCus_Click(object sender, EventArgs e)
        {
            string cat = Interaction.InputBox("Adding Category", "Enter Category", "Name", -1, -1);
            string query = "INSERT INTO tblCategories (Name) VALUES('" + cat + "')";
            using(SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using(SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Category Added","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            LoadCategoryBox();
            
        }

        private void btnAddSup_Click(object sender, EventArgs e)
        {
            new ManageSupplierForm().ShowDialog();
            LoadSupplierBox();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ManageProductFrom_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            LoadCategoryBox();
            LoadSupplierBox();
            ResetFormControls();
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            string query = "usp_tblProducts_ShowData";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(sdr);
                        GDVProducts.DataSource = dt;
                        GDVProducts.Columns[0].Visible = false;
                    }
                    else
                        GDVProducts.DataSource = null;
                }
            }
        }

        private void LoadCategoryBox()
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

        private void LoadSupplierBox()
        {
            string query = "select SupplierId,Name from tblSupplier";
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(sdr);

                    CBsup.DataSource = dt;
                    CBsup.DisplayMember = "Name";
                    CBsup.ValueMember = "SupplierId";
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                string query="";
                if(ProductId == 0)
                    query="INSERT INTO tblProducts VALUES('"+txtItem.Text+"','"+txtRate.Text+"','"+txtCost.Text+"','"+CBcat.SelectedValue+"','"+CBsup.SelectedValue+"','"+txtStock.Text+"','"+txtAlert.Text+"')";
                else
                    query="UPDATE tblProducts SET Item='"+txtItem.Text+"',Rate='"+txtRate.Text+"', Cost='"+txtCost.Text+"', CategoryId='"+CBcat.SelectedValue +"', SupplierId='"+CBsup.SelectedValue+"', Stock='"+txtStock.Text+"', Alert='"+txtAlert.Text+"' WHERE ProductId='"+ProductId+"'";
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Product Saved/Updated Successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       
                    }
                }
                PopulateGrid();
                ResetFormControls();
            }
            
        }

        private void ResetFormControls()
        {
            ProductId = 0;
            CBcat.SelectedIndex = -1;
            CBsup.SelectedIndex = -1;
            txtItem.Text = "";
            txtRate.Text = "";
            txtCost.Text = "";
            txtStock.Text = "";
            txtAlert.Text = "";

            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        private bool IsValid()
        {
            if(CBcat.SelectedIndex == -1)
            {
                MessageBox.Show("Plz Select Category!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CBcat.Focus();
                return false;
            }
            if (CBsup.SelectedIndex == -1)
            {
                MessageBox.Show("Plz Select Supplier!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CBsup.Focus();
                return false;
            }
            if (txtItem.Text == string.Empty)
            {
                MessageBox.Show("Enter Item Name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtItem.Focus();
                return false;
            }
            if (txtRate.Text == string.Empty)
            {
                MessageBox.Show("Enter Item Rate!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtRate.Focus();
                return false;
            }
            if (txtCost.Text == string.Empty)
            {
                MessageBox.Show("Enter Item Cost!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCost.Focus();
                return false;
            }
            if (txtStock.Text == string.Empty)
            {
                MessageBox.Show("Enter Current Stock!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtStock.Focus();
                return false;
            }
            if (txtAlert.Text == string.Empty)
            {
                MessageBox.Show("Enter Stock Alert Limit!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAlert.Focus();
                return false;
            }
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select * from tblProducts where Item='" + txtItem.Text.Trim() + "'";
                if (this.ProductId > 0)//updating same name 
                    query = "select * from tblProducts where Item='" + txtItem.Text.Trim() + "' and ProductId !=" + this.ProductId;
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();

                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        MessageBox.Show("Product Name already Exsist! \n Plz Choose Different Name!", "Item Exsist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return true;
        }

        private void GDVProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(GDVProducts.Rows.Count > 0)
            {
                ProductId = Convert.ToInt32(GDVProducts.SelectedRows[0].Cells[0].Value);
                txtItem.Text = GDVProducts.SelectedRows[0].Cells[1].Value.ToString();
                txtRate.Text = GDVProducts.SelectedRows[0].Cells[2].Value.ToString();
                txtCost.Text = GDVProducts.SelectedRows[0].Cells[3].Value.ToString();
                CBcat.Text=GDVProducts.SelectedRows[0].Cells[4].Value.ToString();
                CBsup.Text=GDVProducts.SelectedRows[0].Cells[5].Value.ToString();
                txtStock.Text = GDVProducts.SelectedRows[0].Cells[6].Value.ToString();
                txtAlert.Text = GDVProducts.SelectedRows[0].Cells[7].Value.ToString();
            }
            btnDelete.Enabled = true;
            btnSave.Text="Update";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetFormControls();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.ProductId > 0)
            {
                if (MessageBox.Show("Are You Sure ? \n Delete Product " + txtItem.Text + " from System.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                    {
                        string query = "DELETE FROM tblProducts WHERE ProductId='" + this.ProductId + "'";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Product Deleted from System!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControls();
                        }
                    }
                }
            }
        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);
        }

        private void txtCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);
        }

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }

        private void txtAlert_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }
    }
}
