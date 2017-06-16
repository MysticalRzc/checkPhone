using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkPhone
{
    class FeedBack
    {
        public List<ListViewItem> list;
        public Bitmap map;

        public FeedBack()
        {
            list = new List<ListViewItem>();
            map = null;
        }
        public FeedBack(Bitmap processImage,int imageWidth,int imageHeight)
        {
            list = new List<ListViewItem>();
            map = new Bitmap(processImage);
        }
    }
}
