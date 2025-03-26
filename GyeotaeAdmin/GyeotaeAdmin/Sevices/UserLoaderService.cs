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
            var list = new List<UserModel>();

            using (var reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    var fields = line.Split(',');

                    if (fields.Length < 8) continue;

                    try
                    {
                        DateTime.TryParse(fields[1], out DateTime birth);
                        int.TryParse(fields[3], out int income);

                        list.Add(new UserModel
                        {
                            Name = fields[0],
                            BirthDate = birth,
                            Address = fields[2],
                            Income = income,
                            HouseholdType = fields[4],
                            Phone = fields[5],
                            Gender = fields[6],
                            Email = fields[7]
                        });
                    }
                    catch { continue; }
                }
            }

            return list;
        }

        private List<UserModel> LoadFromXlsx(string filePath)
        {
            var list = new List<UserModel>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                bool isFirst = true;

                while (reader.Read())
                {
                    if (isFirst) { isFirst = false; continue; }
                    if (reader.FieldCount < 8) continue;

                    try
                    {
                        DateTime.TryParse(reader.GetValue(1)?.ToString(), out DateTime birth);
                        int.TryParse(reader.GetValue(3)?.ToString(), out int income);

                        list.Add(new UserModel
                        {
                            Name = reader.GetValue(0)?.ToString(),
                            BirthDate = birth,
                            Address = reader.GetValue(2)?.ToString(),
                            Income = income,
                            HouseholdType = reader.GetValue(4)?.ToString(),
                            Phone = reader.GetValue(5)?.ToString(),
                            Gender = reader.GetValue(6)?.ToString(),
                            Email = reader.GetValue(7)?.ToString()
                        });
                    }
                    catch { continue; }
                }
            }

            return list;
        }
    }
}


