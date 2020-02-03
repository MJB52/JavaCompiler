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
            Console.WriteLine(string.Format("{0,-10} | {1,-20} | {2,-0}", "Token", "Lexeme", "Attributes" ));
            string line = string.Empty;
            foreach (var item in Globals.FileTokens)
            {
                var token = Enum.GetName(typeof(Tokens), item.Key);
                line = string.Format("{0,-10} | {1,-20} | ", token, item.Value.Value) + (item.Value.Type != ValueType.None ? Enum.GetName(typeof(ValueType), item.Value.Type) : "");
                Console.WriteLine(line);
            }
        }
    }
}
