using Dapper.Contrib.Extensions;

namespace AdLumeDash.Models;

[Table("tPlaylist")]
public class Playlist
{

    [Key] public int cPlaylist { get; set; }
    public int cEquipamento { get; set; }
    public string? NomePlaylist { get; set; }
    public string? HoraInicio { get; set; }
    //public List<PlaylistItem> Items { get; set; }
    public int cAtivo { get; set; }
}
