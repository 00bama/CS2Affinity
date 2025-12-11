using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;

class Program
{
    static readonly string CS2_DEFAULT_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\game\bin\win64\cs2.exe";
    
    static void Main()
    {
        Console.Title = "CS2 Affinity - touch grass";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        CS2 AFFINITY - by touch grass                   ║");
        Console.WriteLine("║        Prioridad HIGH + CPU0 desactivada               ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        int cpuCount = Environment.ProcessorCount;
        long affinityMask = (1L << cpuCount) - 2;

        Console.WriteLine($"[INFO] CPUs detectadas: {cpuCount}");
        Console.WriteLine($"[INFO] Mascara de afinidad: 0x{affinityMask:X} (CPU0 excluida)");
        Console.WriteLine();

        string? steamPath = GetSteamPath();
        if (string.IsNullOrEmpty(steamPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] No se pudo encontrar la instalacion de Steam.");
            Console.ResetColor();
            Thread.Sleep(3000);
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] Steam: {steamPath}");
        Console.ResetColor();

        if (!IsSteamRunning())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INICIANDO] Abriendo Steam...");
            Console.ResetColor();

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = steamPath,
                    UseShellExecute = true
                });

                Console.WriteLine("[ESPERANDO] Esperando Steam...");
                
                int steamWaitTime = 0;
                while (!IsSteamRunning() && steamWaitTime < 60)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                    steamWaitTime++;
                }
                Console.WriteLine();

                if (!IsSteamRunning())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] Steam no inicio a tiempo.");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    return;
                }

                Console.WriteLine("[INFO] Steam listo. Esperando 5 segundos...");
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
                Thread.Sleep(3000);
                return;
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK] Steam ya esta en ejecucion.");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[INICIANDO] Ejecutando CS2...");
        Console.ResetColor();

        try
        {
            if (File.Exists(CS2_DEFAULT_PATH))
            {
                Console.WriteLine("[INFO] Usando ruta directa de CS2...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = CS2_DEFAULT_PATH,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(CS2_DEFAULT_PATH)
                });
            }
            else
            {
                Console.WriteLine("[INFO] Usando Steam para iniciar CS2...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "steam://rungameid/730",
                    UseShellExecute = true
                });
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {ex.Message}");
            Console.ResetColor();
            Thread.Sleep(3000);
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[ESPERANDO] Buscando cs2.exe...");
        Console.ResetColor();

        Process? cs2Process = null;
        int waitTime = 0;

        while (cs2Process == null && waitTime < 120)
        {
            var processes = Process.GetProcessesByName("cs2");
            if (processes.Length > 0)
                cs2Process = processes[0];
            else
            {
                Console.Write(".");
                Thread.Sleep(1000);
                waitTime++;
            }
        }

        if (cs2Process == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[ERROR] CS2 no inicio a tiempo.");
            Console.ResetColor();
            Thread.Sleep(3000);
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK] cs2.exe detectado - PID: {cs2Process.Id}");
        Console.ResetColor();
        Console.WriteLine("[INFO] Esperando 15 segundos para inicializacion...");
        Thread.Sleep(15000);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("[ACTIVO] Aplicando optimizaciones (30 segundos)...");
        Console.ResetColor();
        Console.WriteLine();

        DateTime startTime = DateTime.Now;
        TimeSpan duration = TimeSpan.FromSeconds(30);

        while (!cs2Process.HasExited && (DateTime.Now - startTime) < duration)
        {
            try
            {
                cs2Process.PriorityClass = ProcessPriorityClass.High;
                cs2Process.ProcessorAffinity = (IntPtr)affinityMask;

                int remaining = (int)(duration - (DateTime.Now - startTime)).TotalSeconds;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] HIGH + 0x{affinityMask:X} ({remaining}s)");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
            }

            Thread.Sleep(5000);
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  [OK] Optimizaciones aplicadas - Cerrando...           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Thread.Sleep(2000);
    }

    static string? GetSteamPath()
    {
        try
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                if (key != null)
                {
                    string? steamExe = key.GetValue("SteamExe") as string;
                    if (!string.IsNullOrEmpty(steamExe) && File.Exists(steamExe))
                        return steamExe;
                }
            }
        }
        catch { }

        string[] possiblePaths = new string[]
        {
            @"C:\Program Files (x86)\Steam\steam.exe",
            @"C:\Program Files\Steam\steam.exe",
            @"D:\Steam\steam.exe",
            @"D:\Program Files (x86)\Steam\steam.exe",
            @"E:\Steam\steam.exe"
        };

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
                return path;
        }

        return null;
    }

    static bool IsSteamRunning()
    {
        var processes = Process.GetProcessesByName("steam");
        return processes.Length > 0;
    }
}
