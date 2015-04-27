﻿// Imported by default
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SQLite
using System.Data.SQLite;

namespace ConsoleApplication
{
    class DatabaseConnection
    {
        SQLiteConnection SQLite_connection;

        public DatabaseConnection()
        {
            LoadDatabase();
        }

        public void LoadDatabase() {
            SQLite_connection = new SQLiteConnection("Data Source=users.sqlite;Version=3;");
            SQLite_connection.Open();
        }

        public Boolean UserIsInDatabase(string username)
        {
            string sql = "SELECT * FROM USER U WHERE U.name = '" + username + "'";
            SQLiteCommand command = new SQLiteCommand(sql, SQLite_connection);
            object test = command.ExecuteScalar();
            // If it's not empty, then the user is in the database
            return test != null;
        }

        public Boolean IsCorrectPassword(string username, string password) {
            string sql = "SELECT * FROM USER U WHERE U.name = '" + username + "'";
            SQLiteCommand command = new SQLiteCommand(sql, SQLite_connection);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            string un = (string)reader["name"];
            string pw = (string)reader["password"];
            return (un == username) && (pw == password);
        }

        public void AddNewUser(string username, string password)
        {
            string sql = "INSERT INTO user (name, password, games, win) VALUES ('" + username +"', '" + password +"', 0, 0);";
            SQLiteCommand command = new SQLiteCommand(sql, SQLite_connection);
            command.ExecuteNonQuery();
        }

        public void PrintCommandResults(string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, SQLite_connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("User: " + reader["name"] + "\tPw: " + reader["password"] + "\tGames: " + reader["games"] + "\tWins: " + reader["win"]);

        }
    }
}
