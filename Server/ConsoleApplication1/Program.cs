using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace ConsoleApplication
{
    class Program
    {
        SQLiteConnection SQLite_connection;

        static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program()
        {
            System.Console.WriteLine("I am here1");
            loadDatabase();
            System.Console.WriteLine("I am here2");

            string sql = "select * from user";
            SQLiteCommand command = new SQLiteCommand(sql, SQLite_connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("User: " + reader["name"] + "\tPw: " + reader["password"] + "\tGames: " + reader["games"] + "\tWins: " + reader["win"]);
            System.Console.ReadLine();

        }

        void loadDatabase()
        {
            SQLite_connection = new SQLiteConnection("Data Source=users.sqlite;Version=3;");
            SQLite_connection.Open();
        }
    }
}
