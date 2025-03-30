using GyeotaeAdmin.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{
    public class CsvExportService
    {
        public void ExportToCsv(IEnumerable<UserModel> users, ProgramModel _selectedProgram)
        {
            // SaveFileDialog를 사용하여 파일 저장 경로와 이름을 선택하게 함
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",  // 파일 형식 필터
                DefaultExt = "csv",                  // 기본 확장자
                FileName = "SentUsers.csv"           // 기본 파일 이름
            };

            // 사용자가 저장할 파일 경로를 선택했을 경우
            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;  // 사용자가 선택한 경로

                var csvContent = new StringBuilder();
                csvContent.AppendLine("Name,Phone,IsNotified,NotificationDate");

                foreach (var user in users)
                {
                    csvContent.AppendLine($"{user.Name},{user.Phone},{user.IsNotified},{user.NotificationDate},{_selectedProgram.Title}");
                }

                // 선택된 경로에 파일 저장
                File.WriteAllText(filePath, csvContent.ToString());

                Console.WriteLine("전송된 사용자 데이터가 CSV 파일로 저장되었습니다.");
            }
        }
    }
}
