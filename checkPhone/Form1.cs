using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace checkPhone
{
    public partial class Form1 : Form
    {
        private static Tools tools = new Tools();
        private Bitmap originalImage = null;
        private Bitmap processedImage = null;
        private int heightImage;
        private int widthImage;
        private int[,] matrixImage = null;
        private Thread thread = null;
        FileInfo[] fileInfor = null;

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        private void addImageToList(String path, ref int index, String imageName)
        {
            listView1.BeginUpdate();

            imageList1.Images.Add(processedImage);
            ListViewItem lvi = new ListViewItem();
            lvi.ImageIndex = index++;
            lvi.Text = imageName;
            listView1.Items.Add(lvi);

            imageList1.Images.Add(Image.FromFile(path));
            ListViewItem lvi2 = new ListViewItem();
            lvi2.ImageIndex = index++;
            lvi2.Text = "原图";
            listView1.Items.Add(lvi2);

            listView1.EndUpdate();

        }
        private void checkAll()
        {
            Stopwatch swAll = new Stopwatch();
            swAll.Start();
            listView1.Items.Clear();
            String path = tools.openPath();
            textBox1.AppendText(path + "\n");
            int size = (int)numericUpDown4.Value;
            int threshold = (int)numericUpDown8.Value;
            if (path != null)
            {
                int index = 0;
                DirectoryInfo folder = new DirectoryInfo(path);
                fileInfor = folder.GetFiles();
                foreach (FileInfo item in fileInfor)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    originalImage = new Bitmap(item.FullName);
                    originalImage = new Bitmap(originalImage, 1000, 1000);
                    pictureBox1.Image = originalImage;
                    pictureBox1.Refresh();

                    //划痕检测
                    this.recoverImage();
                    this.makeMatrix();
                    this.binarizationMatrix();
                    int hasLine = this.findLine();
                    if (hasLine != 0)
                    {
                        addImageToList(item.FullName, ref index, item.Name + " 存在划痕");
                        DateTime dt = DateTime.Now;
                        originalImage.Save(@"C:\Users\RZC\Desktop\手机后壳鉴定\save\" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-ffff}", dt) + "划痕原图.jpg");
                        processedImage.Save(@"C:\Users\RZC\Desktop\手机后壳鉴定\save\" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-ffff}", dt) + "划痕结果图.jpg");
                    }
                    else
                    {
                        //点痕检测
                        this.recoverImage();
                        this.makeMatrix();
                        int hasPoint = tools.findPoint(processedImage, matrixImage, heightImage, widthImage, size, threshold);

                        if (hasPoint == 1)
                        {
                            addImageToList(item.FullName, ref index, item.Name + " 存在点痕");
                            DateTime dt = DateTime.Now;
                            originalImage.Save(@"C:\Users\RZC\Desktop\手机后壳鉴定\save\" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-ffff}", dt) + "点痕原图.jpg");
                            processedImage.Save(@"C:\Users\RZC\Desktop\手机后壳鉴定\save\" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-ffff}", dt) + "点痕结果图.jpg");
                        }
                    }
                    sw.Stop();
                    TimeSpan ts = sw.Elapsed;
                    textBox1.AppendText("检测耗时" + ts.TotalMilliseconds + "\n");

                }
                swAll.Stop();
                TimeSpan tsAll = swAll.Elapsed;
                textBox1.AppendText("检测总耗时" + tsAll.TotalMilliseconds + "\n");
            }
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            thread = new Thread(checkAll);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            checkPoint();
            pictureBox2.Image = processedImage;
        }
        private void button14_Click(object sender, EventArgs e)
        {
            findLine();
            pictureBox2.Image = processedImage;
        }

        private void checkLine()
        {
            if (originalImage == null)
            {
                MessageBox.Show("请读取图片");
                return;
            }
            this.recoverImage();
            this.makeMatrix();
            this.binarizationMatrix();
            this.findLine();
            pictureBox2.Image = processedImage;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            checkLine();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //open Image
            originalImage = tools.openImage();
            if (originalImage == null)
            {
                return;
            }
            processedImage = new Bitmap(originalImage);
            pictureBox1.Image = originalImage;
            widthImage = processedImage.Width;
            heightImage = processedImage.Height;
            labelImageHeight.Text = heightImage.ToString();
            labelImageWidth.Text = widthImage.ToString();
        }
        private void recoverImage()
        {
            //recover Image  恢复
            if (originalImage == null)
            {
                MessageBox.Show("请读取图片");
                return;
            }
            processedImage = new Bitmap(originalImage);
            pictureBox1.Image = originalImage;
            widthImage = processedImage.Width;
            heightImage = processedImage.Height;
            labelImageHeight.Text = heightImage.ToString();
            labelImageWidth.Text = widthImage.ToString();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            this.recoverImage();
        }
        private TimeSpan makeMatrix()
        {
            TimeSpan ts = new TimeSpan();
            if (originalImage == null)
            {
                MessageBox.Show("请读取图片");
                return ts;
            }
            matrixImage = new int[heightImage, widthImage];
            ts = tools.imageToMatrix1(processedImage, matrixImage, heightImage, widthImage);
            return ts;
        }
        private void button11_Click(object sender, EventArgs e)
        {
            TimeSpan ts = this.makeMatrix();
            textBox1.AppendText("矩阵填装用时：" + ts.TotalMilliseconds + "ms");
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (processedImage == null)
            {
                MessageBox.Show("请读取图片");
                return;
            }
            int size = (int)numericUpDown1.Value * 10;

            TimeSpan ts = tools.resizeImage(ref processedImage, size);
            // processedImage = new Bitmap(SizeImage(processedImage, size, size));
            labelImageHeight.Text = processedImage.Height.ToString();
            labelImageWidth.Text = processedImage.Width.ToString();

            pictureBox1.Image = processedImage;
        }

        private int findPoint()
        {
            if (matrixImage == null)
            {
                MessageBox.Show("未填装矩阵");
                return 0;
            }
            int hasPoint;
            int size = (int)numericUpDown4.Value;
            int threshold = (int)numericUpDown8.Value;

            hasPoint = tools.findPoint(processedImage, matrixImage, heightImage, widthImage, size, threshold);

            return hasPoint;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            this.findPoint();
            pictureBox2.Image = processedImage;
            sw.Stop();
            TimeSpan ts = new TimeSpan();
            textBox1.AppendText("点痕检测完毕，耗时" + ts.TotalMilliseconds + "\n");
        }
        private TimeSpan binarizationMatrix()
        {
            TimeSpan ts = new TimeSpan();
            if (matrixImage == null)
            {
                MessageBox.Show("请填装矩阵!");
                return ts;
            }
            int size = (int)numericUpDown7.Value;
            int threshold = (int)numericUpDown5.Value;
            ts = tools.binarizationMatrix(matrixImage, heightImage, widthImage, size, threshold);
            tools.removeNoise(matrixImage, heightImage, widthImage);

            tools.matrixToImage(processedImage, matrixImage, heightImage, widthImage);
            return ts;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.binarizationMatrix();
            pictureBox2.Image = processedImage;
        }
        private int findDeep(int direction, int x, int y, int level)
        {
            int iWidth = widthImage;
            int iHeight = heightImage;
            if (x < 0 | y < 0 || x >= iWidth || y >= iHeight || level > 30)
                return level;
            if (matrixImage[y, x] < 20)
            {
                return findDeep(direction, (int)(x + Math.Cos(Math.PI * direction / 180) * 5), (int)(y + Math.Sin(Math.PI * direction / 180) * 5), level + 1);
            }
            else
                return level;
        }
        private int findLine()
        {
            int hasLine = 0;
            int iWidth = widthImage;
            int iHeight = heightImage;
            int flagDeep = (int)numericUpDown5.Value;

            Bitmap resultMap = new Bitmap(iWidth, iHeight);     //创建函数图像
            Graphics resultGraph = Graphics.FromImage(resultMap);

            this.Refresh();
            for (int i = 20; i < iWidth - 20; i += 10)
            {
                for (int j = 20; j < iHeight - 20; j += 10)
                {
                    if (matrixImage[j, i] < 20)
                    {
                        int count = 0;
                        for (int k = 0; k < 360; k += 20)
                        {
                            if (count > 4)
                                break;
                            int value = findDeep(k, i, j, 0);
                            if (value > 15)
                            {
                                count++;
                                hasLine++;
                                //resultGraph.DrawLine(new Pen(Color.Black, 0), i, j, (int)(i + Math.Cos(Math.PI * k / 180) * 5 * value), (int)(j + Math.Sin(Math.PI * k / 180) * 5 * value));
                                resultGraph.DrawLine(new Pen(Color.Black, 0), i, j, (int)(i + 1), (int)(j + 1));
                            }
                        }
                    }
                }
            }
            processedImage = resultMap;
            return hasLine;
        }
        private void checkPoint()
        {
            if (originalImage == null)
            {
                MessageBox.Show("请读取图片");
                return;
            }
            this.recoverImage();
            this.makeMatrix();
            this.findPoint();
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            int index = this.listView1.SelectedItems[0].Index;
            textBox1.AppendText(index + " ");
            pictureBox2.Image = imageList1.Images[index];
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (thread != null)
                thread.Abort();
        }
        private void addInforToList(List<ListViewItem> list)
        {
            listView1.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                listView2.Items.Add(list[i]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {    
            List<ListViewItem> list = new List<ListViewItem>();
            for(int i = 0;i< 5;i++)
            {
                ListViewItem lvi = new ListViewItem("a");
                lvi.SubItems.Add("asdf");
                list.Add(lvi);
            }
            addInforToList(list);
        }
    }
}
