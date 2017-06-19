using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkPhone
{
    class Line
    {
        public List<LinePoint> upLine;  //左上角方向测线上为主，左为辅
        public List<LinePoint> downLine;//右下角方向的线
        public LinePoint midPoint;          //线条开始扩张的点

        public Line()
        {
            upLine = new List<LinePoint>();
            downLine = new List<LinePoint>();
        }
        public Line(LinePoint midPoint)
        {
            upLine = new List<LinePoint>();
            downLine = new List<LinePoint>();
            this.midPoint = midPoint;
        }
    }
}
