using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLibrary;
using Form2;
using System.IO;
namespace SpectrumAnalysis_1
{

    public partial class Form3 : Form
    {
        public Form1 parent;
        private Form1 form1Instance;
        private float mouseX;
        private float mouseY;
        private float scaleX;
        private float scaleY;
        public Form3()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            //Console.WriteLine(SharedClass.Vr);
            form1Instance = new Form1();
            this.Paint += Form3_paint;
            this.MouseMove += Form3_Mousemove; // 注册鼠标移动事件
        }
        public Form3(Form1 parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        //private void onload(object sender, EventArgs e)
        //{
        //    //Form2.AddFunctionButton(panel1, ByteArrayToImage(Properties.Resources.icon8));

        //}

       
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Task.Run(() =>
            {
                // 调用信号处理方法
                SharedClass.ProcessSignal("E:/Graduate Project/data/ATS9350_data");
                Console.WriteLine(SharedClass.Vr);
                // 更新UI线程上的显示
                this.Invoke((Action)(() =>
                {
                    this.Invalidate(); // 触发重绘
                }));
            });
        }

        private void Form3_paint(object sender, PaintEventArgs e)
        {
            if (SharedClass.Vr == null || SharedClass.gateNumbers == null)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 启用抗锯齿
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // 设置坐标系
            g.Clear(Color.White);
            g.TranslateTransform(50, height - 50);

            scaleX = (width- 200) / 100f;  // 横坐标范围为-30到30
            scaleY = (height - 200) / 40f;

            // 绘制横坐标的刻度和标签
            for (int i = -3; i <= 3; i++)
            {
                float xValue = 10 * i; // 计算每个刻度的真实值
                float xPos = (xValue + 30) * scaleX; // 将真实值映射到绘图位置

                // 绘制刻度线
                g.DrawLine(Pens.Black, xPos, 0, xPos, -5);

                // 绘制刻度标签
                g.DrawString(xValue.ToString("0"), this.Font, Brushes.Black, xPos - 10, 5);
            }

            // 绘制纵坐标的刻度和标签
            int yTickCount = 8; // 设定刻度数量
            for (int i = 0; i <= yTickCount; i++)
            {
                float yValue = i * 5; // 计算每个刻度的真实值
                float yPos = yValue * scaleY; // 将真实值映射到绘图位置

                // 绘制刻度线
                g.DrawLine(Pens.Black, 0, -yPos, -5, -yPos);

                // 绘制刻度标签
                g.DrawString(yValue.ToString("0"), this.Font, Brushes.Black, -30, -yPos - 5);
            }

            // 绘制数据点
            for (int i = 0; i < SharedClass.Vr.Length; i++)
            {
                float y = (float)SharedClass.gateNumbers[i] * scaleY;
                float x = (float)((SharedClass.Vr[i] + 30) * scaleX);
                g.FillEllipse(Brushes.Blue, x - 2, -y - 2, 4, 4);
            }

            // 画坐标轴
            g.DrawLine(Pens.Black, 0, 0, width - 300, 0); // x轴
            g.DrawLine(Pens.Black, 0, 0, 0, -(height - 150)); // y轴

            string xAxisLabel = "径向风速 Vr (m/s)";
            string yAxisLabel = "波门";

            // 横坐标标签 (绘制在靠近右下角)
            g.DrawString(xAxisLabel, this.Font, Brushes.Black, (width - 50) / 2, 0);

            // 纵坐标标签 (绘制在左侧)
            g.DrawString(yAxisLabel, this.Font, Brushes.Black, -40, -350);

            // 绘制鼠标位置对应的自定义坐标
            string mouseCoordinates = $"X: {mouseX:F1}, Y: {mouseY:F1}";
            g.DrawString(mouseCoordinates, this.Font, Brushes.Red, 10, -300);

            g.ResetTransform(); // 重置坐标系
                                // if (SharedClass.Vr == null || SharedClass.gateNumbers == null)
                                // {
                                //     return;
                                // }



            // Graphics g = e.Graphics;
            // g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 启用抗锯齿
            // int width = this.ClientSize.Width;
            // int height = this.ClientSize.Height;

            // // 设置坐标系
            // g.Clear(Color.White);
            // g.TranslateTransform(50, height - 50);

            // float scaleX = (width - 200) / (100f);  // 横坐标范围为-30到30
            // float scaleY = (height - 200) / 40f;
            // // 绘制横坐标的刻度和标签
            // //int xTickCount = 6; // 设定刻度数量
            // for (int i = -3; i <= 3; i++)
            // {
            //     float xValue = 10 * i; // 计算每个刻度的真实值
            //     float xPos = (xValue + 30) * scaleX;                     // 将真实值映射到绘图位置

            //     // 绘制刻度线
            //     g.DrawLine(Pens.Black, xPos, 0, xPos, -5);

            //     // 绘制刻度标签
            //     g.DrawString(xValue.ToString("0"), this.Font, Brushes.Black, xPos - 10, 5);
            // }

            // // 绘制纵坐标的刻度和标签
            // int yTickCount = 8; // 设定刻度数量
            // for (int i = 0; i <= yTickCount; i++)
            // {
            //     float yValue = i * 5; // 计算每个刻度的真实值
            //     float yPos = yValue * scaleY;              // 将真实值映射到绘图位置

            //     // 绘制刻度线
            //     g.DrawLine(Pens.Black, 0, -yPos, -5, -yPos);

            //     // 绘制刻度标签
            //     g.DrawString(yValue.ToString("0"), this.Font, Brushes.Black, -30, -yPos - 5);
            // }

            // for (int i = 0; i < SharedClass.Vr.Length; i++)
            // {
            //     float y = (float)SharedClass.gateNumbers[i] * scaleY;
            //     float x = (float)((SharedClass.Vr[i] + 30) * scaleX);
            //     Console.WriteLine(SharedClass.Vr[i]);
            //     g.FillEllipse(Brushes.Blue, x - 2, -y - 2, 4, 4);
            // }

            // // 画坐标轴
            // g.DrawLine(Pens.Black, 0, 0, width - 300, 0); // x轴
            // g.DrawLine(Pens.Black, 0, 0, 0, -(height - 150)); // y轴
            // string xAxisLabel = "径向风速 Vr (m/s)";
            // string yAxisLabel = "波门";

            // // 横坐标标签 (绘制在靠近右下角)
            // g.DrawString(xAxisLabel, this.Font, Brushes.Black, (width - 50) / 2, 0);

            // // 纵坐标标签 (绘制在左侧，旋转90度)
            //// g.RotateTransform(-90); // 旋转坐标系
            //                         //g.DrawString(yAxisLabel, this.Font, Brushes.Black, -(height - 200) / 2, 0);
            // g.DrawString(yAxisLabel, this.Font, Brushes.Black, -40, -350);
            // g.ResetTransform(); // 重置坐标系
        }

        private void Form3_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Form1.AddFunctionButton(panel1, Form1.ByteArrayToImage(Properties.Resources.icon8));
        }

        private void Form3_Mousemove(object sender, MouseEventArgs e)
        {
            // 将鼠标屏幕坐标转换为绘图坐标
            float transformedX = (e.X - 50) / scaleX; // X轴偏移了50
            float transformedY = -(e.Y - (this.ClientSize.Height - 50)) / scaleY; // Y轴翻转且偏移了高度

            // 更新全局变量
            mouseX = transformedX -30;
            mouseY = transformedY;

            // 触发重绘
            this.Invalidate();
        }
    }
}
