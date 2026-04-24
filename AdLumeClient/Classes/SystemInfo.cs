using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace AdLumeClient.Classes;

class SystemInfo
{
    public static void PrintSummary()
    {

        try
        {

            Log.Information($"------------------------------");
            Log.Information($"- RESUMO DO EQUIPAMENTO      -");
            Log.Information($"------------------------------");

            // 🔹 Sistema Operacional
            Log.Information($"Sistema Operacional....: {RuntimeInformation.OSDescription} - Arquitetura: {RuntimeInformation.OSArchitecture} - Framework: {RuntimeInformation.FrameworkDescription}");
            // 🔹 Processador
            Log.Information($"Processadores (lógicos): {Environment.ProcessorCount}");

            // Nome do processador (Windows)
            if (OperatingSystem.IsWindows())
            {
                var cpuName = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
                Log.Information($"CPU....................: {cpuName}");
            }
            else
            {
                // Linux: tenta ler /proc/cpuinfo
                try
                {
                    var cpuLine = File.ReadLines("/proc/cpuinfo").FirstOrDefault(l => l.StartsWith("model name"));
                    if (cpuLine != null)
                    {
                        Log.Information($"CPU....................: {cpuLine.Split(':')[1].Trim()}");
                    }
                }
                catch
                {
                }
            }

            // 🔹 Memória
            if (OperatingSystem.IsWindows())
            {

                var info = GC.GetGCMemoryInfo();
                long TotalPhysicalMemory = info.TotalAvailableMemoryBytes;
                long used = GC.GetTotalMemory(false);
                long AvailablePhysicalMemory = (TotalPhysicalMemory - used);
                Log.Information($"Memória................: Total: {FormatBytes((long)TotalPhysicalMemory)} / Livre: {FormatBytes((long)AvailablePhysicalMemory)}");
            }
            else if (OperatingSystem.IsLinux())
            {
                try
                {
                    var lines = File.ReadAllLines("/proc/meminfo");

                    long total = ParseMemInfo(lines, "MemTotal");
                    long free = ParseMemInfo(lines, "MemAvailable");

                    Log.Information($"Memória................: Total: {FormatBytes(total * 1024)} -  Livre: {FormatBytes(free * 1024)}");
                }
                catch
                {
                    Log.Information("Memória.................: não foi possível obter");
                }
            }

            if (OperatingSystem.IsWindows())
            {
                // 🔹 Discos
                Log.Information("Discos.................:");
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    Log.Information($"                       : {drive.Name}  Total: {FormatBytes(drive.TotalSize)} / Livre: {FormatBytes(drive.AvailableFreeSpace)}");
                }
            }

            TimeSpan Uptime;
            string Hostname;
            string IPs;

            Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            Hostname = Environment.MachineName;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            IPs = string.Join(", ",
                host.AddressList
                    .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(ip => ip.ToString()));

            Log.Information($"Uptime.................: {Uptime}");
            Log.Information($"Hostname...............: {Hostname}");
            Log.Information($"IPs....................: {IPs}");

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro em PrintSummary");
        }

    }

    static long ParseMemInfo(string[] lines, string key)
    {
        var line = lines.First(l => l.StartsWith(key));
        var parts = line.Split(':')[1].Trim().Split(' ');
        return long.Parse(parts[0]); // valor em KB
    }

    static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    public static void PrintLogoAdLume()
    {

        try
        {


            int numero = Random.Shared.Next(2, 10);

            Console.WriteLine($"");

            if (numero == 1)
            {
                Console.WriteLine(Rainbow(@$"                                          dddddddd                                                                                      "));
                Console.WriteLine(Rainbow(@$"               AAA                        d::::::dLLLLLLLLLLL                                                                           "));
                Console.WriteLine(Rainbow(@$"              A:::A                       d::::::dL:::::::::L                                                                           "));
                Console.WriteLine(Rainbow(@$"             A:::::A                      d::::::dL:::::::::L                                                                           "));
                Console.WriteLine(Rainbow(@$"            A:::::::A                     d:::::d LL:::::::LL                                                                           "));
                Console.WriteLine(Rainbow(@$"           A:::::::::A            ddddddddd:::::d   L:::::L               uuuuuu    uuuuuu     mmmmmmm    mmmmmmm       eeeeeeeeeeee    "));
                Console.WriteLine(Rainbow(@$"          A:::::A:::::A         dd::::::::::::::d   L:::::L               u::::u    u::::u   mm:::::::m  m:::::::mm   ee::::::::::::ee  "));
                Console.WriteLine(Rainbow(@$"         A:::::A A:::::A       d::::::::::::::::d   L:::::L               u::::u    u::::u  m::::::::::mm::::::::::m e::::::eeeee:::::ee"));
                Console.WriteLine(Rainbow(@$"        A:::::A   A:::::A     d:::::::ddddd:::::d   L:::::L               u::::u    u::::u  m::::::::::::::::::::::me::::::e     e:::::e"));
                Console.WriteLine(Rainbow(@$"       A:::::A     A:::::A    d::::::d    d:::::d   L:::::L               u::::u    u::::u  m:::::mmm::::::mmm:::::me:::::::eeeee::::::e"));
                Console.WriteLine(Rainbow(@$"      A:::::AAAAAAAAA:::::A   d:::::d     d:::::d   L:::::L               u::::u    u::::u  m::::m   m::::m   m::::me:::::::::::::::::e "));
                Console.WriteLine(Rainbow(@$"     A:::::::::::::::::::::A  d:::::d     d:::::d   L:::::L               u::::u    u::::u  m::::m   m::::m   m::::me::::::eeeeeeeeeee  "));
                Console.WriteLine(Rainbow(@$"    A:::::AAAAAAAAAAAAA:::::A d:::::d     d:::::d   L:::::L         LLLLLLu:::::uuuu:::::u  m::::m   m::::m   m::::me:::::::e           "));
                Console.WriteLine(Rainbow(@$"   A:::::A             A:::::Ad::::::ddddd::::::ddLL:::::::LLLLLLLLL:::::Lu:::::::::::::::uum::::m   m::::m   m::::me::::::::e          "));
                Console.WriteLine(Rainbow(@$"  A:::::A               A:::::Ad:::::::::::::::::dL::::::::::::::::::::::L u:::::::::::::::um::::m   m::::m   m::::m e::::::::eeeeeeee  "));
                Console.WriteLine(Rainbow(@$" A:::::A                 A:::::Ad:::::::::ddd::::dL::::::::::::::::::::::L  uu::::::::uu:::um::::m   m::::m   m::::m  ee:::::::::::::e  "));
                Console.WriteLine(Rainbow(@$"AAAAAAA                   AAAAAAAddddddddd   dddddLLLLLLLLLLLLLLLLLLLLLLLL    uuuuuuuu  uuuummmmmm   mmmmmm   mmmmmm    eeeeeeeeeeeeee  "));
            }
            if (numero == 2)
            {
                Console.WriteLine(Rainbow(@$"                                                                                       "));
                Console.WriteLine(Rainbow(@$"       db                 88  88                                                       "));
                Console.WriteLine(Rainbow(@$"      d88b                88  88                                                       "));
                Console.WriteLine(Rainbow(@$"     d8'`8b               88  88                                                       "));
                Console.WriteLine(Rainbow(@$"    d8'  `8b      ,adPPYb,88  88          88       88  88,dPYba,,adPYba,    ,adPPYba,  "));
                Console.WriteLine(Rainbow(@$"   d8YaaaaY8b    a8""    `Y88  88          88       88  88P'   ""88""    ""8a  a8P_____88"));
                Console.WriteLine(Rainbow(@$"  d8""""""""""""""""8b   8b       88  88          88       88  88      88      88  8PP"""""""""""""""));
                Console.WriteLine(Rainbow(@$" d8'        `8b  ""8a,   ,d88  88          ""8a,   ,a88  88      88      88  ""8b,   ,aa  "));
                Console.WriteLine(Rainbow(@$"d8'          `8b  `""8bbdP""Y8  88888888888  `""YbbdP'Y8  88      88      88   `""Ybbd8""'  "));
            }
            if (numero == 3)
            {
                Console.WriteLine(Rainbow(@$"    e Y8b          888 888                                   "));
                Console.WriteLine(Rainbow(@$"   d8b Y8b     e88 888 888     8888 8888 888 888 8e   ,e e,  "));
                Console.WriteLine(Rainbow(@$"  d888b Y8b   d888 888 888     8888 8888 888 888 88b d88 88b "));
                Console.WriteLine(Rainbow(@$" d888888888b  Y888 888 888  ,d Y888 888P 888 888 888 888   , "));
                Console.WriteLine(Rainbow(@$"d8888888b Y8b  ""88 888 888.d88  ""88 88""  888 888 888  ""YeeP"" "));
            }
            if (numero == 4)
            {
                Console.WriteLine(Rainbow(@$"      .o.             .o8  ooooo                                                "));
                Console.WriteLine(Rainbow(@$"     .888.           ""888  `888'                                                "));
                Console.WriteLine(Rainbow(@$"    .8""888.      .oooo888   888         oooo  oooo  ooo. .oo.  .oo.    .ooooo.  "));
                Console.WriteLine(Rainbow(@$"   .8' `888.    d88' `888   888         `888  `888  `888P""Y88bP""Y88b  d88' `88b "));
                Console.WriteLine(Rainbow(@$"  .88ooo8888.   888   888   888          888   888   888   888   888  888ooo888 "));
                Console.WriteLine(Rainbow(@$" .8'     `888.  888   888   888       o  888   888   888   888   888  888    .o "));
                Console.WriteLine(Rainbow(@$"o88o     o8888o `Y8bod88P"" o888ooooood8  `V88V""V8P' o888o o888o o888o `Y8bod8P' "));
            }
            if (numero == 5)
            {
                Console.WriteLine(Rainbow(@$"MMP""""""""""""""MM       dP M""""MMMMMMMM                              "));
                Console.WriteLine(Rainbow(@$"M' .mmmm  MM       88 M  MMMMMMMM                              "));
                Console.WriteLine(Rainbow(@$"M         `M .d888b88 M  MMMMMMMM dP    dP 88d8b.d8b. .d8888b. "));
                Console.WriteLine(Rainbow(@$"M  MMMMM  MM 88'  `88 M  MMMMMMMM 88    88 88'`88'`88 88ooood8 "));
                Console.WriteLine(Rainbow(@$"M  MMMMM  MM 88.  .88 M  MMMMMMMM 88.  .88 88  88  88 88.  ... "));
                Console.WriteLine(Rainbow(@$"M  MMMMM  MM `88888P8 M         M `88888P' dP  dP  dP `88888P' "));
                Console.WriteLine(Rainbow(@$"MMMMMMMMMMMM          MMMMMMMMMMM                              "));
            }
            if (numero == 6)
            {
                Console.WriteLine(Rainbow(@$"   █████████       █████ █████                                          "));
                Console.WriteLine(Rainbow(@$"  ███░░░░░███     ░░███ ░░███                                           "));
                Console.WriteLine(Rainbow(@$" ░███    ░███   ███████  ░███        █████ ████ █████████████    ██████ "));
                Console.WriteLine(Rainbow(@$" ░███████████  ███░░███  ░███       ░░███ ░███ ░░███░░███░░███  ███░░███"));
                Console.WriteLine(Rainbow(@$" ░███░░░░░███ ░███ ░███  ░███        ░███ ░███  ░███ ░███ ░███ ░███████ "));
                Console.WriteLine(Rainbow(@$" ░███    ░███ ░███ ░███  ░███      █ ░███ ░███  ░███ ░███ ░███ ░███░░░  "));
                Console.WriteLine(Rainbow(@$" █████   █████░░████████ ███████████ ░░████████ █████░███ █████░░██████ "));
                Console.WriteLine(Rainbow(@$"░░░░░   ░░░░░  ░░░░░░░░ ░░░░░░░░░░░   ░░░░░░░░ ░░░░░ ░░░ ░░░░░  ░░░░░░  "));
            }
            if (numero == 7)
            {
                Console.WriteLine(Rainbow(@$"   █████████       █████ █████                                          "));
                Console.WriteLine(Rainbow(@$"  ███▒▒▒▒▒███     ▒▒███ ▒▒███                                           "));
                Console.WriteLine(Rainbow(@$" ▒███    ▒███   ███████  ▒███        █████ ████ █████████████    ██████ "));
                Console.WriteLine(Rainbow(@$" ▒███████████  ███▒▒███  ▒███       ▒▒███ ▒███ ▒▒███▒▒███▒▒███  ███▒▒███"));
                Console.WriteLine(Rainbow(@$" ▒███▒▒▒▒▒███ ▒███ ▒███  ▒███        ▒███ ▒███  ▒███ ▒███ ▒███ ▒███████ "));
                Console.WriteLine(Rainbow(@$" ▒███    ▒███ ▒███ ▒███  ▒███      █ ▒███ ▒███  ▒███ ▒███ ▒███ ▒███▒▒▒  "));
                Console.WriteLine(Rainbow(@$" █████   █████▒▒████████ ███████████ ▒▒████████ █████▒███ █████▒▒██████ "));
                Console.WriteLine(Rainbow(@$"▒▒▒▒▒   ▒▒▒▒▒  ▒▒▒▒▒▒▒▒ ▒▒▒▒▒▒▒▒▒▒▒   ▒▒▒▒▒▒▒▒ ▒▒▒▒▒ ▒▒▒ ▒▒▒▒▒  ▒▒▒▒▒▒  "));
            }
            if (numero == 8)
            {
                Console.WriteLine(Rainbow(@$"  ▄▄▄▄      ▄▄ ▄▄▄                           "));
                Console.WriteLine(Rainbow(@$"▄██▀▀██▄    ██ ███                           "));
                Console.WriteLine(Rainbow(@$"███  ███ ▄████ ███      ██ ██ ███▄███▄ ▄█▀█▄ "));
                Console.WriteLine(Rainbow(@$"███▀▀███ ██ ██ ███      ██ ██ ██ ██ ██ ██▄█▀ "));
                Console.WriteLine(Rainbow(@$"███  ███ ▀████ ████████ ▀██▀█ ██ ██ ██ ▀█▄▄▄ "));
            }
            if (numero == 9)
            {
                Console.WriteLine(Rainbow(@$" █████╗ ██████╗ ██╗     ██╗   ██╗███╗   ███╗███████╗"));
                Console.WriteLine(Rainbow(@$"██╔══██╗██╔══██╗██║     ██║   ██║████╗ ████║██╔════╝"));
                Console.WriteLine(Rainbow(@$"███████║██║  ██║██║     ██║   ██║██╔████╔██║█████╗  "));
                Console.WriteLine(Rainbow(@$"██╔══██║██║  ██║██║     ██║   ██║██║╚██╔╝██║██╔══╝  "));
                Console.WriteLine(Rainbow(@$"██║  ██║██████╔╝███████╗╚██████╔╝██║ ╚═╝ ██║███████╗"));
                Console.WriteLine(Rainbow(@$"╚═╝  ╚═╝╚═════╝ ╚══════╝ ╚═════╝ ╚═╝     ╚═╝╚══════╝"));
            }

            Console.WriteLine($"");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em PrintLogoAdLume: " + ex.Message);
        }

    }

    static string Rainbow(string text)
    {
        var result = "";
        int len = text.Length;

        for (int i = 0; i < len; i++)
        {
            double t = (double)i / len;

            int r = (int)(Math.Sin(0.3 * i + 0) * 127 + 128);
            int g = (int)(Math.Sin(0.3 * i + 2) * 127 + 128);
            int b = (int)(Math.Sin(0.3 * i + 4) * 127 + 128);

            result += $"\x1b[38;2;{r};{g};{b}m{text[i]}";
        }

        result += "\x1b[0m"; // reset
        return result;
    }


}