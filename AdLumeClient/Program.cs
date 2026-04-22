using System;
using System.Threading.Tasks;
using VideoPlayback;

class Program
{
    static async Task Main()
    {

        var exePath = AppContext.BaseDirectory;

        //var mpv = new MpvClient();
        //
        //// Linux: "/tmp/mpv-socket"
        //// Windows: ignora parametro e usa pipe interno
        //await mpv.ConnectAsync("/tmp/mpv-socket");

        await MpvManager.RestartMpvAsync(Path.Combine(exePath, @"mpv\mpv.exe"));
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


#if true
        //await mpv.LoadFile(Path.Combine(exePath, @"Videos\video_manha_1.mp4.mp4"));        // inicia a playlist (replace)
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_1.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_2.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_3.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_a.mp4"));

        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_1.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_2.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_manha_3.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_b.mp4"));
#endif

#if false
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_1.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_2.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_a.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_1.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_tarde_2.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_b.mp4"));
#endif

#if false
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_1.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_2.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_a.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_1.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_noite_2.mp4"));
        await mpv.LoadFileAppend(Path.Combine(exePath, @"Videos\video_promo_b.mp4"));
#endif


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