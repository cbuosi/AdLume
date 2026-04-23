using AdLumeClient.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VideoPlayback;
using static System.Net.WebRequestMethods;

class Program
{

    static IEnumerable<EquipamentoPlaylistDto>? oEquipamentoPlaylistDto;
    static string hashAtual = "x";

    //static int _currentVersion = -1;
    static HttpClient _http = new HttpClient();

    static async Task Main()
    {

        bool bRet;
        string exePath;
        string mvpPath;
        MpvClient mpv;
        //Config config;
        ConfigurationRoot oConfig;
        string deviceId;
        string serverUrl;
        int SyncIntervalSeconds;

        try
        {
            IniciaSerilog();
            Log.Information("Sistema iniciado");

            oConfig = (ConfigurationRoot)new ConfigurationBuilder()
                          .SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .Build();

            deviceId = oConfig["App:DeviceId"]!;
            serverUrl = oConfig["App:ServerUrl"]!;
            SyncIntervalSeconds = int.Parse(oConfig["App:SyncIntervalSeconds"]!);

            Log.Information($"Dispositivo.....: {deviceId}");
            Log.Information($"Servidor Config.: {serverUrl}");
            Log.Information($"Intervalo Atu...: {SyncIntervalSeconds}");

            exePath = AppContext.BaseDirectory;
            Log.Information($"Rodando em: {exePath}");


            mvpPath = Path.Combine(exePath, @"mpv\mpv.exe");
            if (!System.IO.File.Exists(mvpPath))
            {
                Log.Information($"MVP não encontrado em: {mvpPath}");
                return;
            }


            bRet = await MpvManager.RestartMpvAsync(mvpPath);
            if (!bRet)
            {
                Log.Information($"Erro ao matar ou iniciar MVP.");
                return;
            }

            mpv = new MpvClient();

            await mpv.ConnectAsync(); // Linux: /tmp/mpv-socket

            mpv.OnEvent += HandleMpvEvent;

            while (true)
            {
                //
                oEquipamentoPlaylistDto = await ObterPlaylistEquip(deviceId);
               

                //config = await GetConfig();
                ////
                if (hashAtual != GetObjectHash(oEquipamentoPlaylistDto!))
                {

                    Log.Information("Nova configuração detectada!");
                    await SyncMedia(oEquipamentoPlaylistDto);
                    await UpdatePlayer(mpv, oEquipamentoPlaylistDto);

                    await mpv.SetVolume(50);
                    await mpv.LoopPlaylist();
                    await mpv.SetPause(false);
                    await mpv.SetFullscreen(true);
                    hashAtual = GetObjectHash(oEquipamentoPlaylistDto!);

                }
                else
                {
                    Log.Information("Sem mudanças configuração");
                }

                Log.Information($"Aguardando {SyncIntervalSeconds}");
                await Task.Delay(TimeSpan.FromSeconds(SyncIntervalSeconds));

            }



            /////////////            //var mpv = new MpvClient();
            /////////////            //
            /////////////            //// Linux: "/tmp/mpv-socket"
            /////////////            //// Windows: ignora parametro e usa pipe interno
            /////////////            //await mpv.ConnectAsync("/tmp/mpv-socket");
            /////////////
            /////////////            await MpvManager.RestartMpvAsync(Path.Combine(exePath, @"mpv\mpv.exe"));
            /////////////            //                               @"C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv\mpv.exe"
            /////////////            //                                "C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv\mpv.exe"
            /////////////            //                                 C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv\mvp.exe
            /////////////            //                                 C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv
            /////////////
            /////////////            Console.WriteLine("Conectado ao MPV");
            /////////////
            /////////////            // Tocar vídeo
            /////////////            var mpv = new MpvClient();
            /////////////
            /////////////            //mpv.OnEvent += e => Console.WriteLine($"EVENTx: {e}");
            /////////////            mpv.OnEvent += HandleMpvEvent;
            /////////////
            /////////////            await mpv.ConnectAsync(); // Linux: /tmp/mpv-socket
            /////////////
            /////////////
            /////////////            await mpv.PlaylistClear();
            /////////////
            /////////////
            /////////////#if false
            /////////////        //await mpv.LoadFile(Path.Combine(exePath, @"Videos\video_manha_1.mp4.mp4"));        // inicia a playlist (replace)
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_1.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_2.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_3.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_a.mp4"));
            /////////////
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_1.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_2.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_3.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_b.mp4"));
            /////////////#endif
            /////////////
            /////////////#if true
            /////////////            await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_1.mp4"));
            /////////////            await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_2.mp4"));
            /////////////            await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_a.mp4"));
            /////////////            await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_1.mp4"));
            /////////////            await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_2.mp4"));
            /////////////            await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_b.mp4"));
            /////////////#endif
            /////////////
            /////////////#if false
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_1.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_2.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_a.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_1.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_2.mp4"));
            /////////////        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_b.mp4"));
            /////////////#endif
            /////////////
            /////////////
            /////////////            await mpv.SetVolume(50);
            /////////////
            /////////////            await mpv.LoopPlaylist();
            /////////////            await mpv.SetPause(false);
            /////////////            await mpv.SetFullscreen(true);
            /////////////            Console.ReadLine();

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao executar MPV");
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

    internal static async Task<IEnumerable<EquipamentoPlaylistDto>?> ObterPlaylistEquip(string deviceId)
    {
        string url;
        string json;
        IEnumerable<EquipamentoPlaylistDto> ret;

        try
        {

            // "http://localhost:5080/device/SEU_GUID/config";
            url = $"http://localhost:5080/equipamento/{deviceId}";

            json = await _http.GetStringAsync(url);

            if (json == null)
            {
                return null;
            }

            ret = JsonSerializer.Deserialize<IEnumerable<EquipamentoPlaylistDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return ret;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em ObterPlaylistEquip");
            throw new Exception("Erro em ObterPlaylistEquip");
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