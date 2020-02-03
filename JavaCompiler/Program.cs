using System;
using System.IO;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
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

            var Scanner = new Scanner(path);

            Scanner.GetNextToken();

            foreach (var item in Globals.FileTokens)
            {
                var token = Enum.GetName(typeof(Tokens), item.Key);
                Console.WriteLine(string.Format("Lexeme: {0} Token: {1}", token, item.Value.Value));
            }
        }
    }
}
