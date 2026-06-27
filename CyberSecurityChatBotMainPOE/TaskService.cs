using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CyberSecurityChatBotMainPOE.Models;

namespace CyberSecurityChatBotMainPOE.Services
{
    public class TaskService
    {
        private readonly string _connectionString;

        public TaskService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<UserTask> GetAllTasks()
        {
            List<UserTask> tasks = new List<UserTask>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT id, title, description, reminder_date, status, created_at FROM tasks ORDER BY id DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tasks.Add(new UserTask
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                        ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date")) ? (DateTime?)null : reader.GetDateTime("reminder_date"),
                        Status = reader.GetString("status"),
                        CreatedAt = reader.GetDateTime("created_at")
                    });
                }
            }

            return tasks;
        }

        public bool AddTask(UserTask task)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO tasks (title, description, reminder_date, status) 
                                VALUES (@title, @desc, @date, @status)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", task.Title);
                cmd.Parameters.AddWithValue("@desc", task.Description ?? "");
                cmd.Parameters.AddWithValue("@date", task.ReminderDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@status", task.Status ?? "Pending");

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool CompleteTask(int taskId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE tasks SET status = 'Completed' WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteTask(int taskId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM tasks WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
