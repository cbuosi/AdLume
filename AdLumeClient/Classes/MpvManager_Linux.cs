using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes;
public class MpvManager_Linux : IMpvManager
{

    private const string SocketPath = "/tmp/mpv-socket";

    public async Task<bool> RestartAsync(string mpvPath)
    {
        Kill();
        await Task.Delay(300);
        Start(mpvPath);
        return true;
    }

    public Task<bool> IsInstalledAsync(string mpvPath)
    {
        var path = Path.Combine(mpvPath, "mpv");
        return Task.FromResult(File.Exists(path));
    }

    public string GetIpcEndpoint() => SocketPath;

    private void Start(string mpvPath)
    {
        var exe = Path.Combine(mpvPath, "mpv");

        Process.Start(new ProcessStartInfo
        {
            FileName = exe,
            Arguments = $"--input-ipc-server={SocketPath} --idle=yes",
            UseShellExecute = false
        });
    }

    private void Kill()
    {
        foreach (var p in Process.GetProcessesByName("mpv"))
        {
            try { p.Kill(); } catch { }
        }
    }

}