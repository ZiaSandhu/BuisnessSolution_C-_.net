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
    public partial class BackupDataForm : TemplateForm
    {
        public BackupDataForm()
        {
            InitializeComponent();
        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog.SelectedPath;
                btnSave.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
            {
                string query = "Insert into Backuptable Values('"+txtPath.Text.Trim()+"')";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }

            }

            if (txtPath.Text != string.Empty)
            {
                using(SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    string query = "BACKUP DATABASE Buisness_Solution TO DISK='" + txtPath.Text + "\\Buisness_Solution " + DateTime.Now.Ticks.ToString() + ".bak'";
                    using(SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Successfully BackedUp DataBase", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                this.Close();
            }
        }
    }
}
