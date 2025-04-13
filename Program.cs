//using System;
//using System.Collections.Concurrent;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using NPT_WaitNextBuffer;
//using SharedLibrary;
//using System.Collections.Generic;
//using AlazarTech;
//using System.Linq;
//class Program
//{
//    static ConcurrentQueue<float[]> sharedData = new ConcurrentQueue<float[]>();

//    static float[] CollectDataFromATS9350()
//    {
//        // 模拟数据采集
//        return new float[2000]; // 假设每次采集1024个数据点
//    }

//    static void PerformFFT(float[] data)
//    {
//        // 进行FFT处理的逻辑
//        Console.WriteLine("FFT Processed: " + data.Length);
//    }
//    static float[] convertshorttofloat(List <short[]> data)
//    {
//        List<short[]> rawData = data;

//        // 创建一个新的 List<float[]>，将 short[] 转换为 float[]
//        List<float[]> floatArray = new List<float[]>();

//        foreach (var buffer in rawData)
//        {
//            float[] floatBuffer = new float[buffer.Length];
//            for (int i = 0; i < buffer.Length; i++)
//            {
//                floatBuffer[i] = (float)buffer[i];  // 将 short 转换为 float
//            }
//            floatArray.Add(floatBuffer);
            
//        }
//        float[] convertedData = data.SelectMany(x => x.Select(i => (float)i)).ToArray();
//        //float[] convertedData = floatArray.ToArray();
//        return convertedData;
//    }
//    static void SaveDataToFile(float[] data)
//    {
//        // 将数据保存到文件
//        string filePath = "D:/HU/ATS9350_data/data1.xlsx";
//        File.AppendAllText(filePath, string.Join(",", data) + Environment.NewLine);
//    }

//    static void DataAcquisitionTask(CancellationToken token)
//    {

//        while (!token.IsCancellationRequested)
//        {
//            var data = AcqToDiskApp.AcquireData(AcqToDiskApp.handle);
//            sharedData.Enqueue(convertshorttofloat(data)); // 将数据放入共享队列
//            Thread.Sleep(10); // 模拟采集间隔
//        }
//    }

//    static void FFTProcessingTask(CancellationToken token)
//    {
//        while (!token.IsCancellationRequested)
//        {
//            string filePath = "D:/HU/ATS9350_data/data1.xlsx";
//            if (sharedData.TryDequeue(out var data)) // 从共享队列取出数据
//            {
//                SharedClass.ProcessSignal(filePath); // 对数据进行FFT处理
//                SaveDataToFile(data); // 将数据保存到文件
//            }
//            Thread.Sleep(10); // 模拟处理间隔
//        }
//    }

//    static void Main(string[] args)
//    {
//        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
//        CancellationToken token = cancellationTokenSource.Token;


//        // 启动数据采集任务
//        Task dataAcquisitionTask = Task.Run(() => DataAcquisitionTask(token));

//        // 启动FFT处理任务
//        Task fftProcessingTask = Task.Run(() => FFTProcessingTask(token));

//        // 主线程可以做其他任务，或者等待任务完成
//        Console.WriteLine("Press any key to stop...");
//        // Console.ReadKey();
//        Console.Read();

//        // 停止任务
//        cancellationTokenSource.Cancel();

//        // 等待任务完成
//        Task.WhenAll(dataAcquisitionTask, fftProcessingTask).Wait();
//        Console.WriteLine("Processing stopped.");
//    }
//}
