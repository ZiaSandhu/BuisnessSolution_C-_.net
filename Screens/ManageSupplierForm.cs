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
using System.Windows.Forms;

namespace Buisness_Soluiton.Screens
{
    public partial class ManageSupplierForm : TemplateForm
    {
        public ManageSupplierForm()
        {
            InitializeComponent();
        }
        public int SupplierID { get; set; }
        private void ManageCustomerForm_Load(object sender, EventArgs e)
        {
            ResetFormControls();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Validation
            if (isValid())
            {
                //Inserting Values to database
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    string query;
                    bool SupStatus = (checkIsActive.Checked == true) ? true : false;
                    if (this.SupplierID == 0) //this mean no record selected
                        query = "INSERT INTO tblSupplier(Name,Phone,Email,Address,isActive,RegDate) VALUES('" + txtName.Text.Trim() + "','" + txtPhone.Text.Trim() + "','"+ txtMail.Text.Trim() + "','"+ txtAddress.Text.Trim() + "','"+ SupStatus + "',@date)";
                    else
                        query = "UPDATE tblSupplier SET Name='" + txtName.Text.Trim() + "', Phone='" + txtPhone.Text.Trim() + "',Email='" + txtMail.Text.Trim() + "',Address ='"+ txtAddress.Text.Trim() + "',isActive ='"+ SupStatus + "' WHERE SupplierId= '" + this.SupplierID + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (this.SupplierID == 0)
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@date", System.DateTime.Now); 
                        }
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Supplier Saved/Updated Successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFormControls();
                    }
                }
            }
        }

        private void ResetFormControls()
        {
            btnDelete.Enabled = false;
            btnSave.Text = "Save";

            this.SupplierID = 0;
            txtMail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtName.Text= string.Empty; ;
            txtAddress.Text = string.Empty;
            PopulateSupplierGrid();
        }

        private bool isValid()
        {
            if (txtName.Text == string.Empty)
            {
                MessageBox.Show("Supplier Name is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();
                return false;
            }
            if (txtPhone.Text == string.Empty)
            {
                MessageBox.Show("Phone No. is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPhone.Focus();
                return false;
            }
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select * from tblSupplier where name='" + txtName.Text.Trim() + "'";
                if (this.SupplierID > 0)
                    query = "select * from tblSupplier where name='" + txtName.Text.Trim() + "' and SupplierId !=" + this.SupplierID;
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();

                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        MessageBox.Show("Supplier Name already Exsist! \n Plz Choose Different Name!", "Supplier Exsist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                        return true;
                }
            }
            //return true;
        }
        private void PopulateSupplierGrid()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query;
                if (txtSearch.Focused)
                    query = "select * from tblSupplier where Name like '%" + txtSearch.Text.Trim() + "%'";
                else
                    query = "select * from tblSupplier order by Name";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dtSup = new DataTable();
                        dtSup.Load(sdr);
                        GDVSupplier.DataSource = dtSup;
                        GDVSupplier.Columns[0].Visible = false;
                    }
                    else
                        GDVSupplier.DataSource = null;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetFormControls();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            PopulateSupplierGrid();
        }

        private void GDVCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GDVSupplier.Rows.Count > 0)
            {
                this.SupplierID = Convert.ToInt32(GDVSupplier.CurrentRow.Cells[0].Value);
                txtName.Text = GDVSupplier.CurrentRow.Cells[1].Value.ToString();
                txtPhone.Text = GDVSupplier.CurrentRow.Cells[2].Value.ToString();
                txtMail.Text = GDVSupplier.CurrentRow.Cells[3].Value.ToString();
                txtAddress.Text = GDVSupplier.CurrentRow.Cells[4].Value.ToString();
                checkIsActive.Checked = Convert.ToBoolean(GDVSupplier.CurrentRow.Cells[5].Value);

                btnDelete.Enabled = true;
                btnSave.Text = "Update";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.SupplierID > 0)
            {
                if (MessageBox.Show("Are You Sure ? \n Delete Supplier " + txtName.Text + " from System.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)

                {
                    using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                    {
                        string query = "DELETE FROM tblSupplier WHERE SupplierId='" + this.SupplierID + "'";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Supplier Deleted from System!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetFormControls();
                        }
                    } 
                }
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);

        }
    }
}
