using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public static class Globals
    {
        public static string Token { get; set; }
        public static string Lexeme { get; set; }
        public static char Ch { get; set; }
        public static int LineNo { get; set; }
        public static int Value { get; set; }
        public static float ValueR { get; set; }
    }
}
