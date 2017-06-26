using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkPhone
{
    class Natenna
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
        public Natenna()
        { }
        public Natenna(Bitmap processImage, int[,] matrixImage, int imageWidth, int imageHeight)
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
            orientation(1500);
            checkLine();
            //汇总检测结果
            for (int i = 0; i < lineList.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(i + "");
                lvi.SubItems.Add("衔接缝隙" + i);
                lvi.SubItems.Add(lineList[i].midPoint.Length * 0.0028 + "mm");
                feedBack.list.Add(lvi);
            }
            tools.matrixToImage(feedBack.map, matrixImage, imageHeight, imageWidth);
            return feedBack;
        }
        private Boolean orientation(int w)
        {//线条定位
            int H, W;
            H = 1; W = w;
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
                //    inforShow.inforShow(midPoint.Length + "\n");

                int j = midPoint.UpPoint.X - 50;
                j = j < 20 ? 20 : j;
                int sum = 0;
                for (; j < midPoint.UpPoint.X - 30; j++)
                {
                    sum += matrixImage[j, W] - matrixImage[j - 1, W];
                }
                int maxIndex = j;
                int minIndex = j;
                int[] rememberList = new int[10];
                int add = 0;
                int count = 0;
                int flag2 = 0;
                for (; j < midPoint.DownPoint.X + 50 && j < imageHeight; j++)
                {
                    if (count < 0)
                    {
                        count++;
                        add += sum;
                        rememberList[j % 6] = sum;
                    }
                    else
                    {
                        if (Math.Abs(add) > 100)
                        {
                            if (flag2 == 0)
                            {
                                if (add > 0)
                                {
                                    maxIndex = j;
                                    flag2 = 1;
                                }
                                else
                                {
                                    minIndex = j;
                                    flag2 = -1;
                                }
                            }
                            else
                            {
                                if (flag2 == 1)
                                    minIndex = j;
                                else
                                    maxIndex = j;
                            }
                        }
                        add -= rememberList[j % 6];
                        add += sum;
                        rememberList[j % 6] = sum;
                    }
                    sum -= matrixImage[j - 19, W] - matrixImage[j - 20, W];
                    sum += matrixImage[j, W] - matrixImage[j - 1, W];
                }
                midPoint.Length = Math.Abs(maxIndex - minIndex);
                //inforShow.inforShow("线段宽度：" + midPoint.Length * 0.003 + "mm\n");
                line.midPoint = midPoint;
                if (maxIndex < minIndex)
                {
                    int temp = maxIndex; maxIndex = minIndex; minIndex = temp;
                }
                line.midPoint.UpPoint = new Point(minIndex - 15, line.midPoint.UpPoint.Y);
                line.midPoint.DownPoint = new Point(maxIndex - 15, line.midPoint.DownPoint.Y);
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
                int begin, end;
                begin = end = 0;
                for (int i = item.midPoint.UpPoint.X; i < item.midPoint.DownPoint.X; i++)
                {
                    inforShow.inforShow(matrixImage[i, item.midPoint.UpPoint.Y] + "\n");
                   
                    if(matrixImage[i,item.midPoint.UpPoint.Y]<40)
                    {
                        if (begin == 0)
                            begin = i;
                        else
                            end = i;
                    }
                }
                int sum = 40;
                int value;
                int count = 1;
                int mid = (begin + end) / 2;
                inforShow.inforShow(mid+"\n");
                for (int i = item.midPoint.UpPoint.Y; i > 0; i--)
                {
                    int j, k;
                    for (j = mid; j <mid+100 && j< imageHeight; j++)
                    {
                        if (matrixImage[j, i] < 50)
                        {
                            matrixImage[j, i] = 255;
                        }
                    }
                    for (k = mid; k > mid-100 && k > 0; k--)
                    {
                        if (matrixImage[k, i] < 50)
                        {
                            matrixImage[k, i] = 255;
                        }
                    }
                    int mid2 = (j + k) / 2;
                    if (mid2 > mid)
                        mid++;
                    if (mid2 < mid)
                        mid--;

                }
                sum = 50;
                count = 1;
                for (int i = item.midPoint.UpPoint.Y; i < imageWidth; i++)
                {
                    value = sum / count;
                    sum = 0;
                    count = 1;
                    value = 55;
                    for (int j = item.midPoint.UpPoint.X; j < item.midPoint.DownPoint.X + 50; j++)
                    {
                        sum += matrixImage[j, i];
                        count++;
                        if (matrixImage[j, i] < value - 10)
                            matrixImage[j, i] = 255;
                    }
                }
            }
            return 0;
        }
        private int findLine(int H, int W)
        {//计算方差，判断是不是
            int sum = 0;
            int count = 0;
            int result = 0;
            for (int i = H; i < H + 30 && i < imageHeight; i++)
            {
                for (int j = W; j < W + 30 && j < imageWidth; j++)
                {
                    sum += matrixImage[i, j];
                    count++;
                }
            }
            sum /= count;
            for (int i = H; i < H + 30 && i < imageHeight; i++)
            {
                for (int j = W; j < W + 30 && j < imageWidth; j++)
                {
                    result = (int)Math.Pow(matrixImage[i, j] - sum, 2);
                }
            }
            return result;
        }
    }
}