using Buisness_Soluiton.General;
using Buisness_Soluiton.Screens.Templates;
using System;
using System.Collections;
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
    public partial class LoginForm :TemplateForm
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
               //Validating Information
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    string query = "select * from tblUsers where UserName ='"+ txtUserName.Text.Trim()+"' and Password= '"+ txtPassword.Text.Trim()+"'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        SqlDataReader sdr = cmd.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            sdr.Close();
                            //creating login history
                            query = "INSERT INTO tblLoginHistory VALUES('" + txtUserName.Text.Trim() + "',@date)";
                            using (SqlCommand cmd1 = new SqlCommand(query, con))
                            {
                                cmd1.CommandType = CommandType.Text;
                                cmd1.Parameters.AddWithValue("@date", System.DateTime.Now);
                                if (con.State != ConnectionState.Open)
                                    con.Open();
                                cmd1.ExecuteNonQuery();
                            }
                            //Checking IsDay Start
                            query = "select * from tblLoginHistory WHERE  CAST(date AS DATE) = CAST( getdate() AS DATE)";
                            using (SqlCommand cmd2 = new SqlCommand(query, con))
                            {
                                if (con.State != ConnectionState.Open)
                                    con.Open();
                                SqlDataReader sdr1 = cmd2.ExecuteReader();
                                if (sdr1.HasRows) // If Start
                                {
                                    DataTable dt = new DataTable();
                                    dt.Load(sdr1);
                                    if(dt.Rows.Count == 1)
                                    {
                                        sdr1.Close();
                                        //Insert to daily transaction at start of the day
                                        string query1 = "INSERT INTO tblDailySheet(title,date) Values('Today_Sale',@date),('Yesterday_Balance',@date)";
                                        using (SqlCommand cmd3 = new SqlCommand(query1, con))
                                        {
                                            cmd3.CommandType = CommandType.Text;
                                            cmd3.Parameters.AddWithValue("@date", System.DateTime.Now);
                                            if (con.State != ConnectionState.Open)
                                                con.Open();
                                            cmd3.ExecuteNonQuery();
                                        }
                                    }
                                    
                                }
                                
                            }
                            GoToDashboard();
                        }
                        else
                            MessageBox.Show("UserName & Password is Incorrect!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void GoToDashboard()
        {
            this.Hide();
            DashboardForm df = new DashboardForm();
            General.General.UserName = txtUserName.Text.Trim();
            df.Show();
        }

        void DayStart()
        { 
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
            return true;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkShowpass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkShowpass.Checked == true)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
                txtPassword.UseSystemPasswordChar = true;
        }
    }
}
