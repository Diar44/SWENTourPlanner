namespace SWENTourPlanner.Models
{
    public class TourLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public string Difficulty { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int Rating { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
    }
}
