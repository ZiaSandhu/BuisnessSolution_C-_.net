using Buisness_Soluiton.General;
using Buisness_Soluiton.Screens.Templates;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Buisness_Soluiton.Screens
{
    public partial class AddIncome :TemplateForm //MetroTemplate
    {
        public AddIncome()
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
                    string query = "Insert into tblDailySheet(Title,Income,Note,date) Values('" + txtTitle.Text.Trim() + "','" + txtAmount.Text.Trim() + "','"+txtDesc.Text.Trim()+"',@date)";
                    
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@date", dateTimePicker1.Value.Date);
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Amount Added Successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFormControls();
                    }
                }
            }
        }

        private void ResetFormControls()
        {
            txtAmount.Clear();
            txtTitle.Clear();
            txtDesc.Clear();
            txtTitle.Focus();
        }

        private bool isValid()
        {
            if (txtTitle.Text == string.Empty)
            {
                MessageBox.Show("Title is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTitle.Focus();
                return false;
            }
            if (txtAmount.Text == string.Empty)
            {
                MessageBox.Show("Amount is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAmount.Focus();
                return false;
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

        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            General.General.NumberValidation(e);
        }
    }
}
