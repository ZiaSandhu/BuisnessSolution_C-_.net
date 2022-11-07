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
    public partial class RestoreDateForm : TemplateForm
    {
        public RestoreDateForm()
        {
            InitializeComponent();
        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = ofd.FileName;
                btnSave.Enabled = true;
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtPath.Text != string.Empty)
            {
                using (SqlConnection con = new SqlConnection(General.General.ConnectionString))
                {
                    string query = "use master; ALTER DATABASE Buisness_Solution SET Single_User WITH ROLLBACK IMMEDIATE ";
                    query += "RESTORE DATABASE Buisness_Solution FROM DISK = '" + txtPath.Text + "' WITH REPLACE;";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                       // cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Successfully RESTORED DataBase", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                this.Close();
            }
        }
    }
}
