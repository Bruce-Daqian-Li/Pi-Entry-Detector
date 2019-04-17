using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using EDD;

namespace EDD.Utils
{
    /// <summary>
    /// Logging Class, Some Logging things. It's the core of every operations.
    /// It Needs to be Initialised once the application started, before anything else.
    /// </summary>
    public static class L
    {
        public static LogLevel _LogLevel { private get; set; }
        private static object LOCKER { get; } = new object();

        private static StreamWriter Fs { get; set; }
        private static string LogFilePath { get; set; }

        //Actually it should be a new instance when used, However, due to the "static" LW class, there's only one instance.
        //To prevent instances takes up your memory, a constant instance is used...

        public static void InitLog()
        {
            LogFilePath = Environment.CurrentDirectory + "/Logs/" + DateTime.Now.ToFileNameString() + ".log";
            Directory.CreateDirectory(Environment.CurrentDirectory + "/Logs/");
            Fs = File.CreateText(LogFilePath);
            Fs.AutoFlush = true;
            WriteLogInternal(_LogLevel, "Log is Now Initialised!");
        }

        private static void WriteLogInternal(LogLevel level, string LogMessage)
        {
            if (level < _LogLevel) return;
            StackFrame frame = new StackTrace().GetFrame(2);

            StringBuilder MessageBuilder = new StringBuilder();
            //MessageBuilder.Append($"[{frame.GetFileName()}({frame.GetFileLineNumber()}L,{frame.GetFileColumnNumber()}C)]\r\n");
            MessageBuilder.Append($"[{DateTime.Now.ToDetailedString()} - {level.ToString()}] ");
            MessageBuilder.Append($"[{frame?.GetMethod()?.ReflectedType.Name}::{frame?.GetMethod()?.Name}] - ");
            MessageBuilder.Append(LogMessage);
            MessageBuilder.Append("\r\n");

            string Message = MessageBuilder.ToString();
            lock (LOCKER)
            {
                switch (level)
                {
                    case LogLevel.DBG:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogLevel.INF:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case LogLevel.WRN:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case LogLevel.ERR:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                Console.Write(Message);
                Console.ResetColor();

                Fs.Write(Message);
            }
        }

        public static void D(string Message) => WriteLogInternal(LogLevel.DBG, Message);
        public static void I(string Message) => WriteLogInternal(LogLevel.INF, Message);
        public static void W(string Message) => WriteLogInternal(LogLevel.WRN, Message);
        public static void E(string Message) => WriteLogInternal(LogLevel.ERR, Message + new StackTrace(skipFrames: 1).ToString());
        public static void LogException(this Exception ex) => WriteLogInternal(LogLevel.ERR, ex.ShowStackTrace());
        public static string ShowStackTrace(this Exception ex)
        {
            StringBuilder output = new StringBuilder();
            output.Append(ex.ToString());
            int depth = 1;
            Exception _ex = ex;
            while (true)
            {
                output.Append("\r\nDepth: " + depth);
                output.Append("\r\n" + _ex.ToString());
                output.Append(_ex.StackTrace);
                output.Append("\r\n============== END of An Exception Stack Trace ==============\r\n");
                if (_ex.InnerException == null) break;
                else _ex = _ex.InnerException;
                depth++;
            }
            return output.ToString();
        }
    }
    public enum LogLevel { DBG = 0, INF = 1, WRN = 2, ERR = 3 }
}
