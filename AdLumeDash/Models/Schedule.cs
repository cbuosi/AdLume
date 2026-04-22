namespace AdLumeDash.Models
{

    public class Schedule
    {
        public Guid Id { get; set; }
        public Guid PlaylistId { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string DaysOfWeek { get; set; } // "Mon,Tue,Wed"
    }

}
