using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkPhone
{
    class LinePoint
    {
        private Point upPoint;   //左上角的点上为主作为辅

        public Point UpPoint
        {
            get { return upPoint; }
            set { upPoint = value; }
        }
        private Point downPoint; //右下角的店

        public Point DownPoint
        {
            get { return downPoint; }
            set { downPoint = value; }
        }
        public int Length;      //划痕宽度

        public LinePoint()
        {

        }
        
    }
}
