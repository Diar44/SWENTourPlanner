using Npgsql;
using Dapper;
using SWENTourPlanner.Models;
using System;
using System.Data;
using log4net;
using System.Windows;

namespace SWENTourPlanner.Repository
{
    public class TourDatabaseHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourDatabaseHelper));

        private string host = "localhost";
        private string usernamedb = "postgres";
        private string password = "Halamadrid1";
        private string database = "postgres";

        private string getConnectionString()
        {
            log.Debug("Trying to Connect to db.");

            return "Host=" + host + ";Username=" + usernamedb + ";Password=" + password + ";Database=" + database;
        }

        public void SaveTour(Tour tour)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO tours (name, description, from_location, to_location, transport_type, estimated_distance, estimated_time, route_image_url, created_at, updated_at)
                        VALUES (@Name, @Description, @From, @To, @TransportType, @Distance, @EstimatedTime, @RouteInformation, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

                    connection.Execute(insertQuery, new
                    {
                        tour.Name,
                        tour.Description,
                        From = tour.From,
                        To = tour.To,
                        TransportType = tour.TransportType,
                        Distance = tour.Distance,
                        EstimatedTime = tour.EstimatedTime,
                        RouteInformation = tour.RouteInformation
                    });

                    log.Info($"Saved tour '{tour.Name}' successfully.");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error saving tour to database", ex);
                throw; // Optionally, rethrow the exception to propagate it further if needed
            }
        }

        public List<Tour> GetTours()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
                {
                    connection.Open();

                    string query = "SELECT tour_Id AS Id, name AS Name, description AS Description, from_location AS From, to_location AS To, transport_type AS TransportType, estimated_distance AS Distance, estimated_time AS EstimatedTime, route_image_url AS RouteInformation FROM tours";
                    var tours = connection.Query<Tour>(query).AsList();

                    log.Info($"Retrieved {tours.Count} tours from database.");
                    return tours;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error retrieving tours from database", ex);
                throw; // Optionally, rethrow the exception to propagate it further if needed
            }
        }

        public void SaveTourLog(TourLog tourLog)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO tour_logs (log_date, comment, difficulty, total_distance, total_time, rating, tour_id)
                        VALUES (@Date, @Comment, @Difficulty, @TotalDistance, @TotalTime, @Rating, @TourId)";

                    connection.Execute(insertQuery, new
                    {
                        tourLog.Date,
                        tourLog.Comment,
                        tourLog.Difficulty,
                        tourLog.TotalDistance,
                        tourLog.TotalTime,
                        tourLog.Rating,
                        tourLog.TourId
                    });

                    log.Info($"Saved tour log for tour ID {tourLog.TourId} successfully.");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error saving tour log to database", ex);
                throw; // Optionally, rethrow the exception to propagate it further if needed
            }
        }

        public List<TourLog> GetTourLogs(int tourId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(getConnectionString()))
                {
                    connection.Open();
                    string query = @"
                        SELECT log_id as id, log_date, comment, difficulty, total_distance, total_time, rating, tour_id
                        FROM tour_logs
                        WHERE tour_id = @TourId;";
                    return connection.Query<TourLog>(query, new { TourId = tourId }).AsList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error retrieving tour logs from database", ex);
                throw;
            }
        }

        public void UpdateTourLog(TourLog tourLog)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
            {
                connection.Open();

                string updateQuery = @"
                    UPDATE tour_logs 
                    SET log_date = @Date, 
                        comment = @Comment, 
                        difficulty = @Difficulty, 
                        total_distance = @TotalDistance, 
                        total_time = @TotalTime, 
                        rating = @Rating 
                    WHERE log_id = @Id";

                connection.Execute(updateQuery, new
                {
                    tourLog.Id,
                    tourLog.Date,
                    tourLog.Comment,
                    tourLog.Difficulty,
                    tourLog.TotalDistance,
                    tourLog.TotalTime,
                    tourLog.Rating
                });

                connection.Close();
            }
        }

        public void DeleteTourLog(int tourLogId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM tour_logs WHERE log_id = @TourLogId";

                connection.Execute(deleteQuery, new { TourLogId = tourLogId });

                log.Info($"Deleted tour log with ID {tourLogId}.");
            }
        }

        public void DeleteTour(int tourId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM tours WHERE tour_id = @Id";
                connection.Execute(deleteQuery, new { Id = tourId });
                log.Info($"Deleted tour with ID {tourId} successfully.");
            }
        }

        public void UpdateTour(Tour tour)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(getConnectionString()))
            {
                connection.Open();
                string updateQuery = @"
            UPDATE tours
            SET name = @Name,
                description = @Description,
                from_location = @From,
                to_location = @To,
                estimated_distance = @Distance,
                updated_at = CURRENT_TIMESTAMP
            WHERE tour_id = @Id";

                connection.Execute(updateQuery, new
                {
                    tour.Name,
                    tour.Description,
                    tour.From,
                    tour.To,
                    tour.Distance,
                    tour.Id
                });

                log.Info($"Updated tour with ID {tour.Id} successfully.");
            }
        }

    }
}
