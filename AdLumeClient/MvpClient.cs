using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VideoPlayback;

public class MpvClient : IDisposable
{
    private Stream? _stream;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private int _requestId = 0;

    public event Action<string>? OnEvent;


    public MpvClient()
    {
            
    }

    // =========================
    // 🔌 CONEXÃO
    // =========================

    public async Task<bool> ConnectAsync(string address = "/tmp/mpv-socket", string pipeName = "mpv-pipe")
    {

        try
        {

            if (OperatingSystem.IsWindows())
            {
                var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                await pipe.ConnectAsync();
                _stream = pipe;
            }
            else
            {
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                var endpoint = new UnixDomainSocketEndPoint(address);
                await socket.ConnectAsync(endpoint);
                _stream = new NetworkStream(socket);
            }

            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

            _ = Task.Run(ReadLoop);

            return true;

        }
        catch (Exception)
        {

            return false;
        }


    }

    private async Task ReadLoop()
    {
        while (true)
        {
            var line = await _reader!.ReadLineAsync();
            if (line == null)
            {
                break;
            }
            OnEvent?.Invoke(line);
        }
    }

    private int NextId() => Interlocked.Increment(ref _requestId);

    private async Task SendAsync(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        await _writer!.WriteLineAsync(json);
    }

    public Task CommandAsync(string command, params object[] args)
    {
        var payload = new
        {
            command = BuildCommand(command, args),
            request_id = NextId()
        };

        return SendAsync(payload);

    }

    private object[] BuildCommand(string cmd, object[] args)
    {
        var result = new object[args.Length + 1];
        result[0] = cmd;
        Array.Copy(args, 0, result, 1, args.Length);
        return result;
    }

    // =========================
    // ▶️ PLAYBACK
    // =========================

    public Task LoadFile(string path)
        => CommandAsync("loadfile", path);

    public Task LoadFileAppend(string path)
        => CommandAsync("loadfile", path, "append-play");

    public Task Stop()
        => CommandAsync("stop");

    public Task Quit()
        => CommandAsync("quit");

    public Task TogglePause()
        => CommandAsync("cycle", "pause");

    public Task SetPause(bool value)
        => CommandAsync("set_property", "pause", value);

    // =========================
    // ⏩ SEEK / FRAME
    // =========================

    public Task SeekRelative(double seconds)
        => CommandAsync("seek", seconds, "relative");

    public Task SeekAbsolute(double seconds)
        => CommandAsync("seek", seconds, "absolute");

    public Task FrameStep()
        => CommandAsync("frame-step");

    public Task FrameBackStep()
        => CommandAsync("frame-back-step");

    // =========================
    // 🔁 LOOP / PLAYLIST
    // =========================

    public Task LoopFile()
        => CommandAsync("set_property", "loop-file", "inf");

    public Task LoopPlaylist()
        => CommandAsync("set_property", "loop-playlist", "inf");

    public Task PlaylistNext()
        => CommandAsync("playlist-next");

    public Task PlaylistPrev()
        => CommandAsync("playlist-prev");

    public Task PlaylistClear()
        => CommandAsync("playlist-clear");

    public Task PlaylistRemove(int index)
        => CommandAsync("playlist-remove", index);

    public Task PlaylistMove(int from, int to)
        => CommandAsync("playlist-move", from, to);

    // =========================
    // 🔊 ÁUDIO
    // =========================

    public Task SetVolume(double value)
        => CommandAsync("set_property", "volume", value);

    public Task AddVolume(double delta)
        => CommandAsync("add", "volume", delta);

    public Task ToggleMute()
        => CommandAsync("cycle", "mute");

    // =========================
    // 🎬 VÍDEO
    // =========================

    public Task SetFullscreen(bool value)
        => CommandAsync("set_property", "fullscreen", value);

    //public Task SetFullscreen(bool value)
    //    => CommandAsync("set_property", "fullscreen", value);

    //public Task ToggleFullscreen()
    //    => CommandAsync("cycle", "fullscreen");

    public Task Screenshot()
        => CommandAsync("screenshot");

    // =========================
    // 🧠 PROPRIEDADES
    // =========================

    public Task GetTimePosition()
        => CommandAsync("get_property", "time-pos");

    public Task GetDuration()
        => CommandAsync("get_property", "duration");

    // =========================
    // ⚡ VELOCIDADE
    // =========================

    public Task SetSpeed(double speed)
        => CommandAsync("set_property", "speed", speed);

    // =========================
    // 📝 LEGENDAS
    // =========================

    public Task CycleSubtitles()
        => CommandAsync("cycle", "sub");

    public Task SetSubtitleDelay(double delay)
        => CommandAsync("set_property", "sub-delay", delay);

    // =========================
    // 🎧 TRACKS
    // =========================

    public Task SetAudioTrack(int id)
        => CommandAsync("set_property", "aid", id);

    public Task SetSubtitleTrack(int id)
        => CommandAsync("set_property", "sid", id);

    // =========================
    // 🧹 DISPOSE
    // =========================

    public void Dispose()
    {
        _reader?.Dispose();
        _writer?.Dispose();
        _stream?.Dispose();
    }
}