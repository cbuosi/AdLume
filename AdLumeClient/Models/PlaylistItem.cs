using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Models;

public class PlaylistItem
{
    public string MediaHash { get; set; } = string.Empty;
    public int Duration { get; set; } // 0 = usar duração do vídeo
}