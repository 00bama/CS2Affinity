using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;

class P
{
    static readonly string _p = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\game\bin\win64\cs2.exe";
    
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

        int _c = Environment.ProcessorCount;
        long _m = (1L << _c) - 2;

        Console.WriteLine($"[INFO] CPUs detectadas: {_c}");
        Console.WriteLine($"[INFO] Mascara de afinidad: 0x{_m:X} (CPU0 excluida)");
        Console.WriteLine();

        string _s = G();
        if (string.IsNullOrEmpty(_s))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] No se pudo encontrar la instalacion de Steam.");
            Console.ResetColor();
            Thread.Sleep(3000);
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] Steam: {_s}");
        Console.ResetColor();

        if (!R())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INICIANDO] Abriendo Steam...");
            Console.ResetColor();

            try
            {
                Process.Start(new ProcessStartInfo { FileName = _s, UseShellExecute = true });
                Console.WriteLine("[ESPERANDO] Esperando Steam...");
                
                int _w = 0;
                while (!R() && _w < 60) { Console.Write("."); Thread.Sleep(1000); _w++; }
                Console.WriteLine();

                if (!R())
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
            catch (Exception _e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {_e.Message}");
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
            if (File.Exists(_p))
            {
                Console.WriteLine("[INFO] Usando ruta directa de CS2...");
                Process.Start(new ProcessStartInfo { FileName = _p, UseShellExecute = true, WorkingDirectory = Path.GetDirectoryName(_p) });
            }
            else
            {
                Console.WriteLine("[INFO] Usando Steam para iniciar CS2...");
                Process.Start(new ProcessStartInfo { FileName = "steam://rungameid/730", UseShellExecute = true });
            }
        }
        catch (Exception _e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {_e.Message}");
            Console.ResetColor();
            Thread.Sleep(3000);
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[ESPERANDO] Buscando cs2.exe...");
        Console.ResetColor();

        Process _x = null;
        int _t = 0;

        while (_x == null && _t < 120)
        {
            var _a = Process.GetProcessesByName("cs2");
            if (_a.Length > 0) _x = _a[0];
            else { Console.Write("."); Thread.Sleep(1000); _t++; }
        }

        if (_x == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[ERROR] CS2 no inicio a tiempo.");
            Console.ResetColor();
            Thread.Sleep(3000);
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK] cs2.exe detectado - PID: {_x.Id}");
        Console.ResetColor();
        Console.WriteLine("[INFO] Esperando 15 segundos para inicializacion...");
        Thread.Sleep(15000);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("[ACTIVO] Aplicando optimizaciones (30 segundos)...");
        Console.ResetColor();
        Console.WriteLine();

        DateTime _d = DateTime.Now;
        TimeSpan _u = TimeSpan.FromSeconds(30);

        while (!_x.HasExited && (DateTime.Now - _d) < _u)
        {
            try
            {
                _x.PriorityClass = ProcessPriorityClass.High;
                _x.ProcessorAffinity = (IntPtr)_m;

                int _r = (int)(_u - (DateTime.Now - _d)).TotalSeconds;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] HIGH + 0x{_m:X} ({_r}s)");
                Console.ResetColor();
            }
            catch (Exception _e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {_e.Message}");
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

    static string G()
    {
        try
        {
            using (RegistryKey _k = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                if (_k != null)
                {
                    string _v = _k.GetValue("SteamExe") as string;
                    if (!string.IsNullOrEmpty(_v) && File.Exists(_v)) return _v;
                }
            }
        }
        catch { }

        string[] _l = new string[]
        {
            @"C:\Program Files (x86)\Steam\steam.exe",
            @"C:\Program Files\Steam\steam.exe",
            @"D:\Steam\steam.exe",
            @"D:\Program Files (x86)\Steam\steam.exe",
            @"E:\Steam\steam.exe"
        };

        foreach (string _i in _l) { if (File.Exists(_i)) return _i; }
        return null;
    }

    static bool R() { return Process.GetProcessesByName("steam").Length > 0; }
}
