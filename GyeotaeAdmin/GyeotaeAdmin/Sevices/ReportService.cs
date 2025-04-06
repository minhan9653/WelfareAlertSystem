using ClosedXML.Excel;
using GyeotaeAdmin.Models;
using GyeotaeAdmin.ViewModels;
using Excel = Microsoft.Office.Interop.Excel;

using Microsoft.Win32;


namespace GyeotaeAdmin.Sevices
{
    public static class ReportService
    {
        public static List<ProgramStatEntry> GenerateProgramStatistics(List<ParticipationSummary> users)
        {
            var result = new List<ProgramStatEntry>();

            int totalUsers = users.Count;
            if (totalUsers == 0)
                return result;

            var programCounts = new Dictionary<string, int>();

            foreach (var user in users)
            {
                foreach (var kvp in user.ProgramParticipation)
                {
                    var program = kvp.Key;
                    var value = kvp.Value;

                    if (value == 1)
                    {
                        if (!programCounts.ContainsKey(program))
                            programCounts[program] = 0;

                        programCounts[program]++;
                    }
                }
            }

            foreach (var kvp in programCounts.OrderByDescending(p => p.Value))
            {
                double rate = (double)kvp.Value / totalUsers * 100.0;

                result.Add(new ProgramStatEntry
                {
                    ProgramName = kvp.Key,
                    ParticipationCount = kvp.Value,
                    ParticipationRate = Math.Round(rate, 1)
                });
            }

            return result.OrderByDescending(p => p.ParticipationRate).ToList(); // 참여율 기준으로 오름차순 정렬
        }
        public static List<NonParticipantEntry> CalculateNonParticipants(List<ParticipationSummary> users)
        {
            var result = new List<NonParticipantEntry>();

            if (users.Count == 0)
                return result;

            int totalUsers = users.Count;

            var allPrograms = users
                .SelectMany(u => u.ProgramParticipation.Keys)
                .Distinct();

            foreach (var program in allPrograms)
            {
                int nonCount = users.Count(user =>
                    user.ProgramParticipation.ContainsKey(program) &&
                    user.ProgramParticipation[program] == 0); // ✅ null 제외, 0만 미참여

                double rate = (double)nonCount / totalUsers * 100.0;

                result.Add(new NonParticipantEntry
                {
                    ProgramName = program,
                    NonParticipationRate = Math.Round(rate, 1)
                });
            }

            return result.OrderByDescending(p => p.NonParticipationRate).ToList(); // 미참여율 기준 오름차순 정렬
        }

        public static void ExportToExcelWithCharts(List<ProgramStatEntry> programStatistics, List<RecommendationResult> recommendationResults, List<NonParticipantEntry> nonParticipants)
        {
            using (var workbook = new XLWorkbook())
            {
                // "Program Statistics" 시트 추가
                var worksheet = workbook.Worksheets.Add("Program Statistics");

                // 헤더 작성 (프로그램 참여율 통계)
                worksheet.Cell("A1").Value = "프로그램명";
                worksheet.Cell("B1").Value = "참여자 수";
                worksheet.Cell("C1").Value = "참여율 (%)";

                // 데이터 삽입 및 스타일 설정
                for (int i = 0; i < programStatistics.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = programStatistics[i].ProgramName;
                    worksheet.Cell(i + 2, 2).Value = programStatistics[i].ParticipationCount;
                    worksheet.Cell(i + 2, 2).Style.NumberFormat.Format = "0.0"; // 숫자 포맷 (천 단위 구분기호)
                    worksheet.Cell(i + 2, 3).Value = programStatistics[i].ParticipationRate;
                    worksheet.Cell(i + 2, 3).Style.NumberFormat.Format = "0.0"; // 백분율 형식

                    // 텍스트 래핑을 설정하여 텍스트가 셀 안에서 줄 바꿈 되도록
                    worksheet.Cell(i + 2, 1).Style.Alignment.WrapText = true;
                    worksheet.Cell(i + 2, 2).Style.Alignment.WrapText = true;
                    worksheet.Cell(i + 2, 3).Style.Alignment.WrapText = true;
                }

                // 열 너비 수동 설정
                worksheet.Column(1).Width = 30; // 프로그램명
                worksheet.Column(2).Width = 15; // 참여자 수
                worksheet.Column(3).Width = 15; // 참여율

                // 행 높이 자동 조정
                worksheet.Rows().AdjustToContents();

                // 빈 행 추가
                worksheet.Row(programStatistics.Count + 2).Height = 30;

                // AI 추천 프로그램 데이터 삽입
                var recommendationSheet = workbook.Worksheets.Add("AI 추천 프로그램");
                recommendationSheet.Cell("A1").Value = "프로그램명";
                recommendationSheet.Cell("B1").Value = "AI 관심도";

                for (int i = 0; i < recommendationResults.Count; i++)
                {
                    recommendationSheet.Cell(i + 2, 1).Value = recommendationResults[i].ProgramName;
                    recommendationSheet.Cell(i + 2, 2).Value = recommendationResults[i].FormattedScore;
                }
                // 열 너비 자동 조정 (수동으로 너비 설정)
                recommendationSheet.Column(1).Width = 30; // 프로그램명
                recommendationSheet.Column(2).Width = 15; // 미참여율 (%)

                // 빈 행 추가
                recommendationSheet.Row(recommendationResults.Count + 2).Height = 30;

                // 미참여율 통계 데이터 삽입
                var nonParticipantsSheet = workbook.Worksheets.Add("미참여자 통계");
                nonParticipantsSheet.Cell("A1").Value = "프로그램명";
                nonParticipantsSheet.Cell("B1").Value = "미참여율 (%)";

                // 미참여율 값을 문자식으로 삽입
                for (int i = 0; i < nonParticipants.Count; i++)
                {
                    nonParticipantsSheet.Cell(i + 2, 1).Value = nonParticipants[i].ProgramName;

                    // 숫자값을 그대로 삽입 (예: 0.75 -> "75%")
                    nonParticipantsSheet.Cell(i + 2, 2).Value = nonParticipants[i].NonParticipationRate;
                    nonParticipantsSheet.Cell(i + 2, 2).Style.NumberFormat.Format = "0.0"; // 그냥 값으로 출력 (백분율 형식 X)
                }

                // 열 너비 자동 조정 (수동으로 너비 설정)
                nonParticipantsSheet.Column(1).Width = 30; // 프로그램명
                nonParticipantsSheet.Column(2).Width = 15; // 미참여율 (%)

                // 엑셀 파일로 저장
                // SaveFileDialog를 통해 파일 저장 경로 지정
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = "ProgramReport.xlsx" // 기본 파일 이름 지정
                };

                // 사용자가 경로를 선택하고 저장을 클릭하면
                if (saveFileDialog.ShowDialog() == true)
                {
                    // 사용자가 선택한 경로에 파일 저장
                    workbook.SaveAs(saveFileDialog.FileName);
                }
            }
        }


        // 차트 추가 (Excel Interop 사용)msoffice것만 가능
        private static void AddChartToExcelWithInterop(IXLWorksheet worksheet)
        {
            // Excel Application 시작
            var excelApp = new Excel.Application();
            excelApp.Visible = false; // Excel 창이 보이지 않도록 설정

            // 워크북과 시트 생성
            var workbooks = excelApp.Workbooks;
            var workbook = workbooks.Add();
            var sheet = workbook.Sheets[1] as Excel.Worksheet;

            // 데이터를 삽입한 후 차트를 생성
            Excel.Range chartRange = sheet.Range["A1:B10"];  // 데이터 범위 지정 (예시)

            // 차트 생성
            Excel.ChartObjects charts = sheet.ChartObjects() as Excel.ChartObjects;
            Excel.ChartObject chartObject = charts.Add(100, 100, 500, 300); // 위치와 크기 설정
            Excel.Chart chart = chartObject.Chart;
            chart.SetSourceData(chartRange);
            chart.ChartType = Excel.XlChartType.xlColumnClustered;  // 차트 유형

            // 차트 제목 설정
            chart.HasTitle = true;
            chart.ChartTitle.Text = "프로그램별 참여율";

            // Excel 파일로 저장
            workbook.SaveAs("ProgramReportWithChart.xlsx");
            workbook.Close();
            excelApp.Quit();
        }
    }



}
