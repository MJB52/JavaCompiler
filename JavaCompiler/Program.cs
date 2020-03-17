using System;
using System.IO;

/// <summary>
/// JavaCompiler
/// Author: Michael Bauer
/// Class: Compilers with Dr. Hamer
///
/// Project External References/NugetPackages: FunctionalSharp.DiscriminatedUnions by Patrick Van Lohuizen 
/// </summary>
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

            var scanner = new Scanner(path);
            var symTab = new SymbolTable(Globals.PrimeNo);

            var parser = new Parser(scanner, symTab);
            scanner.GetNextToken();
            parser.Prog();
            symTab.WriteTable(0);
        }
    }
}