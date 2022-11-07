using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buisness_Soluiton.General
{
    public class General
    {
        public static string ConnectionString = @"Data Source=DESKTOP-4CCDK29;Initial Catalog=Buisness_Solution;Integrated Security=True";
        public static void NumberValidation(KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                e.Handled = false;
            else
                e.Handled = true;
        }
        public static string UserName { get; set; }
    }
  
}
