using Serilog;
using System.Globalization;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace AdLumeClient.Classes.mpv;

public class MpvClient : IMpvClient
{
    private Stream? _stream;

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

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em SendCommandAsync: Comando: " + strLog + " Erro: " + ex.Message);
        }


    }

    public Task SetVolume(int volume)
        => SendCommandAsync(new object[] { "set_property", "volume", volume });

    public Task LoadFileAppend(string file)
        => SendCommandAsync(new object[] { "loadfile", file, "append-play" });

    public Task PlaylistClear()
        => SendCommandAsync(new object[] { "playlist-clear" });

    public Task SetPause(bool pause)
        => SendCommandAsync(new object[] { "set_property", "pause", pause });

    public Task SetFullscreen(bool fullscreen)
        => SendCommandAsync(new object[] { "set_property", "fullscreen", fullscreen });

    public Task LoopPlaylist()
        => SendCommandAsync(new object[] { "set_property", "loop-playlist", "inf" });
}
