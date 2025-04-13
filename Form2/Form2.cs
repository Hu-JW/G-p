using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlazarTech;
using System.IO;
using NPT_WaitNextBuffer;
using SharedLibrary;
using System.Collections.Generic;
namespace Form2
{
    public partial class Form2 : Form

    {
        private float mouseX;
        private float mouseY;
        private float scaleX;
        private float scaleY;

        public Form2()
        {
            InitializeComponent();
            // 启用双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.Paint += Form2_Paint;
            this.MouseMove += Form2_MouseMove; // 注册鼠标移动事件
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 创建一个任务来异步调用 Connection 方法
           // List<short[]> allData = new List<short[]>();
            Task<List<double []>> acquireTask = Task.Run(() => AcqToDiskApp.AcquireData(AcqToDiskApp.handle));
            Console.WriteLine("acquire success");
            Task exportTask = acquireTask.ContinueWith((t) =>
            {
                // 获取采集的数据
                List<double[]> allData = t.Result;  // 获取从 acquireTask 返回的数据

                // 将数据传递给 ExportDataToExcel
                SharedClass.ExportDataToExcel(allData);
            });
            Task.WhenAll(acquireTask, exportTask).Wait();
            Task processTask = Task.Run(() => SharedClass.ProcessSignal("D:/HU/ATS9350_data/yuanshi1.xlsx"));

            // 等待所有任务完成
           
            // 使用 ContinueWith 获取数据并传递给 ExportDataToExcel
            
           
           
            
            this.Invoke((Action)(() =>
                {
                    this.Invalidate(); // 触发重绘
                }));
            // Task.Run(() =>
            {


                //    // 更新UI线程上的显示
                //    this.Invoke((Action)(() =>
                //    {
                //        this.Invalidate(); // 触发重绘
                //    }));
                //});
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void AddFunctionButton(Panel panel, Image icon)
        {
            PictureBox pictureBox = null;
            foreach (Control control in panel.Controls)
            {
                if (control is PictureBox)
                {
                    pictureBox = (PictureBox)control;
                    break;
                }
            }
            if (pictureBox != null)
            {
                pictureBox.Image = icon;
                // pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // 控制图片缩放
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            AddFunctionButton(panel1, ByteArrayToImage(Properties.Resources.icon8));



        }
        private Image ByteArrayToImage(byte[] byteArray)
        {

            using (var ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (SharedClass.dopplerShifts == null || SharedClass.gateNumbers == null)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 启用抗锯齿
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // 设置坐标系
            g.Clear(Color.White);
            g.TranslateTransform(50, height - 50);

            scaleX = (width - 100) / 200f;  // 横坐标范围为0到200
            scaleY = (height - 100) / 40f;  // 纵坐标范围为0到40

            // 绘制横坐标的刻度和标签
            int xTickCount = 20; // 设定刻度数量
            for (int i = 0; i <= xTickCount; i++)
            {
                float xValue = 200 * i / (float)xTickCount; // 计算每个刻度的真实值
                float xPos = xValue * scaleX; // 将真实值映射到绘图位置

                // 绘制刻度线
                g.DrawLine(Pens.Black, xPos, 0, xPos, -5);

                // 绘制刻度标签
                g.DrawString(xValue.ToString("0"), this.Font, Brushes.Black, xPos - 10, 5);
            }

            // 绘制纵坐标的刻度和标签
            int yTickCount = 20; // 设定刻度数量
            for (int i = 0; i <= yTickCount; i++)
            {
                float yValue = i * 40 / (float)yTickCount; // 计算每个刻度的真实值
                float yPos = yValue * scaleY; // 将真实值映射到绘图位置

                // 绘制刻度线
                g.DrawLine(Pens.Black, 0, -yPos, -5, -yPos);

                // 绘制刻度标签
                g.DrawString(yValue.ToString("0"), this.Font, Brushes.Black, -30, -yPos - 5);
            }

            // 绘制数据点
            for (int i = 0; i < SharedClass.dopplerShifts.Length; i++)
            {
                float y = (float)SharedClass.gateNumbers[i] * scaleY;
                //float x = (float)((SharedClass.dopplerShifts[i]) * scaleX);
                //Console.WriteLine(SharedClass.dopplerShifts[i]);
                //float x = (float)((SharedClass.correctedFrequency[i]) * scaleX);
                // Console.WriteLine(SharedClass.correctedFrequency[i]);
                float x = (float)((SharedClass.CorrectedFrequencytoarray[i]) * scaleX);
                Console.WriteLine(SharedClass.CorrectedFrequencytoarray[i]);
                g.FillEllipse(Brushes.Blue, x - 2, -y - 2, 4, 4);
            }

            // 画坐标轴
            g.DrawLine(Pens.Black, 0, 0, width - 100, 0); // x轴
            g.DrawLine(Pens.Black, 0, 0, 0, -(height - 100)); // y轴

            string xAxisLabel = "多普勒频移(MHZ)";
            string yAxisLabel = "波门";

            // 横坐标标签 (绘制在靠近右下角)
            g.DrawString(xAxisLabel, this.Font, Brushes.Black, (width + 200) / 2, -20);

            // 纵坐标标签 (绘制在左侧，旋转90度)
            g.DrawString(yAxisLabel, this.Font, Brushes.Black, -(height - 240) / 2, -240);

            // 绘制鼠标位置对应的自定义坐标
            string mouseCoordinates = $"X: {mouseX:F1}, Y: {mouseY:F1}";
            g.DrawString(mouseCoordinates, this.Font, Brushes.Red, 10, -200);

            g.ResetTransform(); // 重置坐标系
                                //if (SharedClass.dopplerShifts == null || SharedClass.gateNumbers == null)
                                //    return;

            //Graphics g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 启用抗锯齿
            //int width = this.ClientSize.Width;
            //int height = this.ClientSize.Height;

            //// 设置坐标系
            //g.Clear(Color.White);
            //g.TranslateTransform(50, height - 50);

            //// float scaleX = (width - 100) / (200f - 120f);  // 横坐标范围为0到200
            //float scaleX = (width - 100) / 200f;
            //float scaleY = (height - 100) / 40f;
            //// 绘制横坐标的刻度和标签
            //int xTickCount = 20; // 设定刻度数量
            //for (int i = 0; i <= xTickCount; i++)
            //{

            //    float xValue =  200  * i / (float)xTickCount; // 计算每个刻度的真实值
            //   // float xPos = (xValue - 120) * scaleX; 
            //   //float xPos = (xValue - 120) * scaleX;// 将真实值映射到绘图位置
            //    float xPos = xValue * scaleX;

            //    // 绘制刻度线
            //    g.DrawLine(Pens.Black, xPos, 0, xPos, -5);

            //    // 绘制刻度标签
            //    g.DrawString(xValue.ToString("0"), this.Font, Brushes.Black, xPos - 10, 5);
            //}

            //// 绘制纵坐标的刻度和标签
            //int yTickCount = 20; // 设定刻度数量
            //for (int i = 0; i <= yTickCount; i++)
            //{
            //    float yValue = i * 40 / (float)yTickCount; // 计算每个刻度的真实值
            //    float yPos = yValue * scaleY;              // 将真实值映射到绘图位置

            //    // 绘制刻度线
            //    g.DrawLine(Pens.Black, 0, -yPos, -5, -yPos);

            //    // 绘制刻度标签
            //    g.DrawString(yValue.ToString("0"), this.Font, Brushes.Black, -30, -yPos - 5);
            //}

            //for (int i = 0; i < SharedClass.dopplerShifts.Length; i++)
            //{
            //    float y = (float)SharedClass.gateNumbers[i] * scaleY;
            //    float x = (float)((SharedClass.dopplerShifts[i]) * scaleX);
            //    Console.WriteLine(SharedClass.dopplerShifts[i]);
            //    g.FillEllipse(Brushes.Blue, x - 2, -y - 2, 4, 4);
            //}

            //// 画坐标轴
            //g.DrawLine(Pens.Black, 0, 0, width - 100, 0); // x轴
            //g.DrawLine(Pens.Black, 0, 0, 0, -(height - 100)); // y轴
            //string xAxisLabel = "多普勒频移(MHZ)";
            //string yAxisLabel = "波门";

            //// 横坐标标签 (绘制在靠近右下角)
            //g.DrawString(xAxisLabel, this.Font, Brushes.Black, (width + 200) / 2, -20);

            //// 纵坐标标签 (绘制在左侧，旋转90度)
            //// g.RotateTransform(-90); // 旋转坐标系
            //g.DrawString(yAxisLabel, this.Font, Brushes.Black, -(height - 240) / 2, -240);
            ////g.DrawString(yAxisLabel, this.Font, Brushes.Black, -40, -350);
            //g.ResetTransform(); // 重置坐标系
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            // 将鼠标屏幕坐标转换为绘图坐标
            float transformedX = (e.X - 50) / scaleX; // X轴偏移了50
            float transformedY = -(e.Y - (this.ClientSize.Height - 50)) / scaleY; // Y轴翻转且偏移了高度

            // 更新全局变量
            mouseX = transformedX;
            mouseY = transformedY;

            // 触发重绘
            this.Invalidate();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


    }
}
