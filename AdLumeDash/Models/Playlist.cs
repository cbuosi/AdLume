namespace AdLumeDash.Models
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<PlaylistItem> Items { get; set; }
    }

}
