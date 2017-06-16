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
        //图像参数
        Bitmap processImage;
        int[,] matrixImage;
        int imageWidth;
        int imageHeight;
        FeedBack feedBack;
        //检测参数
        List<Point> line;

        public Card()
        { }
        public Card(Bitmap processImage, int[,] matrixImage, int imageWidth, int imageHeight)
        {
            tools = new Tools();

            this.processImage = processImage;
            this.matrixImage = matrixImage;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            feedBack = new FeedBack(processImage, imageHeight, imageWidth);
            line = new List<Point>();
        }
        public FeedBack check()
        {
            orientation();
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
        {
            int H, W;
            H = 0; W = 1000;

            for (H = 0; H <imageHeight-30; H++)
            {
                if (findLine(H, W) > 1000)
                {
                    Point point = new Point(H, W);
                    line.Add(point);
                    //sign(H, W);
                }
            }
            foreach (Point item in line)
            {
                sign(item.X, item.Y);
            }
            return true;
        }
        private void sign(int H,int W)
        {
            for (int i = H; i < H + 30 && i < imageHeight; i++)
            {
                for (int j = W; j < W + 30 && j < imageWidth; j++)
                {
                    matrixImage[i, j] = 255;
                }
            }
        }
        private int findLine(int H, int W)
        {
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