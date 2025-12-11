using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.Principal;

class P
{
    static readonly string _p = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\game\bin\win64\cs2.exe";
    
    static void Main()
    {
        Console.Title = "CS2 Affinity - 00bama";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        CS2 AFFINITY - by 00bama                         ║");
        Console.WriteLine("║        Prioridad HIGH + CPU0 desactivada               ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        int _c = Environment.ProcessorCount;
        long _m = (1L << _c) - 2;

        Console.WriteLine($"[INFO] CPUs detectadas: {_c}");
        Console.WriteLine($"[INFO] Mascara de afinidad: 0x{_m:X} (CPU0 excluida)");
        Console.WriteLine();

        // Aviso si no se ejecuta con privilegios elevados (afinidad/prioridad puede fallar)
        try
        {
            bool _isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!_isAdmin)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[AVISO] Ejecutando sin privilegios elevados. Para aplicar prioridad/afinidad completa, ejecuta como Administrador.");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        catch { }

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
                Console.WriteLine("[ESPERANDO] Esperando Steam y SteamWebHelper...");
                // Lanzamos Steam y usamos modo estricto para evitar falsos positivos (steamwebhelper puede aparecer temprano)
                Process.Start(new ProcessStartInfo { FileName = _s, UseShellExecute = true });

                // Espera hasta que Steam esté listo y, preferiblemente, que steamwebhelper.exe esté ejecutándose
                if (!SteamHelper.WaitForSteamFullyReady(totalTimeoutMs: 90000, webHelperTimeoutMs: 30000, fallbackWaitMs: 10000, strictWhenJustStarted: true))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] Steam no inicio a tiempo o no está completamente listo.");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    return;
                }

                Console.WriteLine("[INFO] Steam listo.");
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
            Console.WriteLine("[OK] Steam ya esta en ejecucion. Verificando que esté completamente listo...");
            Console.ResetColor();

            // Asegurarse de que Steam esté realmente listo antes de continuar
            if (!SteamHelper.WaitForSteamFullyReady(totalTimeoutMs: 90000, webHelperTimeoutMs: 30000, fallbackWaitMs: 10000))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Steam no está completamente listo. Intenta abrir Steam y vuelve a ejecutar la aplicación.");
                Console.ResetColor();
                Thread.Sleep(3000);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[INFO] Steam listo.");
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

    static class SteamHelper
    {
        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        public static bool WaitForSteamReady(int timeoutMs = 60000)
        {
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                var proc = Process.GetProcessesByName("steam").FirstOrDefault();
                if (proc != null)
                {
                    try { proc.WaitForInputIdle(2000); } catch { }
                    proc.Refresh();
                    if (proc.MainWindowHandle != IntPtr.Zero && IsWindowVisible(proc.MainWindowHandle) && !string.IsNullOrEmpty(proc.MainWindowTitle))
                        return true;
                }

                Thread.Sleep(300);
            }

            return false;
        }

        // Espera adicional para steamwebhelper.exe (mejora la detección de que Steam ya terminó de iniciar sesión/UI)
        public static bool WaitForSteamFullyReady(int totalTimeoutMs = 120000, int webHelperTimeoutMs = 60000, int fallbackWaitMs = 15000, bool strictWhenJustStarted = false)
        {
            var swTotal = Stopwatch.StartNew();

            while (swTotal.ElapsedMilliseconds < totalTimeoutMs)
            {
                var steamProc = Process.GetProcessesByName("steam").FirstOrDefault();
                if (steamProc == null)
                {
                    Thread.Sleep(300);
                    continue;
                }

                if (Process.GetProcessesByName("steamwebhelper").Any())
                {
                    
                    if (strictWhenJustStarted)
                    {
                        try
                        {
                            var upSec = (DateTime.Now - steamProc.StartTime).TotalSeconds;
                            if (upSec < 12)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine($"[INFO] steamwebhelper detectado pero Steam se inició hace {upSec:F1}s (<12s); esperando más... ({swTotal.ElapsedMilliseconds}ms)");
                                Console.ResetColor();
                                Thread.Sleep(500);
                                continue;
                            }
                        }
                        catch { }
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[INFO] steamwebhelper detectado; Steam listo.");
                    Console.ResetColor();
                    return true;
                }

                // 2) Ventana principal visible
                try { steamProc.WaitForInputIdle(2000); } catch { }
                steamProc.Refresh();
                if (steamProc.MainWindowHandle != IntPtr.Zero && IsWindowVisible(steamProc.MainWindowHandle) && !string.IsNullOrEmpty(steamProc.MainWindowTitle))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[INFO] Ventana principal de Steam visible; Steam listo.");
                    Console.ResetColor();
                    return true;
                }

                // 3) Si Steam lleva suficiente tiempo ejecutándose, considerarlo listo
                try
                {
                    var upSec = (DateTime.Now - steamProc.StartTime).TotalSeconds;
                    if (upSec >= 8)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[INFO] Steam en ejecución desde hace suficiente tiempo; asumiendo listo.");
                        Console.ResetColor();
                        return true;
                    }
                }
                catch { }

                Thread.Sleep(300);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[AVISO] No se detectó Steam completamente; esperando un tiempo extra antes de continuar...");
            Console.ResetColor();
            Thread.Sleep(fallbackWaitMs);

            var sp = Process.GetProcessesByName("steam").FirstOrDefault();
            if (sp != null)
            {
                try
                {
                    var upSec = (DateTime.Now - sp.StartTime).TotalSeconds;
                    if (upSec < 8)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] Steam se inició hace muy poco (<8s); inténtalo de nuevo más tarde.");
                        Console.ResetColor();
                        return false;
                    }
                }
                catch { }
            }

            return true;
        }
    }
