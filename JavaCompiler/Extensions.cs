namespace JavaCompiler
{
    public static class Extensions
    {
        public static bool IsLegalWordToken(this char ch) => char.IsLetter(ch) || ch == '_' || ch == '.';
    }
}