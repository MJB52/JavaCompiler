using System;
using System.IO;

namespace JavaCompiler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ConsoleLogger.NoFilePassed();
                Environment.Exit(-1);
            }

            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, args[0]);

            if (!File.Exists(path))
            {
                ConsoleLogger.FileNotFound(args[0]);
                Environment.Exit(-1);
            }

            Console.WriteLine($"{"Token",-10} | {"Lexeme",-20} | {"Attributes",0}");
            Console.WriteLine(new string('-', 46));

            var Scanner = new Scanner(path);
            Scanner.GetNextToken();
        }
    }
}