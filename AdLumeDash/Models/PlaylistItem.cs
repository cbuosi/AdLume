using Dapper.Contrib.Extensions;

namespace AdLumeDash.Models;

[Table("tPlaylistItem")]
public class PlaylistItem
{
    [Key] int cPlaylistItem { get; set; }
    int cPlaylist { get; set; }
    public int Ordem { get; set; }
    public int cMidia { get; set; } // PK auto increment
    public int cAtivo { get; set; }
}
