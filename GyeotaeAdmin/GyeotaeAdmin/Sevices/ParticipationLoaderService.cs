using ClosedXML.Excel;
using GyeotaeAdmin.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{

    public static class ParticipationLoaderService
    {
        public static List<ParticipationRecord> LoadMultipleFiles(string[] filePaths)
        {
            return LoadMultipleExcelFiles(filePaths);
        }

        public static List<ParticipationRecord> LoadMultipleExcelFiles(string[] filePaths)
        {
            var records = new List<ParticipationRecord>();

            foreach (var path in filePaths)
            {
                using var workbook = new XLWorkbook(path);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

                foreach (var row in rows)
                {
                    records.Add(new ParticipationRecord
                    {
                        Name = row.Cell(1).GetValue<string>(),
                        Phone = row.Cell(2).GetValue<string>(),
                        ProgramName = row.Cell(5).GetValue<string>(),
                        Participation = row.Cell(6).GetValue<string>()
                    });
                }
            }

            return records;
        }

        public static List<ParticipationSummary> TransformToSummary(List<ParticipationRecord> records)
        {
            var grouped = records.GroupBy(r => new { r.Name, r.Phone });
            var allPrograms = records.Select(r => r.ProgramName).Distinct();

            var summaries = new List<ParticipationSummary>();

            foreach (var group in grouped)
            {
                var summary = new ParticipationSummary
                {
                    Name = group.Key.Name,
                    Phone = group.Key.Phone
                };

                foreach (var program in allPrograms)
                {
                    // 현재 프로그램에 대한 참여 여부 확인
                    var record = group.FirstOrDefault(r => r.ProgramName == program);

                    if (record != null)
                    {
                        // 참여 여부가 "O"이면 1, 아니면 0
                        summary.ProgramParticipation[program] =
                            record.Participation.Trim().ToUpper() == "O" ? 1 : 0;
                    }
                    else
                    {
                        // 추천되지 않은 프로그램에 대해 null로 설정
                        summary.ProgramParticipation[program] = null;
                    }
                }

                summaries.Add(summary);
            }

            return summaries;
        }
    }


    public static class ParticipationTransformer
    {
        public static List<ExpandoObject> ToExpandoList(List<ParticipationSummary> summaries)
        {
            var result = new List<ExpandoObject>();

            foreach (var summary in summaries)
            {
                dynamic row = new ExpandoObject();
                var dict = (IDictionary<string, object>)row;

                dict["Name"] = summary.Name;
                dict["Phone"] = summary.Phone;

                foreach (var program in summary.ProgramParticipation)
                {
                    if (!string.IsNullOrWhiteSpace(program.Key))
                    {
                        var cleanKey = program.Key.Replace("\n", " ").Replace("\r", " ").Trim();
                        dict[cleanKey] = program.Value;
                    }
                }

                result.Add(row);
            }

            return result;
        }
    }
}