using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClosedXML.Excel;
using MathNet.Numerics.IntegralTransforms;

using System.Numerics;
using System.IO;
using DocumentFormat.OpenXml.Drawing;
using ScottPlot;
using Fast_fourier_transform;

namespace SharedLibrary
{
    public class SharedClass
    {
        public static double[] dopplerShifts = new double[40]; // 设为 public 和 static 以便跨项目访问
        public static double[] gateNumbers = new double[40];
        public static double[] Vr = new double[40];
        public static double[] magnitudes = new double[40];
        public static List <double> CorrectedFrequency = new List<double>();
        public static double correctedFrequency;
        public static double[] CorrectedFrequencytoarray = new double[40];
        public static void ProcessSignal(string filepath,int sampleRate)
        {
            // 示例信号处理逻辑
            // var filePath = "E:/Graduate Project/data/tek0009CH2/tek0000CH.xlsx";
            var filePath = filepath;
            double[] hanningWindow = CreateHanningWindow(50);  // 使用汉宁窗以减少频谱泄漏

            // Step 1: Read the Excel file and compute averages
            List<double> rowAverages = CalculateRowAverages(filePath);



            // Step 2: Split data into 40 gates
            int gateSize = rowAverages.Count / 40;

            // Step 3: Perform FFT on each gate
            var plt = new ScottPlot.Plot(600, 400);
            var allFrequencies = new List<double>();
            var allMagnitudes = new List<double>();
            //dopplerShifts = new double[40];
            //double[] dopplerShifts1 = new double[40];
            //gateNumbers = new double[40];
            double[] gateData_windows = new double[50];
            //double[] Vr = new double[40];
            double[] autocorr = new double[50];
            var frequencies = Fourier.FrequencyScale(autocorr.Length, sampleRate);
            for (int i = 0; i < 40; i++)
            {
                int start = i * gateSize;
                var gateData = rowAverages.Skip(start).Take(gateSize).ToArray();
                for (int n = 0; n < 50; n++)
                {
                    gateData_windows[n] = gateData[n] * hanningWindow[n];

                }
                autocorr = AutoCorrelation(gateData_windows);
                // var frequencies = Fourier.FrequencyScale(autocorr.Length, sampleRate);
                //magnitudes = FFT(frequencies,autocorr, 50, 500000000);
                List<double> Autocorr = autocorr.ToList();
                process_data pd = new process_data(Autocorr);
                //var powerSpectrum = FFT(autocorr, 50, 500000000);
                // Step 4: Zero-padding

                var peakIndex = FindPeakIndex(frequencies, magnitudes, 0, sampleRate);
                // var peakIndex = FindPeakIndex(frequencies, powerSpectrum, 120000000, 200000000);
                Console.WriteLine(peakIndex);
                correctedFrequency = CalculateCentroidAroundPeak(frequencies, magnitudes, peakIndex, 8);  // Using 3 neighbors on each side
                CorrectedFrequency.Add(correctedFrequency);
               
               // Console.WriteLine(correctedFrequency/1000000);
               //dopplerShifts[i] = correctedFrequency / 1000000;
               //Console.WriteLine(dopplerShifts[i]);



               var Vr = dopplerShifts.Select(x => ((x - 160) * 1.55) / 2).ToArray();
                //Console.WriteLine(Vr[i]);
                gateNumbers[i] = i + 1;
                //var plt1 = new ScottPlot.Plot(600, 400);
                // plt1.Title($"Gate {i + 1}");
                //plt1.XLabel("Vr (m/s)");
                //plt1.YLabel("Gate Numbers");

                //plt1.AddScatter(Vr, gateNumbers, label: "Vr per Gate", lineWidth: 0);
                /// plt1.SetAxisLimits(xMin: -30, xMax: 30);

                //plt1.SaveFig($"D:/HU/result_picture/Vr/Vr.jpeg");
                double[] CorrectedFrequencytoarray = CorrectedFrequency.ToArray();
                //var plt2 = new scottplot.plot(600, 400);

                //plt2.xlabel("frequency");
                //plt2.ylabel("magnitude");

                // plt2.AddScatter(frequencies,magnitudes, label: "Vr per Gate", lineWidth: 1);
                //ComplexNumber [] outpt_signal_array = process_data.outpt_signal.ToArray();
                // 假设 pd 是一个已经计算好 FFT 的 process_data 对象


                // 例如在按钮点击事件中调用：
                FFTPlotForm.ShowFFTChart(pd.fft_output,sampleRate);
            

                //plt2.AddScatter(frequencies, outpt_signal_array, label: "Vr per Gate", lineWidth: 1);
                //plt.AddBar(magnitudes);
                // plt2.AddBar(frequencies, magnitudes,l);
                //plt2.SetAxisLimits(xMin: -80000000, xMax: 80000000);

               // plt2.SaveFig($"D:/HU/result_picture/frequency/frequency{i+1}.jpeg");


            }
        

            //MessageBox.Show("Processing Complete");

        }
        public static void ExportDataToExcel(List<double[]> allData)
        {
            // 目标保存路径
            string directoryPath = @"D:/HU/ATS9350_data";

            // 如果路径不存在，创建它
            //if (!Directory.Exists(directoryPath))
            //{
            //    Directory.CreateDirectory(directoryPath);
            //}
           
            int bufferCount = allData.Count;  // 总的缓冲区数量
            int fileCount = (int)Math.Ceiling((double)bufferCount / 10);  // 计算需要多少个 Excel 文件

            // 遍历缓冲区，按缓冲区分批次生成 Excel 文件
            for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
            {
                // 创建一个新的 Excel 工作簿
                using (var workbook = new XLWorkbook())
                {
                    // 创建一个新的工作表
                    var worksheet = workbook.Worksheets.Add("Data");

                    // 每个 Excel 文件最多包含 10 个缓冲区的数据
                    int startBufferIndex = fileIndex * 10;
                    int endBufferIndex = Math.Min(startBufferIndex + 10, bufferCount);

                    // 将数据写入 Excel 文件
                    for (int bufferIndex = startBufferIndex; bufferIndex < endBufferIndex; bufferIndex++)
                    {
                        var bufferData = allData[bufferIndex];  // 获取当前缓冲区的数据
                        //var bufferData = allData[bufferIndex].Select(Val => (Val -128.0)*10/127.0).ToArray();  // 获取当前缓冲区的数据
                        // 将当前缓冲区的数据写入 Excel 的一列
                        for (int rowIndex = 0; rowIndex < bufferData.Length; rowIndex++)
                        {
                            worksheet.Cell(rowIndex + 1, bufferIndex - startBufferIndex + 1).Value = bufferData[rowIndex];
                        }
                    }

                    // 保存 Excel 文件
                    string filePath = System.IO.Path.Combine(directoryPath, $"yuanshi{fileIndex + 1}.xlsx");
                    workbook.SaveAs(filePath);
                }
            }

            Console.WriteLine($"数据已成功导出到 {fileCount} 个 Excel 文件中！");
        }
    
    public static List<double> CalculateRowAverages(string filePath)
        {
            var averages = new List<double>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var totalRows = worksheet.RowsUsed().Count();
                //Console.WriteLine(totalRows);
                for (int row = 1; row <= totalRows; row++)
                {
                    var values = new List<double>();
                    for (int col = 1; col <= 1; col++)
                    {
                        var cellValue = worksheet.Cell(row, col).Value;
                        if (double.TryParse(cellValue.ToString(), out double numericValue))
                        {
                            values.Add(numericValue);
                        }
                        else
                        {
                            Console.WriteLine($"Unable to parse value in row {row}, column {col}, cell value: {cellValue}");
                        }
                    }
                    if (values.Count == 1)
                    {
                        averages.Add(values.Average());
                    }
                }
            }
            return averages;
        }
        //public static List<ComplexNumber> FFT(List<ComplexNumber> inpt_signal)
        //{
        //    int i;
        //    int N = inpt_signal.Count;
        //    if (N == 1)
        //        return inpt_signal;

        //    // Even array
        //    List<ComplexNumber> evenList = new List<ComplexNumber>();
        //    for (i = 0; i < (N / 2); i++)
        //    {
        //        evenList.Add(inpt_signal[2 * i]);
        //    }
        //    evenList = FFT(evenList);

        //    // Odd array
        //    List<ComplexNumber> oddList = new List<ComplexNumber>();
        //    for (i = 0; i < (N / 2); i++)
        //    {
        //        oddList.Add(inpt_signal[(2 * i) + 1]);
        //    }
        //    oddList = FFT(oddList);

        //    // Result
        //    ComplexNumber[] result = new ComplexNumber[N];

        //    for (i = 0; i < (N / 2); i++)
        //    {
        //        double w = (-2.0 * i * Math.PI) / N;
        //        ComplexNumber wk = new ComplexNumber(Math.Cos(w), Math.Sin(w));
        //        ComplexNumber even = evenList[i];
        //        ComplexNumber odd = oddList[i];

        //        result[i] = even + (wk * odd);
        //        result[i + (N / 2)] = even - (wk * odd);
        //    }
        //    return result.ToList();
        //}
        //public static double[] FFT(double[] frequency,double[] autocorr, int gateSize, double sampleRate)
        //{

        //    var paddedData = new Complex[gateSize];
        //    for (int j = 0; j < gateSize; j++)
        //    {
        //        paddedData[j] = new Complex(autocorr[j], 0);
        //    }


        //    Fourier.Forward(paddedData, FourierOptions.Matlab);
        //    // Step 6: Calculate frequency scale
        //    //var frequencies1 = Fourier.FrequencyScale(paddedData.Length, sampleRate);
        //    //var filteredFreqs = frequencies1.Take(paddedData.Length / 2).ToArray();
        //    var magnitudes = paddedData.Take(frequency.Length).Select(c => c.Magnitude).ToArray();
        //    magnitudes[0] = 0;
        //   // var filteredMags = paddedData.Take(paddedData.Length / 2).Select(c => c.Magnitude).ToArray();
        //    var powerspectrum = magnitudes.Select(m => m * m /50 ).ToArray();

        //    return magnitudes;
        //    //return powerspectrum;

        //}
        public static double[] WelchEstimate(double[] signal, int windowSize, int overlap, double sampleRate)
        {
            int step = windowSize - overlap;
            int totalSegments = (signal.Length - overlap) / step;

            double[] window = CreateHanningWindow(windowSize); // Assume this function generates a Hann window
            double[] powerSpectralDensity = new double[windowSize / 2];

            for (int seg = 0; seg < totalSegments; seg++)
            {
                var segment = new Complex[windowSize];
                var segment_windows = new double[windowSize];
                for (int i = 0; i < windowSize; i++)
                {
                    int index = seg * step + i;
                    if (index < signal.Length)
                    {
                        segment_windows[i] = signal[index] * window[i];
                        segment[i] = new Complex(segment_windows[i], 0);
                    }
                }

                // Call your existing FFT function here
                Fourier.Forward(segment, FourierOptions.Matlab);

                // Calculate power spectrum
                for (int i = 0; i < powerSpectralDensity.Length; i++)
                {
                    double magnitude = Math.Sqrt(segment[i].Real * segment[i].Real + segment[i].Imaginary * segment[i].Imaginary);
                    powerSpectralDensity[i] += magnitude * magnitude / totalSegments;
                }
            }

            // Normalize the PSD to the sample rate
            //for (int i = 0; i < powerSpectralDensity.Length; i++)
            //{
            //    powerSpectralDensity[i] /= sampleRate;
            //}

            return powerSpectralDensity;
        }
        static double[] CreateRectangularWindow(int length)
        {
            double[] window = new double[length];
            for (int i = 0; i < length; i++)
            {
                window[i] = 1.0;
            }
            return window;
        }
        static double[] CreateHanningWindow(int length)
        {
            double[] window = new double[length];
            for (int i = 0; i < length; i++)
            {
                window[i] = 0.5 - 0.5 * Math.Cos(2 * Math.PI * i / (length - 1)); // 汉宁窗的公式
            }
            return window;
        }
        static int FindPeakIndex(double[] frequencies, double[] magnitudes, double lowerBound, double upperBound)
        {
            int startIndex = Array.FindIndex(frequencies, f => f >= lowerBound);
            int endIndex = Array.FindIndex(frequencies, f => f >= upperBound);
            double maxMagnitude = 0;
            int peakIndex = startIndex;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (magnitudes[i] > maxMagnitude)
                {
                    maxMagnitude = magnitudes[i];
                    peakIndex = i;
                }
            }
            return peakIndex;
        }
        //static double CalculateCentroidAroundPeak(double[] frequencies, double[] magnitudes, int peakIndex, int neighborCount)
        //{
        //    int start = Math.Max(12, peakIndex - neighborCount);
        //    int end = Math.Min(20, peakIndex + neighborCount);
        //    double totalWeightedFrequency = 0;
        //    double totalWeightedIndex = 0;
        //    double totalMagnitude = 0;
        //    for (int i = start; i <= end; i++)
        //    {
        //        totalWeightedIndex += i * magnitudes[i];
        //        totalMagnitude += magnitudes[i];
        //        totalWeightedFrequency = totalWeightedIndex * 10000000 / totalMagnitude;
        //    }

        //    return totalWeightedFrequency;
        //}
        static double CalculateCentroidAroundPeak(double[] frequencies, double[] magnitudes, int peakIndex, int neighborCount)
        {
            int start = Math.Max(12, peakIndex - neighborCount);
            int end = Math.Min(20, peakIndex + neighborCount);
            double totalWeightedFrequency = 0;
            double totalWeightedIndex = 0;
            double totalMagnitude = 0;
            for (int i = start; i <= end; i++)
            {
                totalWeightedIndex += i * magnitudes[i];
                totalMagnitude += magnitudes[i];
                
            }
            totalWeightedFrequency = totalWeightedIndex * 10000000 / totalMagnitude;
            Console.WriteLine($"校正后的主瓣中心频率: {totalWeightedFrequency} Hz");
            return totalWeightedFrequency;
        }
        //自相关处理
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
        
    }
}
