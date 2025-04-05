using ClosedXML.Excel;
using GyeotaeAdmin.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                FileName = "SentUsers.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Phone");
                dt.Columns.Add("IsNotified");
                dt.Columns.Add("NotificationDate");
                dt.Columns.Add("Programname");
                dt.Columns.Add("participation");

                foreach (var user in users)
                {
                    dt.Rows.Add(
                        user.Name,
                        user.Phone,
                        user.IsNotified,
                        user.NotificationDate,
                        _selectedProgram.Title
                    );
                }

                using (var workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dt, "SentUsers");
                    workbook.SaveAs(saveFileDialog.FileName);
                }

                Console.WriteLine("전송된 사용자 데이터가 Excel 파일로 저장되었습니다.");
            }
        }
    }
}