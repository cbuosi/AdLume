using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes.mpv;

public class MpvManager_Windows : IMpvManager
{
    private const string PipeName = @"\\.\pipe\mpv-pipe";

    public async Task<bool> RestartAsync(string mpvPath)
    {
        try
        {
            Log.Information("RestartAsync (Windows): " + mpvPath);
            Kill();
            await Task.Delay(300);
            await StartAsync(mpvPath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Erro RestartAsync (Windows): " + mpvPath + " : " + ex.Message);
            return false;
        }
    }

    public Task<bool> IsInstalledAsync(string mpvPath)
    {
        try
        {
            Log.Information("IsInstalledAsync (Windows): " + mpvPath);

            return Task.FromResult(File.Exists(mpvPath));
        }
        catch (Exception ex)
        {
            Log.Error("Erro IsInstalledAsync (Windows): " + mpvPath + " : " + ex.Message);
            return Task.FromResult(false);
        }
    }

    public string GetIpcEndpoint() => PipeName;

    //private bool Start(string mpvPath)
    private async Task<bool> StartAsync(string mpvPath)
    {
        try
        {
            Log.Information("Start (Windows): " + mpvPath);

            Process.Start(new ProcessStartInfo
            {
                FileName = mpvPath,
                Arguments = $"--input-ipc-server={PipeName} --idle=yes --force-window=yes",
                UseShellExecute = false
            });

            await Task.Delay(TimeSpan.FromSeconds(5));

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Erro Start (Windows): " + mpvPath + " : " + ex.Message);
            return false;
        }
    }

    private bool Kill()
    {
        try
        {
            Log.Information("Kill (Windows)");

            foreach (var p in Process.GetProcessesByName("mpv"))
            {
                try
                {
                    Log.Information($"Matando processo: {p.Id}");
                    p.Kill();
                }
                catch
                {
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Erro Kill (Windows): " + ex.Message);
            return false;
        }
    }
}