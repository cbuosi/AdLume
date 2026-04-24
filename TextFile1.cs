using AdLumeClient;
using AdLumeClient.Classes;
using AdLumeClient.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using VideoPlayback;

static class Program
{
    static string strArqPlaylist = "Playlist.json";
    static List<EquipamentoPlaylistDto>? oEquipamentoPlaylistDto;
    static string hashAtual = "x";
    static string deviceId = "";
    static string serverUrl = "";
    static int SyncIntervalSeconds;

    static IMpvManager mpvManager = MpvManagerFactory.Create();
    static IMpvClient mpvClient = MpvClientFactory.Create();

    static HttpClient _http = new HttpClient();

    public static async Task Main(string[] args)
    {
        bool bRet;
        bool bInternet;
        bool bForcaAtualiza = true;
        Process[] processes;
        ConfigurationRoot oConfig;

        try
        {
            IniciaSerilog();
            Log.Information($"{Environment.OSVersion.VersionString} - {Environment.OSVersion.Platform} - {Environment.OSVersion.Version.ToString()}");

            if (OperatingSystem.IsWindows())
            {
                Log.Information("SO Detectado: Windows");
            }
            else if (OperatingSystem.IsLinux())
            {
                Log.Information("SO Detectado: Linux");
            }
            else
            {
                Log.Information("SO Detectado: ????");
            }

            Log.Information($"Caminho app.....: {AppContext.BaseDirectory}");

            oConfig = (ConfigurationRoot)new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            deviceId = oConfig["App:DeviceId"]!;
            serverUrl = oConfig["App:ServerUrl"]!;
            SyncIntervalSeconds = int.Parse(oConfig["App:SyncIntervalSeconds"]!);
            bInternet = await InternetCheck.TemInternetAsync();

            Log.Information($"Dispositivo.....: {deviceId}");
            Log.Information($"Servidor Config.: {serverUrl}");
            Log.Information($"Intervalo Atu...: {SyncIntervalSeconds}");
            Log.Information($"Internet........: {(bInternet ? "SIM" : "NÃO")}");

            var ok = await ReiniciaMPV();
            if (!ok)
            {
                Log.Information($"Erro MPV (1)");
                return;
            }

            while (true)
            {
                processes = Process.GetProcessesByName("mpv");
                if (processes.Length == 0)
                {
                    Log.Information($"");
                    await ReiniciaMPV();
                    bForcaAtualiza = true;
                }

                oEquipamentoPlaylistDto = await ObterPlaylistEquip(deviceId);

                if (hashAtual != GetObjectHash(oEquipamentoPlaylistDto!) || bForcaAtualiza)
                {
                    bForcaAtualiza = false;
                    Log.Information("Nova configuração detectada!");

                    bRet = await SyncMedia(oEquipamentoPlaylistDto);
                    if (!bRet)
                    {
                        Log.Information($"Erro ao sincronizar midias.");
                        return;
                    }

                    bRet = await UpdatePlayer(oEquipamentoPlaylistDto);
                    if (!bRet)
                    {
                        Log.Information($"Erro UpdatePlayer (1)");
                        return;
                    }

                    await mpvClient.SetVolume(50);
                    await mpvClient.LoopPlaylist();
                    await mpvClient.SetPause(false);
                    await mpvClient.SetFullscreen(true);

                    hashAtual = GetObjectHash(oEquipamentoPlaylistDto!);
                }
                else
                {
                    Log.Information("Sem mudanças configuração");
                }

                Log.Information($"Aguardando {SyncIntervalSeconds} segundos para verificar nova configuração.");
                await Task.Delay(TimeSpan.FromSeconds(SyncIntervalSeconds));

            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro geral");
        }
    }

    private static async Task<bool> ReiniciaMPV()
    {
        try
        {
            Log.Information($"");
            var basePath = AppContext.BaseDirectory;
            var mpvFolder = Path.Combine(basePath, "mpv");

            if (!await mpvManager.IsInstalledAsync(mpvFolder))
            {
                Log.Information($"");
                return false;
            }

            if (!await mpvManager.RestartAsync(mpvFolder))
            {
                Log.Information($"");
                return false;
            }

            await Task.Delay(300);
            await mpvClient.ConnectAsync(mpvManager.GetIpcEndpoint());

            return true;
        }
        catch
        {
            Log.Information($"");
            return false;
        }
    }

    public static async Task<bool> UpdatePlayer(IEnumerable<EquipamentoPlaylistDto>? lista)
    {
        var basePath = AppContext.BaseDirectory;
        var mediaPath = Path.Combine(basePath, "Videos");

        Log.Information($"");

        await mpvClient.PlaylistClear();

        foreach (var item in lista!)
        {
            var file = Path.Combine(mediaPath, $"{item.HashMidia}.mp4");
            if (!File.Exists(file)) continue;

            await mpvClient.LoadFileAppend(file);
        }

        return true;
    }

    static async Task<bool> SyncMedia(IEnumerable<EquipamentoPlaylistDto>? lista)
    {

        Log.Information($"");

        var basePath = AppContext.BaseDirectory;
        var mediaPath = Path.Combine(basePath, "Videos");

        Directory.CreateDirectory(mediaPath);

        foreach (var media in lista!)
        {
            var filePath = Path.Combine(mediaPath, $"{media.HashMidia}.mp4");

            if (File.Exists(filePath)) continue;

            var url = string.Format(media.UrlMidia!, "https", "localhost", "7246");
            var bytes = await _http.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(filePath, bytes);
        }

        return true;
    }

    static internal async Task<List<EquipamentoPlaylistDto>?> ObterPlaylistEquip(string deviceId)
    {

        Log.Information($"");

        var arq = Path.Combine(AppContext.BaseDirectory, strArqPlaylist);
        var json = "[]";

        if (await InternetCheck.TemInternetAsync())
        {
            var url = $"{serverUrl}/equipamento/{deviceId}";
            json = await _http.GetStringAsync(url);
            File.WriteAllText(arq, json);
        }
        else if (File.Exists(arq))
        {
            json = File.ReadAllText(arq);
        }

        return JsonSerializer.Deserialize<List<EquipamentoPlaylistDto>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    static string GetObjectHash(object obj)
    {

        Log.Information($"");

        var json = JsonSerializer.Serialize(obj);
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(json))).ToLower();
    }

    private static void IniciaSerilog()
    {
        Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "logs"));

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"))
            .CreateLogger();
    }
}
