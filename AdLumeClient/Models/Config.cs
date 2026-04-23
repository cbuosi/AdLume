using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Models;

public class Config
{
    public int Version { get; set; }
    public List<MediaItem> Media { get; set; } = new();
    public List<Playlist> Playlists { get; set; } = new();
    public List<Schedule> Schedule { get; set; } = new();
}