using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Models;

public class Schedule
{
    public string PlaylistId { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty; // "08:00"
    public string End { get; set; } = string.Empty;   // "18:00"
    public List<string> Days { get; set; } = new();
}