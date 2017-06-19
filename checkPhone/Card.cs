using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkPhone
{
    class Card
    {
        //使用工具
        Tools tools;
        InforShowForm inforShow;
        //图像参数
        Bitmap processImage;
        int[,] matrixImage;
        int imageWidth;
        int imageHeight;
        FeedBack feedBack;
        //检测参数
        List<Point> resemLineList;
        List<Line> lineList;
        public Card()
        { }
        public Card(Bitmap processImage, int[,] matrixImage, int imageWidth, int imageHeight)
        {
            tools = new Tools();
            inforShow = new InforShowForm();

            this.processImage = processImage;
            this.matrixImage = matrixImage;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            feedBack = new FeedBack(processImage, imageHeight, imageWidth);
            resemLineList = new List<Point>();
            lineList = new List<Line>();
        }
        public FeedBack check()
        {
            orientation();
            checkLine();
            //汇总检测结果
            for (int i = 0; i < 5; i++)
            {
                ListViewItem lvi = new ListViewItem(i + "");
                lvi.SubItems.Add("asdf" + i);
                feedBack.list.Add(lvi);
            }
            tools.matrixToImage(feedBack.map, matrixImage, imageHeight, imageWidth);
            return feedBack;
        }
        private Boolean orientation()
        {//线条定位
            int H, W;
            H = 1; W = 1000;
            //int sum = 0;
            //for (; H < 30; H++)
            //{
            //    sum += matrixImage[H, W] - matrixImage[H - 1, W];
            //}

            //for (H = 30; H < imageHeight - 30; H++)
            //{
            //    sum -= matrixImage[H - 29, W] - matrixImage[H - 30, W];
            //    sum += matrixImage[H, W] - matrixImage[H - 1, W];
            //    if (Math.Abs(sum) > 20)
            //    {
            //        Point point = new Point(H, W);
            //        resemLineList.Add(point);
            //    }
            //}
            foreach (Point item in resemLineList)
            {
                sign(item.X, item.Y);
            }


            for (H = 0; H < imageHeight - 30; H++)
            {
                int value = findLine(H, W);
                if (value > 400)
                {
                    Point point = new Point(H, W);
                    resemLineList.Add(point);
                }
            }
            for (int i = 0; i < resemLineList.Count; i++)
            {

                Line line = new Line();
                LinePoint midPoint = new LinePoint();
                midPoint.UpPoint = resemLineList[i];
                Boolean flag = true;
                while (flag && i + 1 < resemLineList.Count)
                {
                    if (Math.Abs(resemLineList[i + 1].X - resemLineList[i].X) < 10)
                        i++;
                    else
                        flag = false;
                }
                midPoint.DownPoint = resemLineList[i];
                midPoint.Length = midPoint.DownPoint.X - midPoint.UpPoint.X;
                inforShow.inforShow(midPoint.Length + "\n");
                int j = midPoint.UpPoint.X - 50;
                int sum = 0;
                for (; j <  midPoint.UpPoint.X - 30; j++)
                {
                    sum += matrixImage[j, W] - matrixImage[j - 1, W];
                }
                for (; j < midPoint.DownPoint.X + 30; j++)
                {
                    sum += matrixImage[j - 19, W] - matrixImage[j - 20, W];
                    sum += matrixImage[j, W] - matrixImage[j - 1, W];
                    inforShow.inforShow(sum + "\n");
                }
                inforShow.inforShow(">>>>>>>>>>>>>>>>>>\n\n");
                line.midPoint = midPoint;


                lineList.Add(line);
            }
            foreach (Line item in lineList)
            {
                sign(item.midPoint.UpPoint.X, item.midPoint.UpPoint.Y);
                sign(item.midPoint.DownPoint.X, item.midPoint.DownPoint.Y);
            }
            return true;
        }
        private void sign(int H, int W)
        {//标记
            for (int i = H; i < H + 3 && i < imageHeight; i++)
            {
                for (int j = W; j < W + 10 && j < imageWidth; j++)
                {
                    matrixImage[i, j] = 255;
                }
            }
        }
        private int checkLine()
        {
            foreach (Line item in lineList)
            {
                for (int i = item.midPoint.UpPoint.X; i < 100; i++)
                {

                }
            }

            return 0;
        }
        private int findLine(int H, int W)
        {//计算方差，判断是不是
            int sum = 0;
            int count = 0;
            int result = 0;
            for (int i = H; i < H + 10 && i < imageHeight; i++)
            {
                for (int j = W; j < W + 10 && j < imageWidth; j++)
                {
                    sum += matrixImage[i, j];
                    count++;
                }
            }
            sum /= count;

            for (int i = H; i < H + 10 && i < imageHeight; i++)
            {
                for (int j = W; j < W + 10 && j < imageWidth; j++)
                {
                    result = (int)Math.Pow(matrixImage[i, j] - sum, 2);
                }
            }
            return result;
        }
    }
}