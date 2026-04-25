using AdLumeClient.Classes;
using AdLumeClient.Classes.mpv;
using AdLumeClient.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

static class Program
{
    static string strArqPlaylist = "Playlist.json";

    static List<EquipamentoPlaylistDto>? oEquipPlaylistDto;
    static List<EquipamentoPlaylistDto>? oEquipPlaylistHorarioAtual;

    static string hashAtual = "";
    static string deviceId = "";
    static string serverUrl = "";
    static string mpvAppPath = "";
    static int SyncIntervalSeconds;

    static IMpvManager mpvManager = MpvManagerFactory.Create();
    static IMpvClient mpvClient = MpvClientFactory.Create();

    static HttpClient? _http;

    public static async Task Main(string[] args)
    {
        // ------------------ ------------------ ------------------ ------------------ ------------------ ------------------ ------------------ 
        bool bRet;
        bool bInternet;
        bool bForcaAtualiza = true;
        Process[] processes;
        ConfigurationRoot oConfig;
        // ------------------ ------------------ ------------------ ------------------ ------------------ ------------------ ------------------ 
        try
        {
            IniciaSerilog();

            //Inicia cliente http com umas paradas pra ignorar certificado zoado XD
            _http = CreateHttpClient();

            Log.Information($"##################################################");
            Log.Information($"# AdLume Client v.2.0.1 - BBS Informática © 2026 #");
            Log.Information($"##################################################");
            Log.Information($"");


            SystemInfo.PrintLogoAdLume();

            SystemInfo.PrintSummary();

            //var health = SystemHealth.Get(
            //    mediaPath: Path.Combine(AppContext.BaseDirectory, "Videos"),
            //    mpvConnected: true, // ou controle real depois
            //    lastSync: DateTime.Now
            //);
            //
            //health.Print();


            Log.Information($"------------------------------");
            Log.Information($"\x1b[32m- RESUMO CONFIGURAÇÃO        -");
            Log.Information($"------------------------------");

            // 🔹 Sistema Operacional
            oConfig = (ConfigurationRoot)new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Information($"Sistema Operacional....: {Environment.OSVersion.VersionString} - {Environment.OSVersion.Platform} - {Environment.OSVersion.Version.ToString()}");

            if (OperatingSystem.IsWindows())
            {
                Log.Information("SO Detectado...........: Windows");
                mpvAppPath = oConfig["App:pathMPV_Win"]!;
            }
            else if (OperatingSystem.IsLinux())
            {
                Log.Information("SO Detectado...........: Linux");
                mpvAppPath = oConfig["App:pathMPV_Linux"]!;
            }
            else
            {
                Log.Information("SO Detectado...........: ????");
                mpvAppPath = "";
            }

            Log.Information($"Caminho app............: {AppContext.BaseDirectory}");


            deviceId = oConfig["App:DeviceId"]!;
            serverUrl = oConfig["App:ServerUrl"]!;
            SyncIntervalSeconds = int.Parse(oConfig["App:SyncIntervalSeconds"]!);
            bInternet = await InternetCheck.TemInternetAsync();


            //{Subject}
            Log.Information("Dispositivo............: {Subject}", deviceId);
            Log.Information($"Servidor Config........: {serverUrl}");
            Log.Information($"Intervalo Atualização..: {SyncIntervalSeconds}");
            Log.Information($"Internet...............: {(bInternet ? "SIM" : "NÃO")}");
            Log.Information($"Caminho MPV (Player)...: {mpvAppPath}");

            while (true)
            {

                oEquipPlaylistDto = await ObterPlaylistEquip(deviceId);

                FiltraHorarioAtual();

                if (hashAtual != GetObjectHash(oEquipPlaylistHorarioAtual!) || bForcaAtualiza)
                {
                    bForcaAtualiza = false;
                    Log.Information("Nova configuração detectada!");

                    var ok = await ReiniciaMPV();
                    if (!ok)
                    {
                        Log.Information($"Erro em reiniciar MPV (1)");
                        return;
                    }

                    processes = Process.GetProcessesByName("mpv");
                    if (processes.Length == 0)
                    {
                        Log.Information($"Processo do MPV não encontrado, reiniciando...");
                        await ReiniciaMPV();
                        bForcaAtualiza = true;
                    }

                    bRet = await SyncMedia(oEquipPlaylistHorarioAtual);
                    if (!bRet)
                    {
                        Log.Information($"Erro ao sincronizar midias.");
                        return;
                    }

                    bRet = await UpdatePlayer(oEquipPlaylistHorarioAtual);
                    if (!bRet)
                    {
                        Log.Information($"Erro UpdatePlayer (1)");
                        return;
                    }

                    await mpvClient.SetVolume(30);
                    await mpvClient.LoopPlaylist();
                    await mpvClient.SetPause(false);
                    await mpvClient.SetFullscreen(true);

                    hashAtual = GetObjectHash(oEquipPlaylistHorarioAtual!);
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

    private static bool FiltraHorarioAtual()
    {

        int iHorarioAtual;

        try
        {

            Log.Information($"----- FiltraHorarioAtual -----");

            oEquipPlaylistHorarioAtual = new List<EquipamentoPlaylistDto>();

            if (oEquipPlaylistDto == null)
            {
                return false;
            }

            if (oEquipPlaylistDto.Count() == 0)
            {
                return false;
            }

            iHorarioAtual = clsUtil.HoraParaInt(DateTime.Now.ToString("HH:mm"));

            Log.Information($"Horario Atual: {DateTime.Now.ToString("HH:mm")} ({iHorarioAtual})");

            foreach (EquipamentoPlaylistDto item in oEquipPlaylistDto)
            {

                //esta no range da lista....
                //15:00 < 16:21 < 19:00 - adiciona
                if ((item.MinIni() <= iHorarioAtual) && (iHorarioAtual <= item.MinFin()))
                {
                    Log.Information($"Mídia Selecionada: {item.DescMidia} {item.HoraInicio}({item.MinIni()}) <= {DateTime.Now.ToString("HH:mm")} ({iHorarioAtual}) <= {item.HoraFim}({item.MinFin()})");
                    oEquipPlaylistHorarioAtual.Add(item);
                }

            }

            Log.Information($"Filtrado: Total: {oEquipPlaylistDto.Count()} / Filtrado: {oEquipPlaylistHorarioAtual.Count()}");

            return true;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em FiltraHorarioAtual");
            return false;
        }

    }

    private static async Task<bool> ReiniciaMPV()
    {
        try
        {

            Log.Information($"----- ReiniciaMPV Inicio -----");
            //var basePath = AppContext.BaseDirectory;
            //var mpvFolder = Path.Combine(basePath, "mpv");

            if (!await mpvManager.IsInstalledAsync(mpvAppPath))
            {
                Log.Information($"IsInstalledAsync erro!");
                return false;
            }

            if (!await mpvManager.RestartAsync(mpvAppPath))
            {
                Log.Information($"RestartAsync erro!");
                return false;
            }

            await Task.Delay(300);
            await mpvClient.ConnectAsync(mpvManager.GetIpcEndpoint());

            Log.Information($"----- ReiniciaMPV Fim -----");

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Erro em ReiniciaMPV");
            return false;
        }
    }

    public static async Task<bool> UpdatePlayer(IEnumerable<EquipamentoPlaylistDto>? lista)
    {

        int tot = 0;

        try
        {

            Log.Information($"----- UpdatePlayer -----");

            var basePath = AppContext.BaseDirectory;
            var mediaPath = Path.Combine(basePath, "Videos");

            await mpvClient.PlaylistClear();

            foreach (var item in lista!)
            {
                var file = Path.Combine(mediaPath, $"{item.HashMidia}.mp4");
                if (!File.Exists(file))
                {
                    continue;
                }

                tot += 1;

                if (tot == 1)
                {
                    await mpvClient.LoadFilePrimeira(file);
                }
                else
                {
                    await mpvClient.LoadFileAppend(file);
                }

            }

            return true;

        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Erro em UpdatePlayer");
            return false;
        }

    }

    static async Task<bool> SyncMedia(IEnumerable<EquipamentoPlaylistDto>? lista)
    {

        try
        {

            Log.Information($"----- SyncMedia -----");

            var basePath = AppContext.BaseDirectory;
            var mediaPath = Path.Combine(basePath, "Videos");

            Directory.CreateDirectory(mediaPath);

            foreach (var media in lista!)
            {
                var filePath = Path.Combine(mediaPath, $"{media.HashMidia}.mp4");

                if (File.Exists(filePath))
                {
                    Log.Information($"Ja existe a midia: {media.HashMidia}.mp4");
                    continue;
                }

                //var url = string.Format(media.NomeMidia!, "https", "localhost", "7246");
                var url = $"{serverUrl}/media/{media.NomeMidia}";


                var bytes = await _http!.GetByteArrayAsync(url);
                if (bytes.Length > 0)
                {
                    Log.Information($"Salvando nova midia: {media.HashMidia}.mp4 - Tam: {bytes.Length.ToString("N0")} bytes");
                    await File.WriteAllBytesAsync(filePath, bytes);
                }


            }

            return true;

        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Erro em SyncMedia");
            return false;
        }

    }

    static internal async Task<List<EquipamentoPlaylistDto>?> ObterPlaylistEquip(string deviceId)
    {

        try
        {


            Log.Information($"----- ObterPlaylistEquip -----");

            var arq = Path.Combine(AppContext.BaseDirectory, strArqPlaylist);
            var json = "[]";

            if (await InternetCheck.TemInternetAsync())
            {
                var url = $"{serverUrl}/equipamento/{deviceId}";
                json = await _http!.GetStringAsync(url);

                if (File.Exists(arq))
                {
                    File.Delete(arq);
                }

                File.WriteAllText(arq, json);
            }
            else if (File.Exists(arq))
            {
                json = File.ReadAllText(arq);
            }

            return JsonSerializer.Deserialize<List<EquipamentoPlaylistDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Erro em ObterPlaylistEquip");
            return new List<EquipamentoPlaylistDto>();
        }

    }

    static string GetObjectHash(object obj)
    {
        try
        {

            Log.Information($"----- GetObjectHash -----");
            var json = JsonSerializer.Serialize(obj);
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(json))).ToLower();

        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Erro em GetObjectHash");
            return Guid.NewGuid().ToString();
        }

    }

    private static void IniciaSerilog()
    {

        try
        {

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "logs"));

            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .WriteTo.Console()
            //    .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"))
            //    .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    Path.Combine(AppContext.BaseDirectory, "logs", "adlume-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();


        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro em IniciaSerilog: " + ex.Message);
            throw;
        }

    }

    static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = ValidateCert };
        return new HttpClient(handler);
    }

    static bool ValidateCert(HttpRequestMessage? request, X509Certificate2? cert, X509Chain? chain, SslPolicyErrors errors)
    {
        //Log.Warning("SSL erro: {Errors} - {Url}", errors, request?.RequestUri);
        //           Internet...............:
        Log.Warning("URL....................: {Url}", request?.RequestUri);
        Log.Warning("Subject................: {Subject}", cert?.Subject);
        Log.Warning("Issuer.................: {Issuer}", cert?.Issuer);
        Log.Warning("Thumbprint.............: {Thumbprint}", cert?.GetCertHashString());
        Log.Warning("Errors.................: {Errors}", errors);

        if (chain != null)
        {
            foreach (var status in chain.ChainStatus)
            {
                Log.Warning("Chain Status......: {Status} - {Info}",
                    status.Status, status.StatusInformation);
            }
        }

        return true; // DEV

    }

}
