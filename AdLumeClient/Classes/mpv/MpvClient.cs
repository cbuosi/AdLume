using Serilog;
using System.Globalization;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AdLumeClient.Classes.mpv;

public class MpvClient : IMpvClient
{

    private const int T_REFRESH = 100;

    private Stream? _stream;

    event Action<string>? OnEvent;
    //event Action<string>

    public MpvClient()
    {
        Log.Information("ctor MpvClient...");
        Task.Run(ReadLoop);
    }

    private async Task ReadLoop()
    {
        var buffer = new byte[8192];

        while (true)
        {

            if (_stream == null)
            {
                continue;
            }

            var bytesRead = await _stream!.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead <= 0)
            {
                continue;
            }

            var msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            //Log.Information("MPV RAW: {Msg}", msg);
            OnEvent?.Invoke(msg);

        }
    }

    public async Task ConnectAsync(string endpoint)
    {
        try
        {

            if (OperatingSystem.IsWindows())
            {
                Log.Information("----- ConnectAsync (Windows): " + endpoint + " -----");
                if (string.IsNullOrWhiteSpace(endpoint))
                    throw new ArgumentException("Endpoint (socket path) não pode ser vazio.");

                var client = new NamedPipeClientStream(".", "mpv-pipe",
                    PipeDirection.InOut, PipeOptions.Asynchronous);

                await client.ConnectAsync(2000);
                _stream = client;
            }
            else
            {

                Log.Information("----- ConnectAsync (Linux): " + endpoint + " -----");

                if (string.IsNullOrWhiteSpace(endpoint))
                    throw new ArgumentException("Endpoint (socket path) não pode ser vazio.");

                if (!File.Exists(endpoint))
                    throw new FileNotFoundException($"Socket não encontrado em: {endpoint}");

                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                var ep = new UnixDomainSocketEndPoint(endpoint);

                await socket.ConnectAsync(ep);
                // ownership = true → fecha o socket junto com o stream
                _stream = new NetworkStream(socket, ownsSocket: true);

                //Log.Information("----- ConnectAsync (Linux): " + endpoint + " -----");
                //var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                //var ep = new UnixDomainSocketEndPoint(endpoint);
                //
                //await socket.ConnectAsync(ep);
                //_stream = new NetworkStream(socket);
            }

            this.OnEvent += HandleMpvEvent;

            Log.Information("----- ConnectAsync Fim -----");

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em ConnectAsync: " + ex.Message);
        }

    }

    public async Task SendCommandAsync(object[] command)
    {

        string strLog = "";

        try
        {


            foreach (var item in command)
            {
                if (strLog.Length > 0)
                {
                    strLog += " / " + item.ToString();
                }
                else
                {
                    strLog += item.ToString();
                }

            }

            Log.Information("SendCommandAsync: " + strLog);

            var payload = JsonSerializer.Serialize(new { command }) + "\n";
            var bytes = Encoding.UTF8.GetBytes(payload);

            await _stream!.WriteAsync(bytes, 0, bytes.Length);

            //tempinho pra processar
            await Task.Delay(T_REFRESH);

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em SendCommandAsync: Comando: " + strLog + " Erro: " + ex.Message);
        }


    }

    public async  Task SetVolume(int volume)
    {
        await SendCommandAsync(new object[] { "set_property", "volume", volume });
    }

    public async Task LoadFilePrimeira(string file)
    {
        
        await SendCommandAsync(new object[] { "loadfile", file, "replace" });
    }

    public async Task LoadFileAppend(string file)
    {
        
        await SendCommandAsync(new object[] { "loadfile", file, "append" }); //append-play
    }

    public async Task PlaylistClear()
    {
        
        await SendCommandAsync(new object[] { "playlist-clear" });
    }

    public async Task SetPause(bool pause)
    {
        
        await SendCommandAsync(new object[] { "set_property", "pause", pause });
    }

    public async Task SetFullscreen(bool fullscreen)
    {
        
        await SendCommandAsync(new object[] { "set_property", "fullscreen", fullscreen });
    }

    public async Task LoopPlaylist()
    {
        
        await SendCommandAsync(new object[] { "set_property", "loop-playlist", "inf" });
    }

    static void HandleMpvEvent(string e)
    {

        if (e == null)
        {
            return;
        }

        //Log.Information("Sistema iniciado");
        Log.Debug("MPV EVENT: {Event}", e.ToString().Replace("\n", " "));
    }


    public void Dispose()
    {
        Log.Information("MpvClient.Dispose()");
    }

}
