using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        Console.Title = "CS2 Affinity Optimizer - touch grass";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     CS2 AFFINITY OPTIMIZER - by touch grass            ║");
        Console.WriteLine("║     Prioridad HIGH + CPU0 desactivada                  ║");
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
            Console.WriteLine("[INFO] Asegurate de que Steam este instalado correctamente.");
            Console.ResetColor();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] Steam encontrado en: {steamPath}");
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

                Console.WriteLine("[ESPERANDO] Esperando a que Steam inicie completamente...");
                
                while (!IsSteamRunning())
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                }
                Console.WriteLine();

                Console.WriteLine("[INFO] Steam detectado. Esperando 10 segundos adicionales para inicializacion completa...");
                Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] No se pudo iniciar Steam: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Presiona cualquier tecla para salir...");
                Console.ReadKey();
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
        Console.WriteLine("[INICIANDO] Ejecutando CS2 a traves de Steam...");
        Console.ResetColor();

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "steam://rungameid/730",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] No se pudo iniciar CS2: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[ESPERANDO] Buscando proceso cs2.exe...");
        Console.ResetColor();

        Process? cs2Process = null;

        while (cs2Process == null)
        {
            var processes = Process.GetProcessesByName("cs2");
            if (processes.Length > 0)
                cs2Process = processes[0];
            else
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK] cs2.exe detectado con PID: {cs2Process.Id}");
        Console.ResetColor();
        Console.WriteLine("[INFO] Esperando 20 segundos para inicializacion completa...");
        Thread.Sleep(20000);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("[ACTIVO] Aplicando optimizaciones durante 30 segundos...");
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

                TimeSpan remaining = duration - (DateTime.Now - startTime);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Prioridad=HIGH, Afinidad=0x{affinityMask:X} - Tiempo restante: {remaining.Seconds}s");
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
        Console.WriteLine("║  [COMPLETADO] Optimizaciones aplicadas exitosamente    ║");
        Console.WriteLine("║  La aplicacion se cerrara automaticamente...           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Thread.Sleep(2000);
    }

    static string? GetSteamPath()
    {
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

        return null;
    }

    static bool IsSteamRunning()
    {
        var processes = Process.GetProcessesByName("steam");
        return processes.Length > 0;
    }
}
