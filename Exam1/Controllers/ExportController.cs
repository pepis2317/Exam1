using ClosedXML.Excel;
using Exam1.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
namespace Exam1.Controllers
{
    [Route("api/export-logs")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly string logFilePath = $"logs/Log-{DateTime.Now.ToString("yyyyMMdd")}.txt";

        [HttpGet("excel-logs")]
        public IActionResult ExportLogsToExcel()
        {
            if (!System.IO.File.Exists(logFilePath))
            {
                return NotFound("Log file not found.");
            }
                
            List<string> logLines = ReadLogFileWithoutLocking(logFilePath);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Logs");
                worksheet.Cell(1, 1).Value = "Timestamp";
                worksheet.Cell(1, 2).Value = "Log Level";
                worksheet.Cell(1, 3).Value = "Message";

                int row = 2;
                foreach (var log in logLines)
                {
                    var logParts = ParseLog(log);
                    worksheet.Cell(row, 1).Value = logParts.TimeStamp;
                    worksheet.Cell(row, 2).Value = logParts.Level;
                    worksheet.Cell(row, 3).Value = logParts.Message;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",$"Log-{DateTime.Now.ToString("yyyyMMdd")}.xlsx");
                }
            }
        }
        [HttpGet("pdf-logs")]
        public IActionResult ExportLogsToPdf()
        {

            if (!System.IO.File.Exists(logFilePath))
            {
                return NotFound("Log file not found.");
            }
                
            string[] logLines;
            using (var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                logLines = streamReader.ReadToEnd().Split(Environment.NewLine);
            }

            Regex logPattern = new Regex(@"(?<Timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} \+\d{2}:\d{2}) \[(?<Level>\w+)\] (?<Message>.*)");

            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                document.Add(new Paragraph("System Logs").SetFontSize(16));

                Table table = new Table(3, true);
                table.AddHeaderCell("Timestamp");
                table.AddHeaderCell("Level");
                table.AddHeaderCell("Message");

                foreach (var line in logLines)
                {
                    Match match = logPattern.Match(line);
                    if (match.Success)
                    {
                        table.AddCell(match.Groups["Timestamp"].Value);
                        table.AddCell(match.Groups["Level"].Value);
                        table.AddCell(match.Groups["Message"].Value);
                    }
                }
                document.Add(table);
                document.Close();

                return File(stream.ToArray(), "application/pdf", $"Logs-{DateTime.Now:yyyyMMdd}.pdf");
            }
        }
        private List<string> ReadLogFileWithoutLocking(string path)
        {
            var lines = new List<string>();
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
                    
            }
            return lines;
        }
        private LogFormatModel ParseLog(string logLine)
        {
            int levelStartIndex = logLine.IndexOf(" [");

            if (levelStartIndex == -1)
            {
                return new LogFormatModel
                {
                    TimeStamp = "N/A",
                    Level = "N/A",
                    Message = logLine
                };
            }

            string timestamp = logLine.Substring(0, levelStartIndex).Trim();
            int levelEndIndex = logLine.IndexOf("]", levelStartIndex);

            string level = logLine.Substring(levelStartIndex + 2, levelEndIndex - levelStartIndex - 2);
            string message = logLine.Substring(levelEndIndex + 1).Trim();

            return new LogFormatModel
            {
                TimeStamp = timestamp,
                Level = level,
                Message = message
            };
        }


    }
}
