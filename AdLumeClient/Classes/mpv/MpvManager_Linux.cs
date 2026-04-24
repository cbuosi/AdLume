using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes.mpv;

public class MpvManager_Linux : IMpvManager
{

    private const string SocketPath = "/tmp/mpv-socket";

    public async Task<bool> RestartAsync(string mpvPath)
    {
        try
        {
            Log.Information("RestartAsync (linux): " + mpvPath);
            Kill();
            await Task.Delay(300);
            Start(mpvPath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Erro RestartAsync (linux): " + mpvPath + " : " + ex.Message);
            return false;
        }

    }

    public Task<bool> IsInstalledAsync(string mpvPath)
    {
        try
        {
            Log.Information("IsInstalledAsync (linux): " + mpvPath);
            return Task.FromResult(File.Exists(mpvPath));
        }
        catch (Exception ex)
        {
            Log.Error("Erro RestartAsync (linux): " + mpvPath + " : " + ex.Message);
            return Task.FromResult(false);
        }
    }

    public string GetIpcEndpoint() => SocketPath;

    private bool Start(string mpvPath)
    {
        try
        {
            Log.Information("Start (linux): " + mpvPath + " SocketPath: " + SocketPath);
            Process.Start(new ProcessStartInfo
            {
                FileName = mpvPath,
                Arguments = $"--input-ipc-server={SocketPath} --idle=yes",
                UseShellExecute = false
            });

            return true;

        }
        catch (Exception ex)
        {
            Log.Error("Erro Start (linux): " + mpvPath + " : " + ex.Message);
            return false;
        }
    }

    private bool Kill()
    {
        try
        {
            Log.Information("Kill (linux)");
            foreach (var p in Process.GetProcessesByName("mpv"))
            {
                try { p.Kill(); } catch { }
            }
            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Erro Kill (linux): " + ex.Message);
            return false;
        }
    }

}