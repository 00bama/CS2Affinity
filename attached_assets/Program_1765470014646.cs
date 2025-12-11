using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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
        Console.WriteLine("[ACTIVO] Aplicando optimizaciones cada 5 segundos...");
        Console.ResetColor();
        Console.WriteLine();

        while (!cs2Process.HasExited)
        {
            try
            {
                cs2Process.PriorityClass = ProcessPriorityClass.High;
                cs2Process.ProcessorAffinity = (IntPtr)affinityMask;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Prioridad=HIGH, Afinidad=0x{affinityMask:X}");
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

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine("[FIN] cs2.exe ha terminado. Presiona cualquier tecla para salir...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
