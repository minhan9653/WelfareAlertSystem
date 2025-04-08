using ExcelDataReader;
using GyeotaeAdmin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GyeotaeAdmin.Sevices
{
    public interface IUserLoaderService
    {
        List<UserModel> LoadFromFile(string filePath);
    }

    public class UserLoaderService : IUserLoaderService
    {
        public List<UserModel> LoadFromFile(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            return ext == ".csv" ? LoadFromCsv(filePath) : LoadFromXlsx(filePath);
        }

        private List<UserModel> LoadFromCsv(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  // ← 요 줄 중요!

            var list = new List<UserModel>();
            var errors = new List<string>();

            using (var reader = new StreamReader(filePath, Encoding.GetEncoding("euc-kr")))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrEmpty(headerLine)) return list;

                var headers = headerLine.Split(',').Select(h => h.Trim()).ToList();
                var columnIndex = new Dictionary<string, int>();
                for (int i = 0; i < headers.Count; i++)
                {
                    columnIndex[headers[i]] = i;
                }

                string[] requiredColumns = { "이름", "생년월일", "주소", "소득", "가구형태", "전화번호", "성별", "이메일" };
                foreach (var col in requiredColumns)
                {
                    if (!columnIndex.ContainsKey(col))
                    {
                        MessageBox.Show($"필수 컬럼 '{col}'이 누락되었거나 인코딩 오류로 깨졌습니다.", "CSV 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return list;
                    }
                }

                int lineNumber = 1;
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var fields = line.Split(',');

                    try
                    {
                        string name = fields[columnIndex["이름"]];
                        string birthStr = fields[columnIndex["생년월일"]];
                        string household = fields[columnIndex["가구형태"]];

                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(birthStr))
                        {
                            errors.Add($"[{lineNumber}행] 이름 또는 생년월일이 비어있습니다.");
                            continue;
                        }

                        DateTime.TryParse(birthStr, out DateTime birth);
                        int.TryParse(fields[columnIndex["소득"]], out int income);

                        if (household != "1인가구") continue;

                        list.Add(new UserModel
                        {
                            Name = name,
                            BirthDate = birth,
                            Address = fields[columnIndex["주소"]],
                            Income = income,
                            HouseholdType = household,
                            Phone = fields[columnIndex["전화번호"]],
                            Gender = fields[columnIndex["성별"]],
                            Email = fields[columnIndex["이메일"]]
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"[{lineNumber}행] 처리 오류: {ex.Message}");
                        continue;
                    }
                }
            }

            if (errors.Any())
            {
                string errorMessage = string.Join("\n", errors);
                MessageBox.Show(errorMessage, "CSV 데이터 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return list;
        }


        private List<UserModel> LoadFromXlsx(string filePath)
        {
            var list = new List<UserModel>();
            var errors = new List<string>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // 💡 파일 공유 허용 (엑셀 열려있을 때 대비)
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var columnIndex = new Dictionary<string, int>();
                bool isFirst = true;

                while (reader.Read())
                {
                    int lineNumber = reader.Depth + 1;  // ✅ 현재 줄 번호 (1부터 시작)

                    if (isFirst)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string colName = reader.GetValue(i)?.ToString();
                            if (!string.IsNullOrWhiteSpace(colName))
                            {
                                columnIndex[colName.Trim()] = i;
                            }
                        }

                        // 필수 컬럼 확인
                        string[] required = { "이름", "생년월일", "주소", "소득", "가구형태", "전화번호", "성별", "이메일" };
                        foreach (var col in required)
                        {
                            if (!columnIndex.ContainsKey(col))
                            {
                                MessageBox.Show($"필수 컬럼 '{col}'이 누락되어 있습니다.", "엑셀 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                                return list;
                            }
                        }

                        isFirst = false;
                        continue;
                    }

                    try
                    {
                        string name = reader.GetValue(columnIndex["이름"])?.ToString();
                        string birthStr = reader.GetValue(columnIndex["생년월일"])?.ToString();
                        string household = reader.GetValue(columnIndex["가구형태"])?.ToString();

                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(birthStr))
                        {
                            errors.Add($"[{lineNumber}행] 이름 또는 생년월일이 비어 있습니다.");
                            continue;
                        }

                        DateTime.TryParse(birthStr, out DateTime birth);
                        int.TryParse(reader.GetValue(columnIndex["소득"])?.ToString(), out int income);
                        if (household != "1인가구") continue;

                        list.Add(new UserModel
                        {
                            Name = name,
                            BirthDate = birth,
                            Address = reader.GetValue(columnIndex["주소"])?.ToString(),
                            Income = income,
                            HouseholdType = household,
                            Phone = reader.GetValue(columnIndex["전화번호"])?.ToString(),
                            Gender = reader.GetValue(columnIndex["성별"])?.ToString(),
                            Email = reader.GetValue(columnIndex["이메일"])?.ToString()
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"[{lineNumber}행] 처리 오류: {ex.Message}");
                        continue;
                    }
                }
            }

            // ✅ 오류 메시지 한 번에 출력
            if (errors.Any())
            {
                string errorMessage = string.Join("\n", errors);
                MessageBox.Show(errorMessage, "엑셀 데이터 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return list;
        }

    }
}


