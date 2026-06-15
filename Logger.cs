using System;
using System.IO;

namespace KromaRec.Logging
{
    public enum LogLevel { Debug, Info, Warn, Error }

    public static class Logger
    {
        private static LogLevel _minLevel = LogLevel.Info;
        private static readonly object _lock = new();
        private static StreamWriter? _fileWriter;

        public static void Configure(LogLevel level, string? logFile = null)
        {
            _minLevel = level;
            if (logFile != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
                _fileWriter = new StreamWriter(logFile, append: true) { AutoFlush = true };
            }
        }

        public static void Debug(string component, string msg) => Log(LogLevel.Debug, component, msg);
        public static void Info(string component, string msg)  => Log(LogLevel.Info,  component, msg);
        public static void Warn(string component, string msg)  => Log(LogLevel.Warn,  component, msg);
        public static void Error(string component, string msg) => Log(LogLevel.Error, component, msg);
        public static void Error(string component, Exception ex) => Log(LogLevel.Error, component, ex.ToString());

        private static void Log(LogLevel level, string component, string msg)
        {
            if (level < _minLevel) return;

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string line = $"[{timestamp}] [{level.ToString().ToUpper(),-5}] [{component}] {msg}";

            lock (_lock)
            {
                Console.ForegroundColor = level switch
                {
                    LogLevel.Debug => ConsoleColor.Gray,
                    LogLevel.Info  => ConsoleColor.Cyan,
                    LogLevel.Warn  => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    _              => ConsoleColor.White
                };
                Console.WriteLine(line);
                Console.ResetColor();
                _fileWriter?.WriteLine(line);
            }
        }
    }
}
