using Npgsql;
using SWENTourPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

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

            CreateTable("tours", "CREATE TABLE IF NOT EXISTS tours (tour_id SERIAL PRIMARY KEY,name VARCHAR(255) NOT NULL,description TEXT,from_location VARCHAR(255),to_location VARCHAR(255),transport_type VARCHAR(100),estimated_distance FLOAT,estimated_time INTERVAL,route_image_url VARCHAR(255),created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);");
            CreateTable("tour_logs", " CREATE TABLE tour_logs (log_id SERIAL PRIMARY KEY, tour_id INT NOT NULL, log_date TIMESTAMP, comment TEXT, difficulty VARCHAR(50), total_distance FLOAT, total_time INTERVAL, rating INT, created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP, updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, FOREIGN KEY (tour_id) REFERENCES tours(tour_id) ON DELETE CASCADE)");
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
