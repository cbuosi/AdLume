using System;
using System.Threading.Tasks;
using VideoPlayback;

class Program
{
    static async Task Main()
    {

        //var mpv = new MpvClient();
        //
        //// Linux: "/tmp/mpv-socket"
        //// Windows: ignora parametro e usa pipe interno
        //await mpv.ConnectAsync("/tmp/mpv-socket");

        await MpvManager.RestartMpvAsync(@"C:\temp\x1\mpv\mpv.exe");
        //                               @"C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv\mpv.exe"
        //                                "C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv\mpv.exe"
        //                                 C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv\mvp.exe
        //                                 C:\Users\CBuosi\Downloads\MPV-EASY Player V0.41.0.3\mpv

        Console.WriteLine("Conectado ao MPV");

        // Tocar vídeo
        var mpv = new MpvClient();
        mpv.OnEvent += e => Console.WriteLine($"EVENTx: {e}");

        await mpv.ConnectAsync(); // Linux: /tmp/mpv-socket


        await mpv.PlaylistClear();

        await mpv.LoadFile(@"C:\Videos\a1.mp4");        // inicia a playlist (replace)
        await mpv.LoadFileAppend(@"C:\Videos\a2.mp4");
        await mpv.LoadFileAppend(@"C:\Videos\a3.mp4");
        await mpv.LoadFileAppend(@"C:\Videos\b1.mp4");
        await mpv.LoadFileAppend(@"C:\Videos\b2.mp4");
        await mpv.LoadFileAppend(@"C:\Videos\b3.mp4");
        await mpv.LoadFileAppend(@"C:\Videos\b4.mp4");
        await mpv.LoadFileAppend(@"C:\Videos\c1.mp4");
        await mpv.LoadFileAppend(@"C:\Videos\c2.mp4");

        await mpv.SetVolume(50);
        
        ////await mpv.LoopFile();
        await mpv.LoopPlaylist();

        await mpv.SetPause(false);

        await mpv.SetFullscreen(true);
        //await mpv.PlaylistNext();

        //await mpv.SetPause(false);


        ////await mpv.SeekRelative(10);
        //await mpv.PlaylistNext();

        Console.ReadLine();
    }
}