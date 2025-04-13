using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.IO;
namespace SharedLibrary
{
    class ExcelExporter
    {
        public void ExportDataToExcel(List<short[]> allData)
        {
            // 目标保存路径
            string directoryPath = @"E:/Graduate Project/data/ATS9350_data";

            // 如果路径不存在，创建它
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

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

                        // 将当前缓冲区的数据写入 Excel 的一列
                        for (int rowIndex = 0; rowIndex < bufferData.Length; rowIndex++)
                        {
                            worksheet.Cell(rowIndex + 1, bufferIndex - startBufferIndex + 1).Value = bufferData[rowIndex];
                        }
                    }

                    // 保存 Excel 文件
                    string filePath = Path.Combine(directoryPath, $"data {fileIndex + 1}.xlsx");
                    workbook.SaveAs(filePath);
                }
            }

            Console.WriteLine($"数据已成功导出到 {fileCount} 个 Excel 文件中！");
        }
    }
}
