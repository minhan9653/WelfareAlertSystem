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
                var headers = headerLine.Split(',');

                // 헤더명을 키로 하는 인덱스 맵
                var headerMap = headers
                    .Select((name, index) => new { name = name.Trim(), index })
                    .ToDictionary(x => x.name, x => x.index);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    var fields = line.Split(',');

                    try
                    {
                        string get(string columnName) =>
                            headerMap.ContainsKey(columnName) ? fields[headerMap[columnName]].Trim() : "";

                        DateTime.TryParse(get("시작 진행기간"), out DateTime startDate);
                        DateTime.TryParse(get("종료 진행기간"), out DateTime endDate);

                        list.Add(new ProgramModel
                        {
                            Title = get("제목"),
                            Category = get("카테고리"),
                            TargetAge = get("참여대상 연령"),
                            Region = get("지역(소속)"),
                            StartDate = startDate,
                            EndDate = endDate,
                            TargetGender = get("참여대상 성별"),
                            ApplyMethod = get("접수방법"),
                            ApplyLink = get("접수방법 링크"),
                            Contact = get("진행문의")
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
                Dictionary<string, int> headerMap = null;

                while (reader.Read())
                {
                    // 헤더 행 처리
                    if (headerMap == null)
                    {
                        headerMap = new Dictionary<string, int>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var header = reader.GetValue(i)?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
                            {
                                headerMap[header] = i;
                            }
                        }

                        continue; // 헤더는 스킵하고 다음 줄부터 처리
                    }

                    try
                    {
                        string get(string columnName)
                        {
                            if (headerMap.ContainsKey(columnName))
                                return reader.GetValue(headerMap[columnName])?.ToString()?.Trim();
                            return "";
                        }

                        DateTime.TryParse(get("시작 진행기간"), out DateTime startDate);
                        DateTime.TryParse(get("종료 진행기간"), out DateTime endDate);

                        list.Add(new ProgramModel
                        {
                            Title = get("제목"),
                            Category = get("카테고리"),
                            TargetAge = get("참여대상 연령"),
                            Region = get("지역(소속)"),
                            StartDate = startDate,
                            EndDate = endDate,
                            TargetGender = get("참여대상 성별"),
                            ApplyMethod = get("접수방법"),
                            ApplyLink = get("접수방법 링크"),
                            Contact = get("진행문의")
                        });
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return list;
        }

    }
}
