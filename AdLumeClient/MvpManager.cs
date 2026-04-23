using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VideoPlayback;

public static class MpvManager
{
    private const string ProcessName = "mpv";
    private const string PipeName = @"\\.\pipe\mpv-pipe";

    public static async Task<bool> RestartMpvAsync(string mpvPath)
    {
        try
        {
            KillExistingInstances();
            await Task.Delay(500); // pequeno delay para liberar recursos
            StartNewInstance(mpvPath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Information(ex, $"Erro ao matar ou iniciar MVP.");
            return false;
        }

    }

    private static void KillExistingInstances()
    {

        Log.Information($"Matando instancias anteriores de mvp.");

        var processes = Process.GetProcessesByName(ProcessName);

        if (processes.Count() == 0)
        {
            Log.Information($"Sem processo mvp ativo.");
            return;
        }

        foreach (var proc in processes)
        {
            try
            {
                Log.Information($"Matando processo mpv PID={proc.Id}");
                proc.Kill(true);
                proc.WaitForExit(3000);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Erro ao matar processo {proc.Id}: {ex.Message}");
            }
        }

    }

    private static void StartNewInstance(string mpvPath)
    {
        const string args = @"--input-ipc-server=\\.\pipe\mpv-pipe --idle=yes --force-window=yes";

        Log.Information($"Iniciando mvp.");

        if (!File.Exists(mpvPath))
        {
            throw new FileNotFoundException($"mpv.exe não encontrado em: {mpvPath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = mpvPath,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        var process = Process.Start(startInfo);

        if (process == null)
        {
            throw new Exception("Falha ao iniciar mpv");
        }

        Log.Information($"mpv iniciado PID={process.Id}");

    }

    //private static void StartNewInstance(string mpvPath)
    //{
    //    var args = $"--input-ipc-server={PipeName} --idle=yes --force-window=yes";
    //
    //    var startInfo = new ProcessStartInfo
    //    {
    //        FileName = mpvPath,
    //        Arguments = args,
    //        UseShellExecute = false,
    //        CreateNoWindow = false
    //    };
    //
    //    var process = Process.Start(startInfo);
    //
    //    if (process == null)
    //        throw new Exception("Falha ao iniciar mpv");
    //
    //    Console.WriteLine($"mpv iniciado PID={process.Id}");
    //}
}