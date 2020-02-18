using System;

namespace JavaCompiler
{
    internal static class ConsoleLogger
    {
        private static readonly ConsoleColor _happyColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor _neutralColor = ConsoleColor.White;
        private static readonly ConsoleColor _sadColor = ConsoleColor.Red;

        private static void Log(string message, MessageType type)
        {
            if (type == MessageType.Happy)
                Console.ForegroundColor = _happyColor;
            else
                Console.ForegroundColor = _sadColor;
            Console.WriteLine(message);

            Console.ForegroundColor = _neutralColor;
        }

        public static void UnknownLexeme(string lexeme, int lineNumber)
        {
            Log($"Unknown lexeme: \"{lexeme}\" at line number: {lineNumber.ToString()}", MessageType.Sad);
        }

        public static void IllegalLexeme(string lexeme, int lineNumber)
        {
            Log($"Illegal lexeme: \"{lexeme}\" at line number: {lineNumber.ToString()}", MessageType.Sad);
        }

        public static void NoFilePassed()
        {
            Log("Please pass in a file to be compiled. ", MessageType.Sad);
        }

        public static void FileNotFound(string fileName)
        {
            Log($"Could not find file: {fileName}. ", MessageType.Sad);
        }

        private enum MessageType
        {
            Happy,
            Sad
        }
    }
}