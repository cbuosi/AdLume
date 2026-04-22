namespace AdLumeDash.Models
{
    public class PlaylistItem
    {
        public Guid MediaId { get; set; }
        public int Order { get; set; }
        public int DurationSeconds { get; set; }
    }
}
