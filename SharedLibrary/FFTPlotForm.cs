using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Fast_fourier_transform;
public class FFTPlotForm : Form
{
    public FFTPlotForm(List<ComplexNumber> fftResult,int sampleRate)
    {
        this.Text = "FFT 结果绘图";
        this.Width = 800;
        this.Height = 600;

        Chart chart = new Chart();
        chart.Dock = DockStyle.Fill;

        ChartArea chartArea = new ChartArea();
        chartArea.AxisX.Title = "MHz";
        chartArea.AxisY.Title = "Magnitude";
        chartArea.AxisX.IsStartedFromZero = true;
        chartArea.AxisX.Minimum = 0;
        chartArea.AxisX.Interval = 10;
        chart.ChartAreas.Add(chartArea);

        Series series = new Series();
        series.ChartType = SeriesChartType.Line;
        series.BorderWidth = 2;  // 线条宽度
        chart.Series.Add(series);

        // 遍历 FFT 结果，计算幅值后加入数据点
        for (int i = fftResult.Count / 2; i < fftResult.Count; i++)
        {
            double magnitude = fftResult[i].GetMagnitude();
            series.Points.AddXY((i - fftResult.Count / 2) * sampleRate / 64, magnitude);
        }
       
        this.Controls.Add(chart);
    }

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // FFTPlotForm
            // 
            this.ClientSize = new System.Drawing.Size(276, 236);
            this.Name = "FFTPlotForm";
            this.Load += new System.EventHandler(this.FFTPlotForm_Load);
            this.ResumeLayout(false);

    }

    private void FFTPlotForm_Load(object sender, EventArgs e)
    {

    }
    public static void ShowFFTChart(List<ComplexNumber> fftData,int sampleRate)
    {
        // 创建 FFTPlotForm 窗体的实例，将 FFT 数据传入构造函数
        FFTPlotForm plotForm = new FFTPlotForm(fftData,sampleRate);

        // 以模式对话框方式显示（调用 Show() 则为非模态窗体）
        plotForm.ShowDialog();

        // 如果你希望窗口不阻塞调用线程，可以使用 plotForm.Show();
    }
}
