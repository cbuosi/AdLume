using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Models;

public class Playlist
{
    public string Id { get; set; } = string.Empty;
    public List<PlaylistItem> Items { get; set; } = new();
}