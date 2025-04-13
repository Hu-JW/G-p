using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ClosedXML.Excel;
using MathNet.Numerics.IntegralTransforms;

using System.Numerics;
using ScottPlot;
using SharedLibrary;


namespace SpectrumAnalysis_1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //double[] noisySignal = signal();
            //double[] result = AutoCorrelation(noisySignal);
            //PlotSignal(result, "Autocorrelation of Signal", "Lag", "Correlation");
        }

        // 生成高斯噪声（均值为0，标准差为 noiseStdDev）
        static double GaussianNoise(Random random, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble(); // (0, 1)
            double u2 = 1.0 - random.NextDouble(); // (0, 1)
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // 标准正态分布
            return stdDev * randStdNormal; // 根据标准差调整噪声
        }
        public static double[] AutoCorrelation(double[] signal)
        {
            int n = signal.Length;
            double[] result = new double[n];

            for (int m = 0; m < n; m++)
            {
                double sum = 0;
                for (int i = 0; i < n - m; i++)
                {
                    sum += signal[i] * signal[i + m];
                }
                result[m] = sum / (n - m); // 标准化处理
            }

            return result;
        }
        public static double[] signal()
        {
            double frequency = 50.0;  // 正弦波频率，单位：Hz
            double amplitude = 1.0;   // 正弦波幅度
            double sampleRate = 1000.0;  // 采样率，单位：Hz
            double duration = 1.0;    // 信号持续时间，单位：秒
            double noiseStdDev = 0.2; // 噪声标准差

            // 计算采样点数
            int sampleCount = (int)(duration * sampleRate);

            // 创建一个数组存储生成的带噪声信号
            double[] signal = new double[sampleCount];

            // 随机数生成器
            Random random = new Random();

            // 生成含噪声的正弦波信号
            for (int i = 0; i < sampleCount; i++)
            {
                // 计算正弦波值
                double time = i / sampleRate;  // 当前时刻
                double sineWave = amplitude * Math.Sin(2 * Math.PI * frequency * time);

                // 添加高斯噪声
                double noise = GaussianNoise(random, noiseStdDev);

                // 存储带噪声的信号值
                signal[i] = sineWave + noise;
            }

            // 返回生成的信号数组
            return signal;
        }
        //static void PlotSignal(double[] data, string title, string xLabel, string yLabel)
        //{
        //    // 创建一个新的 Plot
        //    var plt = new ScottPlot.Plot(800, 600);

        //    // 添加信号数据到 Plot
        //    plt.AddSignal(data);

        //    // 设置图表标题和轴标签
        //    plt.Title(title);
        //    plt.XLabel(xLabel);
        //    plt.YLabel(yLabel);

        //    // 保存图像到文件或显示
        //    string fileName = "plot.png";
        //    plt.SaveFig(fileName); // 保存为 PNG 文件
        //    Console.WriteLine($"图像已保存到 {fileName}");

        //    // 如果需要显示在窗口中
        //    ScottPlot.WinForms.PlotViewer viewer = new ScottPlot.WinForms.PlotViewer(plt);
        //    viewer.ShowDialog();
        //}

    }
    
}


