using Buisness_Soluiton.General;
using Buisness_Soluiton.Screens.Templates;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Buisness_Soluiton.Screens
{
    public partial class Manage_Users :TemplateForm //MetroTemplate
    {
        public Manage_Users()
        {
            InitializeComponent();
        }
        public string UserName { get; set; }
        private void btnSave_Click(object sender, EventArgs e)
        {
            //Validation
            if (isValid())
            {
                //Inserting Values to database
                using(SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    string query;
                    if (this.UserName == string.Empty || this.UserName == null)
                        query = "Insert into tblUsers(UserName,Password) Values('" + txtUserName.Text.Trim() + "','" + txtPassword.Text.Trim() + "')";
                    else
                        query = "UPDATE tblUsers SET UserName='" + txtUserName.Text.Trim() + "', Password='" + txtPassword.Text.Trim() + "' WHERE UserName= '"+this.UserName+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User Saved/Updated Successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFormControls();
                    }
                }
            }
        }

        private void ResetFormControls()
        {
            btnDelete.Enabled = false;
            btnSave.Text = "Save User";
            this.UserName = string.Empty;
            txtPassword.Clear();
            txtUserName.Clear();
            txtSearch.Clear();
            txtUserName.Focus();
            PopulateUserGrid();
        }

        private bool isValid()
        {
            if (txtUserName.Text == string.Empty)
            {
                MessageBox.Show("UserName is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUserName.Focus();
                return false;
            }
            if (txtPassword.Text == string.Empty)
            {
                MessageBox.Show("Password is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Focus();
                return false;
            }
            if (this.UserName == string.Empty || this.UserName == null)//It means saving new record
            {
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                       string query = "select * from tblUsers where username='"+ txtUserName.Text.Trim()+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();

                        SqlDataReader sdr = cmd.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("UserName already Exsist! \n Plz Choose Different Name!", "UserName Exsist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        else
                            return true;
                    }
                }
            }
            return true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetFormControls();
        }

        private void Manage_Users_Load(object sender, EventArgs e)
        {
            PopulateUserGrid();
        }

        private void PopulateUserGrid()
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "select * from tblUsers where UserName like '%"+ txtSearch.Text.Trim()+"%'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        DataTable dtUser = new DataTable();
                        dtUser.Load(sdr);
                        GDVUser.DataSource = dtUser;
                    }
                    else
                        GDVUser.DataSource = null;
                }
            }
        }

        private void GDVUser_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(GDVUser.Rows.Count > 0)
            {
                this.UserName = GDVUser.CurrentRow.Cells[0].Value.ToString();
                txtUserName.Text = GDVUser.CurrentRow.Cells[0].Value.ToString();
                txtPassword.Text = GDVUser.CurrentRow.Cells[1].Value.ToString();

                btnDelete.Enabled = true;
                btnSave.Text = "Update";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(this.UserName != String.Empty)
            {
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    string query = "DELETE FROM tblUsers WHERE UserName='" + this.UserName + "'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("User Deleted from System!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFormControls();
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            PopulateUserGrid();
        }
    }
}
