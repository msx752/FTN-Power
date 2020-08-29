using System;
using System.Diagnostics;

namespace FModel.Methods.Utilities
{
    static class DebugHelper
    {
        public static void WriteLine(string message = "")
        {
            Console.WriteLine(message);
        }

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public static void WriteException(string exception, string message = "Exception")
        {
            Console.WriteLine($"{message}: " + exception);

        }

        public static void WriteException(Exception exception, string message = "Exception")
        {
            WriteException(exception.ToString(), message);
        }

    }
}
