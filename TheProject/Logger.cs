using System;
using System.IO;

namespace TheProject
{
    internal static class Logger
    {
        private const string FILE_NAME = "log.txt";

        public static void Clear()
        {
            File.Delete(FILE_NAME);
        }

        public static void Log(string formatString, params object[] args)
        {
            string message = string.Format(formatString, args);
            message += Environment.NewLine;
            File.AppendAllText(FILE_NAME, message);
        }

        public static void Log(string message)
        {
            Log("{0}", message);
        }
    }
}