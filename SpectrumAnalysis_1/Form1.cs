using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using ClosedXML.Excel;
using MathNet.Numerics.IntegralTransforms;

using System.Numerics;
using ScottPlot;

namespace SpectrumAnalysis_1
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
           
        }
       
        
        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            Form2.Form2 form2 = new Form2.Form2();
            form2.ShowDialog();


            //tableLayoutPanel1.Visible = false;

            //PictureBox pictureBox = new PictureBox();
            //pictureBox.Size = new Size(40, 30);
            //pictureBox.Image = Image.FromFile("E:/Graduate Project/result_picture/Vr/Vr.jpeg");
            //pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            //pictureBox.Dock = DockStyle.Fill;

            //this.Controls.Add(pictureBox);
         
            






        }

    private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            SetFunctionButton(panel3, label2, "参数设置", Properties.Resources.icon2);
            SetFunctionButton(panel4, label3, "多普勒频移显示", Properties.Resources.windSpeedIcon);
            SetFunctionButton(panel5, label4, "风场反演显示", ByteArrayToImage(Properties.Resources.icon4));
            SetFunctionButton(panel6, label5, "径向风速显示", ByteArrayToImage(Properties.Resources.icon5));
            SetFunctionButton(panel7, label6, "数据库管理", ByteArrayToImage(Properties.Resources.icon6));
            SetFunctionButton(panel8, label7, "关于我们", ByteArrayToImage(Properties.Resources.icon7));
            AddFunctionButton(panel2, ByteArrayToImage(Properties.Resources.icon8));
            AddFunctionButton(panel1,ByteArrayToImage(Properties.Resources.icon1));
        }
        public static Image ByteArrayToImage(byte[] byteArray) 
        { 
            
            using (var ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        //private void AddFunctionButton(Panel panel, string text, Image icon)
        //{
        //    // 清除 Panel 内的其他控件，确保不重复添加
        //    panel.Controls.Clear();

        //    // 创建 PictureBox 显示图标
        //    PictureBox pictureBox = new PictureBox();
        //    pictureBox.Image = icon;  // 设置 PictureBox 的图片
        //    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;  // 控制图片缩放
        //    pictureBox.Dock = DockStyle.Top;  // 将 PictureBox 放在 Panel 顶部
        //    pictureBox.Height = 80; // 设置 PictureBox 的高度
        //    pictureBox.Name = "FunctionIcon"; // 设置唯一名称

        //    // 将 PictureBox 添加到 Panel 中
        //    panel.Controls.Add(pictureBox);

        //    // 创建 Label 显示文字
        //    Label label = new Label();
        //    label.Text = text;
        //    label.TextAlign = ContentAlignment.MiddleCenter;
        //    label.Dock = DockStyle.Bottom;

        //    // 将 Label 添加到 Panel 中
        //    panel.Controls.Add(label);
        //}

         public static void SetFunctionButton(Panel panel, Label label, string text, Image icon)
        {
            // 创建 PictureBox 显示图标
            PictureBox pictureBox = null;
            foreach (Control control in panel.Controls)
            {
                if (control is PictureBox)
                {
                    pictureBox = (PictureBox)control;
                    break;
                }
            }

            // 如果找到了 PictureBox，设置其属性
            if (pictureBox != null)
            {
                pictureBox.Image = icon;
               // pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // 控制图片缩放
            }

            // 设置 Label 文本
            label.Text = text;
        }
         public static void AddFunctionButton(Panel panel,Image icon)
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

            private void pictureBox4_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        //private void pictureBox2_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}

        private void label5_Click(object sender, EventArgs e)
        {
            Form3 form3Instance = new Form3(this);
            form3Instance.ShowDialog();
            // //以图片的形式显示
            // tableLayoutPanel1.Visible = false;

            // PictureBox pictureBox = new PictureBox();
            //// pictureBox.Size = new Size(40, 30);
            // pictureBox.Image = Image.FromFile("E:/Graduate Project/result_picture/DopplerSpectrum/DopplerSpectrum.png");
            // pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            // pictureBox.Dock = DockStyle.Fill;

            // this.Controls.Add(pictureBox);

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click_2(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox4_Click_3(object sender, EventArgs e)
        {

        }
    }
}
