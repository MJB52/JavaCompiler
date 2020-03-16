using System;

namespace JavaCompiler
{
    public static class Extensions
    {
        public static bool IsLegalWordToken(this char ch) => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.';

        public static bool IsDataType(this Tokens token) =>
            token == Tokens.IntT || token == Tokens.BooleanT || token == Tokens.VoidT;
    }

    public class ParseErrorException : Exception
    {
    }
}