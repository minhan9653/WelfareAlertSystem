using GyeotaeAdmin.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GyeotaeAdmin.Sevices
{

    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            var dbPath = "admin.db";

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                // 1. 테이블 생성
                var createTableCmd = new SQLiteCommand(@"
                CREATE TABLE IF NOT EXISTS AdminUsers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL
                );", connection);
                createTableCmd.ExecuteNonQuery();

                // 2. 기본 계정 있는지 확인
                var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM AdminUsers WHERE Username = 'admin'", connection);
                long count = (long)checkCmd.ExecuteScalar();
                if (count == 0)
                {
                    // 3. 기본 계정 생성
                    string defaultPassword = "1234";
                    string hashed = PasswordHelper.HashPassword(defaultPassword); // 여기는 기존에 쓰던 해시 함수

                    var insertCmd = new SQLiteCommand("INSERT INTO AdminUsers (Username, PasswordHash) VALUES (@u, @p)", connection);
                    insertCmd.Parameters.AddWithValue("@u", "admin");
                    insertCmd.Parameters.AddWithValue("@p", hashed);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }

}