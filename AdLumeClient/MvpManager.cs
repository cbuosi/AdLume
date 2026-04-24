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
            Log.Information("Reiniciando Mpv");
            KillExistingInstances();
            await Task.Delay(500); // pequeno delay para liberar recursos
            StartNewInstance(mpvPath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Information(ex, $"Erro ao matar ou iniciar MPV.");
            return false;
        }

    }

    internal static async Task<bool> VerificaInstalaMpv(string mpvPath)
    {

        Log.Information("Verifica Instalação Mpv");

        mpvPath = Path.Combine(mpvPath, @"mpv.exe");
        if (!System.IO.File.Exists(mpvPath))
        {
            Log.Information($"MPV não encontrado em: {mpvPath}");
            return false;
        }

        return true;

    }

    private static void KillExistingInstances()
    {

        try
        {

        Log.Information($"Matando instancias anteriores de MPV.");

        Process[] processes = Process.GetProcessesByName(ProcessName);

        if (processes.Count() == 0)
        {
            Log.Information($"Sem processo MPV ativo.");
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
        catch (Exception ex)
        {
            Log.Error(ex,$"Erro em KillExistingInstances");
            throw;
        }

    }

    private static void StartNewInstance(string mpvPath)
    {
        const string args = @"--input-ipc-server=\\.\pipe\mpv-pipe --idle=yes --force-window=yes";

        Log.Information($"Iniciando MPV.");

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
 
}