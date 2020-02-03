using System;

namespace JavaCompiler
{
    public static class Extensions
    {
        public static bool IsLegalWordToken(this Char ch)
        {
            return char.IsLetter(ch) || (ch == '_') || ch == '.';
        }
    }
}