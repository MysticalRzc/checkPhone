using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkPhone
{
    class Tools
    {
        private int widthImage;
        private int heightImage;
        private int[,] processedMatrix = null;
        public String openPath()
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            //folder.RootFolder = @"C:\Users\RZC\Desktop\手机后壳鉴定\Image";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                return folder.SelectedPath;
            }
            return null;
        }
        public Bitmap openImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = @"C:\Users\RZC\Desktop\手机后壳鉴定\Image";
            ofd.Title = "打开图像文件";
            ofd.Filter = "jpg file(*.jpg)|*.jpg|bmp file(*.bmp)|*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return new Bitmap(ofd.FileName);
            }
            else
                return null;
        }

        public TimeSpan imageToMatrix1(Bitmap processedImage, int[,] matrixImage, int heightImage, int widthImage)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitmapData data = processedImage.LockBits(new Rectangle(0, 0, widthImage, heightImage), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* point = (byte*)(data.Scan0);
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        matrixImage[i, j] = point[0];
                        point += 4;
                    }
                }
            }
            processedImage.UnlockBits(data);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts;
        }
        public TimeSpan imageToMatrix2(Bitmap processedImage, int[,] matrixImage, int heightImage, int widthImage)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                Rectangle rect = new Rectangle(0, 0, widthImage, heightImage);
                //System.Drawing.Imaging.BitmapData bmpData = waitDetection.LockBits(rect,ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                BitmapData srcData = processedImage.LockBits(new Rectangle(0, 0, widthImage, heightImage), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                IntPtr iPtr = srcData.Scan0;
                int iBytes = widthImage * heightImage * 4;
                byte[] PixelValues = new byte[iBytes];
                System.Runtime.InteropServices.Marshal.Copy(iPtr, PixelValues, 0, iBytes);
                processedImage.UnlockBits(srcData);
                matrixImage = new int[heightImage, widthImage];
                int iPoint = 0;
                for (int i = 0; i < heightImage; i++)
                {
                    for (int j = 0; j < widthImage; j++)
                    {
                        matrixImage[i, j] = Convert.ToInt32(PixelValues[iPoint++]);
                        matrixImage[i, j] = Convert.ToInt32(PixelValues[iPoint++]);
                        matrixImage[i, j] = Convert.ToInt32(PixelValues[iPoint++]);
                        iPoint++;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("矩阵填装有问题!");
            }
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts;
        }
        public TimeSpan matrixToImage(Bitmap resultImage, int[,] matrixImage, int heightImage, int widthImage)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitmapData data = resultImage.LockBits(new Rectangle(0, 0, widthImage, heightImage), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* point = (byte*)(data.Scan0);
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        point[0] = point[1] = point[2] = (byte)matrixImage[i, j];
                        point += 4;
                    }
                }
            }
            resultImage.UnlockBits(data);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts;
        }

        public TimeSpan resizeImage(ref Bitmap processedImage, int size)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            processedImage = new Bitmap(processedImage, size, size);
            TimeSpan ts = sw.Elapsed;
            return ts;
        }
        public TimeSpan removeNoise(int[,] matrixMap, int matrixHeight, int matrixWidth)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 1; i < matrixHeight - 1; i++)
            {
                for (int j = 1; j < matrixWidth - 1; j++)
                {
                    if (matrixMap[i, j] == 0)
                    {
                        bool flag = false;
                        for (int x = -1; x <= 1; x++)
                            for (int y = -1; y <= 1; y++)
                                if ((x != 0 || y != 0) && matrixMap[i + x, j + y] == 0)
                                    flag = true;
                        if (!flag)
                        {
                            matrixMap[i, j] = 1;
                        }
                    }
                }
            }
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts;
        }
        public int findPoint(Bitmap processedImage, int[,] matrixImage, int heightImage, int widthImage, int size, int threshold)
        {
            int hasPoint = 0;
            int[,] matrixResult = new int[heightImage, widthImage];
            int[,] map1 = new int[heightImage / size + 1, widthImage / size + 1];
            int[,] map2 = new int[heightImage / size + 1, widthImage / size + 1];
            for (int i = 0; i < heightImage; i += size)
            {
                for (int j = 0; j < widthImage; j += size)
                {
                    int sum = 0;
                    int count = 0;
                    for (int k = i; k < i + size && k < heightImage; k++)
                    {
                        for (int l = j; l < j + size && l < widthImage; l++)
                        {
                            sum += matrixImage[k, l];
                            count++;
                        }
                    }
                    sum /= count;
                    map1[i / size, j / size] = sum;
                }
            }

            int height = heightImage / size;
            int width = widthImage / size;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (!(i < 2 || j < 2 || i >= height - 2 || j >= width - 1))
                        if (map1[i, j] < map1[i - 2, j] - threshold || map1[i, j] < map1[i + 2, j] - threshold)
                        {
                            map2[i, j] = 0;
                            hasPoint = 1;
                        }
                        else
                            map2[i, j] = 1;
                    else
                        map2[i, j] = 1;
                }
            }
            if (hasPoint > 0)
            {
                this.removeNoise(map2, height, width);
                hasPoint = 0;
                matrixResult = new int[heightImage, widthImage];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (map2[i, j] == 1)
                            for (int k = i * size; k < (i + 2) * size && k < heightImage; k++)
                                for (int l = j * size; l < (j + 2) * size && l < widthImage; l++)
                                    matrixResult[k, l] = 255;
                        else
                        {
                            for (int k = i * size; k < (i + 2) * size && k < heightImage; k++)
                                for (int l = j * size; l < (j + 2) * size && l < widthImage; l++)
                                    matrixResult[k, l] = 0;
                            hasPoint = 1;
                        }
                    }
                }
                if (hasPoint == 1)
                    this.matrixToImage(processedImage, matrixResult, heightImage, widthImage);
            }
            return hasPoint;
        }
        public TimeSpan binarizationMatrix(int[,] processedMatrix, int heightImage, int widthImage, int size, int threshold)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int sum = 0;
            for (int i = 0; i < heightImage; i++)
            {
                for (int j = 0; j < widthImage; j++)
                {
                    sum += processedMatrix[i, j];
                }
            }
            sum /= (heightImage * widthImage);
            for (int i = 0; i < heightImage; i += size)
            {
                for (int j = 0; j < widthImage; j += size)
                {
                    int rangeSum = 0;
                    int count = 0;
                    for (int k = i; k < i + size && k < heightImage; k++)
                    {
                        for (int l = j; l < j + size && l < widthImage; l++)
                        {
                            rangeSum += processedMatrix[k, l];
                            count++;
                        }
                    }
                    rangeSum /= count;

                    for (int k = i; k < i + size && k < heightImage; k++)
                    {
                        for (int l = j; l < j + size && l < widthImage; l++)
                        {
                            if (processedMatrix[k, l] < (sum + rangeSum * 5) / 6 - threshold)
                                processedMatrix[k, l] = 0;
                            else
                                processedMatrix[k, l] = 255;
                        }
                    }
                }
            }
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts;
        }
        private int findDeep(int direction, int x, int y, int level)
        {
            int iWidth = widthImage;
            int iHeight = heightImage;
            if (x < 0 | y < 0 || x >= iWidth || y >= iHeight)
                return level;
            if (processedMatrix[y, x] < 20)
            {
                return findDeep(direction, (int)(x + Math.Cos(Math.PI * direction / 180) * 5), (int)(y + Math.Sin(Math.PI * direction / 180) * 5), level + 1);
            }
            else
                return level;
        }
        public TimeSpan findLine(ref Bitmap processedImage, int[,] processedMatrix, int widthImage, int heightImage)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int iWidth = widthImage;
            int iHeight = heightImage;

            int sum = 0;
            sum = sum / (iWidth * iHeight);

            Bitmap resultMap = new Bitmap(iWidth, iHeight);     //创建函数图像
            Graphics resultGraph = Graphics.FromImage(resultMap);
            resultGraph.DrawRectangle(new Pen(Color.Black, 0), 1, 1, iWidth - 2, iHeight - 2);
            for (int i = 0; i < iWidth; i += 10)
            {
                for (int j = 0; j < iHeight; j += 10)
                {
                    if (processedMatrix[j, i] < 20)
                    {
                        for (int k = 0; k < 360; k += 20)
                        {
                            int value = findDeep(k, i, j, 0);
                            if (value > 20)
                            {
                                resultGraph.DrawLine(new Pen(Color.Black, 0), i, j, (int)(i + Math.Cos(Math.PI * k / 180) * 5 * value), (int)(j + Math.Sin(Math.PI * k / 180) * 5 * value));
                            }
                        }
                    }
                }
            }
            processedImage = resultMap;
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts;
        }
    }
}