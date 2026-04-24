using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes.mpv;

public interface IMpvManager
{
    Task<bool> RestartAsync(string mpvPath);
    Task<bool> IsInstalledAsync(string mpvPath);
    string GetIpcEndpoint();
}
