using DotBet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBet.Models
{
    public static class FileLogger 
    {
        private static readonly object _lock = new object();
        private static string _logFilePath = Path.Combine(AppContext.BaseDirectory, "log.txt");

        public static void Log(string message)
        {
            lock (_lock) 
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));

                File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            }
        }

    }
}
