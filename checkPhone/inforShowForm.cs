using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkPhone
{
    public partial class InforShowForm : Form
    {
        public InforShowForm()
        {
            InitializeComponent();
        }
        public void inforShow(String infor)
        {
            this.Show();
            textBox1.AppendText(infor);
        }
    }
}
