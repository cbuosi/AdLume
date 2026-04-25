using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes.mpv;

public interface IMpvClient : IDisposable
{
    Task ConnectAsync(string endpoint);
    Task SendCommandAsync(object[] command);
    Task SetVolume(int volume);
    Task LoadFilePrimeira(string file);
    Task LoadFileAppend(string file);
    Task PlaylistClear();
    Task SetPause(bool pause);
    Task SetFullscreen(bool fullscreen);
    Task LoopPlaylist();
}
