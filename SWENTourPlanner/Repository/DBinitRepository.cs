using Npgsql;
using SWENTourPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MonsterTradingCardGame.Repository
{
    public class DBinitRepository
    {
        private string host = "localhost";
        private string username = "postgres";
        private string password = "Halamadrid1";
        private string database = "TourPlaner";

        private string getConnectionString()
        {
            return "Host=" + host + ";Username=" + username + ";Password=" + password + ";Database=" + database;
        }

        public DBinitRepository()
        {
            //name, tour description, from, to, transport type, tour distance, estimated time, route information

            DropTable("tour");
            DropTable("tourlog");

            CreateTable("tour", "CREATE TABLE IF NOT EXISTS tour (id VARCHAR(255) PRIMARY KEY, name VARCHAR(255) NOT NULL, description VARCHAR(255), tour_from VARCHAR(255) NOT NULL, tour_to VARCHAR(255) NOT NULL, transport_type VARCHAR(255) NOT NULL, distance VARCHAR(255) NOT NULL, estimated_time VARCHAR(255) NOT NULL, information VARCHAR(255));\r\n");
            CreateTable("tourlog", "CREATE TABLE IF NOT EXISTS tourlog (id VARCHAR(255) PRIMARY KEY, cardtotrade VARCHAR(255) NOT NULL, card_type VARCHAR(255), MinimumDamage double PRECISION, username VARCHAR(255) REFERENCES users(username));");
        }

        private void DropTable(string tableName)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand($"DROP TABLE IF EXISTS {tableName};", connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error dropping table '{tableName}': {ex.Message}");
                    }
                }

                connection.Close();
            }
        }

        private void CreateTable(string tableName, string createTableSql)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(createTableSql, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating table '{tableName}': {ex.Message}");
                    }
                }

                connection.Close();
            }
        }
    }
}
