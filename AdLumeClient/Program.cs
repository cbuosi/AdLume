using AdLumeClient;
using AdLumeClient.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VideoPlayback;
using static System.Net.WebRequestMethods;

class Program
{

    static string strArqPlaylist = "Playlist.json";
    static List<EquipamentoPlaylistDto>? oEquipamentoPlaylistDto;
    static string hashAtual = "x";
    static string deviceId = "";
    static string serverUrl = "";
    static int SyncIntervalSeconds;

    //static int _currentVersion = -1;
    static HttpClient _http = new HttpClient();

    static async Task Main()
    {

        bool bRet;
        bool bInternet;
        bool bForcaAtualiza = true;
        Process[] processes;

        MpvClient oMpv;
        ConfigurationRoot oConfig;

        try
        {

            IniciaSerilog();
            Log.Information("Sistema iniciado");

            oConfig = (ConfigurationRoot)new ConfigurationBuilder()
                      .SetBasePath(AppContext.BaseDirectory)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .Build();

            deviceId = oConfig["App:DeviceId"]!;
            Log.Information($"Dispositivo.....: {deviceId}");

            serverUrl = oConfig["App:ServerUrl"]!;
            Log.Information($"Servidor Config.: {serverUrl}");

            SyncIntervalSeconds = int.Parse(oConfig["App:SyncIntervalSeconds"]!);
            Log.Information($"Intervalo Atu...: {SyncIntervalSeconds}");

            bInternet = await InternetCheck.TemInternetAsync();
            Log.Information($"Internet........: {(bInternet ? "SIM" : "NÃO")}");

            //Primeira vez, sempre reinicia MPV
            oMpv = await ReiniciaMPV();

            if (oMpv == null)
            {
                Log.Information($"Erro MPV (1)");
                return;
            }

            while (true)
            {

                //
                //se MPV foi fechado, reinicia...
                processes = Process.GetProcessesByName("mpv");
                if (processes.Count() == 0)
                {
                    oMpv = await ReiniciaMPV();
                    bForcaAtualiza = true;
                    if (oMpv == null)
                    {
                        Log.Information($"Erro MPV (1)");
                        return;
                    }
                }
                //

                oEquipamentoPlaylistDto = await ObterPlaylistEquip(deviceId);
                //config = await GetConfig();
                //
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

                    bRet = await UpdatePlayer(oMpv!, oEquipamentoPlaylistDto);
                    if (!bRet)
                    {
                        Log.Information($"Erro MPV (1)");
                        return;
                    }

                    await oMpv!.SetVolume(50);
                    await oMpv!.LoopPlaylist();
                    await oMpv!.SetPause(false);
                    await oMpv!.SetFullscreen(true);
                    hashAtual = GetObjectHash(oEquipamentoPlaylistDto!);

                }
                else
                {
                    Log.Information("Sem mudanças configuração");
                }

                Log.Information($"Aguardando {SyncIntervalSeconds}");
                await Task.Delay(TimeSpan.FromSeconds(SyncIntervalSeconds));

            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao executar MPV");
        }
        finally
        {
            Log.Information("Fim execução!");
        }

    }

    private static async Task<MpvClient> ReiniciaMPV()
    {
        string exePath;
        string mpvPath;
        bool bRet;
        MpvClient _oMpv;

        try
        {

            exePath = AppContext.BaseDirectory;
            Log.Information($"Rodando em: {exePath}");

            mpvPath = Path.Combine(exePath, @"mpv");
            bRet = await MpvManager.VerificaInstalaMpv(mpvPath);
            if (!bRet)
            {
                Log.Information($"Erro ao encontrar MVP.");
                return null;
            }

            mpvPath = Path.Combine(exePath, @"mpv\mpv.exe");
            bRet = await MpvManager.RestartMpvAsync(mpvPath);
            if (!bRet)
            {
                Log.Information($"Erro ao matar ou iniciar MPV.");
                return null;
            }

            _oMpv = new MpvClient();

            await _oMpv.ConnectAsync(); // Linux: /tmp/mpv-socket

            Log.Information($"Acoplando eventos MPV");
            _oMpv.OnEvent += HandleMpvEvent;


            return _oMpv;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em xxxxxxxxxxxxxxxxx");
            return null;
        }


    }

    private static void IniciaSerilog()
    {

        try
        {

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "logs"));

            Log.Logger = new LoggerConfiguration()
                             .MinimumLevel.Debug()
                             .WriteTo.Console()
                             .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "adlume-.log"), rollingInterval: RollingInterval.Day)
                             .CreateLogger();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    static void HandleMpvEvent(object e)
    {
        //Log.Information("Sistema iniciado");
        Log.Information("MPV EVENT: {Event}", e);
    }

    // -----------------------------
    static internal async Task<List<EquipamentoPlaylistDto>?> ObterPlaylistEquip(string deviceId)
    {

        string arq;
        string url;
        string json = "[]";
        List<EquipamentoPlaylistDto> ret;
        List<EquipamentoPlaylistDto> ret2;
        int HorarioAtual;
        bool bInternet;

        try
        {

            arq = Path.Combine(AppContext.BaseDirectory, strArqPlaylist);

            bInternet = await InternetCheck.TemInternetAsync();
            if (bInternet)
            {
                // ------------------------------------------------------------------------------
                Log.Information("Obtendo parametros Equipamento (Horarios/Videos)");
                //"http://localhost:5080/device/SEU_GUID/config";
                url = $"{serverUrl}/equipamento/{deviceId}";
                json = await _http.GetStringAsync(url);
                if (System.IO.File.Exists(arq))
                {
                    System.IO.File.Delete(arq);
                }
                System.IO.File.WriteAllText(arq, json);
                // ------------------------------------------------------------------------------
            }
            else
            {
                //verifica cache...
                if (System.IO.File.Exists(arq))
                {
                    json = System.IO.File.ReadAllText(arq);
                }
            }

            ret = JsonSerializer.Deserialize<List<EquipamentoPlaylistDto>>
                (json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;


            HorarioAtual = clsUtil.HoraParaInt(DateTime.Now.ToString("HH:mm"));

            ret2 = new List<EquipamentoPlaylistDto>();

            //- 14:00 < 15:23 < 18:00
            foreach (EquipamentoPlaylistDto item in ret)
            {
                if ((item.MinIni() < HorarioAtual) && (HorarioAtual < item.MinFin()))
                {
                    ret2.Add(item);                    
                }
            }

            return ret2;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em ObterPlaylistEquip");
            //throw new Exception("Erro em ObterPlaylistEquip");
            return null;
        }

    }


    static async Task<Config> GetConfig()
    {

        string url;
        string json;
        Config ret;

        try
        {

            // "http://localhost:5080/device/SEU_GUID/config";
            url = "http://localhost:5080/device/11111111-1111-1111-1111-111111111111/config";

            json = await _http.GetStringAsync(url);

            if (json == null)
            {
                return new Config();
            }

            ret = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return ret;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em GetConfig");
            throw new Exception("Erro em GetConfig");
        }

    }

    //static async Task<bool> SyncMedia(Config config)
    static async Task<bool> SyncMedia(IEnumerable<EquipamentoPlaylistDto>? oEquipamentoPlaylistDto)
    {

        string urlMidia;
        string basePath;
        string mediaPath;
        string filePath;
        byte[] bytes;

        try
        {

            Log.Information($"SyncMedia - Verificando (e baixando) {oEquipamentoPlaylistDto!.Count()} arquivos...");

            basePath = AppContext.BaseDirectory;
            mediaPath = Path.Combine(basePath, "Videos");
            Log.Information("Atualizando em: " + mediaPath);

            Directory.CreateDirectory(mediaPath);

            foreach (EquipamentoPlaylistDto media in oEquipamentoPlaylistDto!)
            {

                filePath = Path.Combine(mediaPath, $"{media.HashMidia}.mp4");

                if (System.IO.File.Exists(filePath))
                {
                    Log.Information($"Arquivo: [{media.NomeMidia}] HASH: [{media.HashMidia}] ja existe, ignorando...");
                    continue;
                }

                Log.Information($"Baixando Arquivo: [{media.NomeMidia}] HASH: [{media.HashMidia}]");
                urlMidia = string.Format(media.UrlMidia!, "https", "localhost", "7246");
                bytes = await _http.GetByteArrayAsync(urlMidia);
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);

            }

            return true;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em SyncMedia");
            return false;
        }

    }

    static async Task<bool> UpdatePlayer(MpvClient mpv, IEnumerable<EquipamentoPlaylistDto>? oEquipamentoPlaylistDto)
    {

        //Playlist? playlist;
        string basePath;
        string mediaPath;
        string file;

        try
        {

            Log.Information($"UpdatePlayer: Carregando {oEquipamentoPlaylistDto!.Count()} arquivo(s)...");

            basePath = AppContext.BaseDirectory;
            mediaPath = Path.Combine(basePath, "Videos");

            await mpv.PlaylistClear();

            foreach (EquipamentoPlaylistDto item in oEquipamentoPlaylistDto!)
            {

                file = Path.Combine(mediaPath, $"{item.HashMidia}.mp4");

                if (!System.IO.File.Exists(file))
                {
                    Log.Warning("Arquivo não encontrado: {File}", file);
                    continue;
                }

                await mpv.LoadFileAppend(file);
            }

            return true;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em UpdatePlayer");
            return false;
        }

    }

    static string GetObjectHash(object obj)
    {
        var json = JsonSerializer.Serialize(obj);

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(json);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToHexString(hash).ToLower();
    }


}