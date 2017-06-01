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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(Bitmap bitMap,String title)
        {
            InitializeComponent();
            pictureBox1.Image = bitMap;
            pictureBox1.Refresh();
            this.Text = title;
        }
    }
}
