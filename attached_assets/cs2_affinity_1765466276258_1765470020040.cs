using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Iniciador de CS2 con prioridad HIGH y CPU0 desactivada");
        Console.WriteLine("Ejecuta esto como ADMINISTRADOR");

        int cpuCount = Environment.ProcessorCount;
        // Máscara de afinidad: todos menos CPU0
        long affinityMask = (1L << cpuCount) - 2;

        Process cs2Process = null;

        // Esperar hasta que cs2.exe aparezca
        while (cs2Process == null)
        {
            var processes = Process.GetProcessesByName("cs2");
            if (processes.Length > 0)
                cs2Process = processes[0];
            else
            {
                Thread.Sleep(1000);
            }
        }

        Console.WriteLine($"cs2.exe detectado con PID: {cs2Process.Id}");
        Console.WriteLine("Esperando 20 segundos para que el proceso se inicialice completamente...");
        Thread.Sleep(20000);

        // Bucle para reaplicar prioridad y afinidad continuamente
        while (!cs2Process.HasExited)
        {
            try
            {
                cs2Process.PriorityClass = ProcessPriorityClass.High;
                cs2Process.ProcessorAffinity = (IntPtr)affinityMask;

                Console.WriteLine($"[{DateTime.Now}] Prioridad=HIGH, Afinidad=0x{affinityMask:X}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error aplicando cambios: {ex.Message}");
            }

            Thread.Sleep(5000);
        }

        Console.WriteLine("cs2.exe ha terminado. Saliendo...");
    }
}

