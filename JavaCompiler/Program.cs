using System;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var Scanner = new Scanner(args[1]);

            Scanner.GetNextToken();
        }
    }
}
