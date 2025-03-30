using GyeotaeAdmin.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Helpers
{
    public class SQLiteHelper
    {
        private string _connectionString = "Data Source=SentUsers.db;Version=3;";  // SQLite DB 파일 경로 설정

        // 테이블 생성
        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS SentUsers (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Phone TEXT,
                        IsNotified BOOLEAN,
                        NotificationDate TEXT
                    )";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // 데이터 삽입
        public void InsertSentUser(UserModel user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = @"
                    INSERT INTO SentUsers (Name, Phone, IsNotified, NotificationDate) 
                    VALUES (@Name, @Phone, @IsNotified, @NotificationDate)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Phone", user.Phone);
                    command.Parameters.AddWithValue("@IsNotified", user.IsNotified);
                    command.Parameters.AddWithValue("@NotificationDate", user.NotificationDate);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

