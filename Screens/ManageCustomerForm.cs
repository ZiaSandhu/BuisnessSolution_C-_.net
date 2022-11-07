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
    public partial class ManageCustomerForm : TemplateForm
    {
        public ManageCustomerForm()
        {
            InitializeComponent();
        }
        public int CustomerID { get; set; }
        private void ManageCustomerForm_Load(object sender, EventArgs e)
        {
            PopulateCustomerGrid();
            //ResetFormControls();
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
                    bool CusStatus = (checkIsActive.Checked == true) ? true : false;
                    if (this.CustomerID == 0) //this mean no record selected
                        query = "INSERT INTO tblCustomer(Name,Phone,Email,Address,isActive,RegDate) VALUES('" + txtName.Text.Trim() + "','" + txtPhone.Text.Trim() + "','"+ txtMail.Text.Trim() + "','"+ txtAddress.Text.Trim() + "','"+ CusStatus + "',@date)";
                    else
                        query = "UPDATE tblCustomer SET Name='" + txtName.Text.Trim() + "', Phone='" + txtPhone.Text.Trim() + "',Email='" + txtMail.Text.Trim() + "',Address ='"+ txtAddress.Text.Trim() + "',isActive ='"+ CusStatus + "' WHERE CustomerId= '" + this.CustomerID + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (this.CustomerID == 0)
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@date", System.DateTime.Now);
                        }
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Customer Saved/Updated Successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFormControls();
                    }
                }
            }
        }

        private void ResetFormControls()
        {
            btnDelete.Enabled = false;
            btnSave.Text = "Save";

            this.CustomerID = 0;
            txtMail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtName.Text= string.Empty; ;
            txtAddress.Text = string.Empty;
            PopulateCustomerGrid();
        }

        private bool isValid()
        {
            if (txtName.Text == string.Empty)
            {
                MessageBox.Show("Customer Name is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string query = "select * from tblCustomer where name='" + txtName.Text.Trim() + "'";
                if (this.CustomerID > 0)//updating same name 
                    query = "select * from tblCustomer where name='" + txtName.Text.Trim() + "' and CustomerId !=" + this.CustomerID;
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();

                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        MessageBox.Show("Customer Name already Exsist! \n Plz Choose Different Name!", "Customer Exsist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                        return true;
                }
            }
            //return true;
        }
        private void PopulateCustomerGrid()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select * from tblCustomer where Name like '%" + txtSearch.Text.Trim() + "%'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dtCus = new DataTable();
                        dtCus.Load(sdr);
                        GDVCustomer.DataSource = dtCus;
                        GDVCustomer.Columns[0].Visible = false;
                    }
                    else
                        GDVCustomer.DataSource = null;
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
            PopulateCustomerGrid();
        }

        private void GDVCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GDVCustomer.Rows.Count > 0)
            {
                this.CustomerID = Convert.ToInt32(GDVCustomer.CurrentRow.Cells[0].Value);
                txtName.Text = GDVCustomer.CurrentRow.Cells[1].Value.ToString();
                txtPhone.Text = GDVCustomer.CurrentRow.Cells[2].Value.ToString();
                txtMail.Text = GDVCustomer.CurrentRow.Cells[3].Value.ToString();
                txtAddress.Text = GDVCustomer.CurrentRow.Cells[4].Value.ToString();
                checkIsActive.Checked = Convert.ToBoolean(GDVCustomer.CurrentRow.Cells[5].Value);

                btnDelete.Enabled = true;
                btnSave.Text = "Update";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.CustomerID > 0)
            {
                if (MessageBox.Show("Are You Sure ? \n Delete Customer " + txtName.Text + " from System.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                    {
                        string query = "DELETE FROM tblCustomer WHERE CustomerId='" + this.CustomerID + "'";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            if (con.State != ConnectionState.Open)
                                con.Open();

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Customer Deleted from System!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
