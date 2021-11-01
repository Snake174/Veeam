using System;
using System.Diagnostics;
using System.Threading;

namespace Monitor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string checkedProcess; // отслеживаемый процесс
            int TTL; // время жизни процесса (мин)
            int checkFreq; // частота проверки (мин)

            // Если аргументы не переданы, то считываем их из файла настроек
            if (args.Length != 3)
            {
                INI ini = new INI("monitor.ini");
                checkedProcess = ini.Read("Process", "Settings");
                TTL = Int32.Parse(ini.Read("TTL", "Settings")) * 1000 * 60;
                checkFreq = Int32.Parse(ini.Read("CheckFrequncy", "Settings")) * 1000 * 60;
            }
            else
            {
                checkedProcess = args[0];
                TTL = Int32.Parse(args[1]) * 1000 * 60;
                checkFreq = Int32.Parse(args[2]) * 1000 * 60;
            }

            Object thisLock = new Object();

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate {
                Thread t = new Thread(new ThreadStart(delegate {
                    lock (thisLock)
                    {
                        do
                        {
                            Process[] processes = Process.GetProcessesByName(checkedProcess);

                            foreach (Process p in processes)
                            {
                                if ((DateTime.Now - p.StartTime).TotalMilliseconds >= TTL)
                                {
                                    p.Kill();
                                    Log.Write($"Process {checkedProcess} [{p.Id}] was closed (work time more than {TTL / 60000} min)");
                                }
                            }

                            Thread.Sleep(checkFreq);
                        }
                        while (true);
                    }
                }));
                t.Priority = ThreadPriority.Lowest;
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.IsBackground = true;
            }), null);

            Console.WriteLine("Checking...");
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }
    }
}
