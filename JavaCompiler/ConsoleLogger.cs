using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    class ConsoleLogger
    {
        private static readonly ConsoleColor _happyColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor _neutralColor = ConsoleColor.White;
        private static readonly ConsoleColor _sadColor = ConsoleColor.Red;
        private enum MessageType { Happy, Sad }
        private static void Log(string message, MessageType type)
        {
            if (type == MessageType.Happy)
                Console.ForegroundColor = _happyColor;
            else
                Console.ForegroundColor = _sadColor;
            Console.WriteLine(message);

            Console.ForegroundColor = _neutralColor;
        }
        
        public static void LogUnknownLexeme(string lexeme, int lineNumber)
        {
            Log(string.Format("Unknown lexeme: {0} at line number: {1}", lexeme, lineNumber), MessageType.Sad);
        }
    }
}
