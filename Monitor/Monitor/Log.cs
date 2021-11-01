using System;
using System.IO;

namespace Monitor
{
    static class Log
    {
        public static void Write(string message)
        {
            const string fileName = "monitor.log";

            using (StreamWriter sw = File.AppendText(fileName))
                sw.WriteLine(DateTime.Now.ToString() + ": " + message);
        }
    }
}
