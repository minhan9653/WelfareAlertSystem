using ExcelDataReader;
using GyeotaeAdmin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{
    public interface IProgramLoaderService
    {
        List<ProgramModel> LoadFromFile(string filePath);
    }

    public class ProgramLoaderService : IProgramLoaderService
    {
        public List<ProgramModel> LoadFromFile(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            return ext == ".csv" ? LoadFromCsv(filePath) : LoadFromXlsx(filePath);
        }

        private List<ProgramModel> LoadFromCsv(string filePath)
        {
            var list = new List<ProgramModel>();

            using (var reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    var fields = line.Split(',');

                    if (fields.Length < 19) continue;

                    try
                    {
                        DateTime.TryParse(fields[8], out DateTime startDate);
                        DateTime.TryParse(fields[9], out DateTime endDate);

                        list.Add(new ProgramModel
                        {
                            Title = fields[1],
                            Category = fields[3],
                            TargetAge = fields[9],
                            Region = fields[4],
                            StartDate = startDate,
                            EndDate = endDate,
                            TargetGender = fields[10],
                            ApplyMethod = fields[16],
                            ApplyLink = fields[17],
                            Contact = fields[18]
                        });
                    }
                    catch { continue; }
                }
            }

            return list;
        }

        private List<ProgramModel> LoadFromXlsx(string filePath)
        {
            var list = new List<ProgramModel>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                bool isFirstRow = true;

                while (reader.Read())
                {
                    if (isFirstRow) { isFirstRow = false; continue; }
                    if (reader.FieldCount < 19) continue;

                    try
                    {
                        DateTime.TryParse(reader.GetValue(8)?.ToString(), out DateTime startDate);
                        DateTime.TryParse(reader.GetValue(9)?.ToString(), out DateTime endDate);

                        list.Add(new ProgramModel
                        {
                            Title = reader.GetValue(1)?.ToString(),
                            Category = reader.GetValue(3)?.ToString(),
                            TargetAge = reader.GetValue(9)?.ToString(),
                            Region = reader.GetValue(4)?.ToString(),
                            StartDate = startDate,
                            EndDate = endDate,
                            TargetGender =  reader.GetValue(10)?.ToString(),
                            ApplyMethod = reader.GetValue(16)?.ToString(),
                            ApplyLink = reader.GetValue(17)?.ToString(),
                            Contact = reader.GetValue(18)?.ToString()
                        });
                    }
                    catch { continue; }
                }
            }

            return list;
        }
    }
}
