using System;
using System.IO;

/// <summary>
/// JavaCompiler
/// Author: Michael Bauer
/// Class: Compilers with Dr. Hamer
///
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

            var cwd = Directory.GetCurrentDirectory();
            var inputFilePath = Path.Combine(cwd, args[0]);

            if (!File.Exists(inputFilePath))
            {
                ConsoleLogger.FileNotFound(args[0]);
                Environment.Exit(-1);
            }

            var tacFile = args[0].Substring(0, args[0].LastIndexOf('.')) + ".tac";
            var tacPath = Path.Combine(cwd, tacFile);

            var asmFile = args[0].Substring(0, args[0].LastIndexOf('.')) + ".asm";
            var asmPath = Path.Combine(cwd, asmFile);

            var scanner = new Scanner(inputFilePath);
            var printer = new TACWriter(tacPath);
            var reader = new TACReader(tacPath);
            var asmWriter = new ASMWriter(asmPath, reader);
            var symTab = new SymbolTable(Globals.PrimeNo);
            var parser = new Parser(scanner, symTab, printer, asmWriter);
            scanner.GetNextToken();
            parser.Prog();
            asmWriter.StartPrint();
        }
    }
}