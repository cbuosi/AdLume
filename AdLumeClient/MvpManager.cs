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

    public static async Task RestartMpvAsync(string mpvPath = "mpv.exe")
    {
        KillExistingInstances();

        await Task.Delay(500); // pequeno delay para liberar recursos

        StartNewInstance(mpvPath);
    }

    private static void KillExistingInstances()
    {
        var processes = Process.GetProcessesByName(ProcessName);

        foreach (var proc in processes)
        {
            try
            {
                Console.WriteLine($"Matando processo mpv PID={proc.Id}");
                proc.Kill(true);
                proc.WaitForExit(3000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao matar processo {proc.Id}: {ex.Message}");
            }
        }
    }

    private static void StartNewInstance(string mpvPath)
    {
        if (!File.Exists(mpvPath))
            throw new FileNotFoundException($"mpv.exe não encontrado em: {mpvPath}");

        var args = @"--input-ipc-server=\\.\pipe\mpv-pipe --idle=yes --force-window=yes";

        var startInfo = new ProcessStartInfo
        {
            FileName = mpvPath,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        var process = Process.Start(startInfo);

        if (process == null)
            throw new Exception("Falha ao iniciar mpv");

        Console.WriteLine($"mpv iniciado PID={process.Id}");
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