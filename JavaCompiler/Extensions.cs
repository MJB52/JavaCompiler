namespace JavaCompiler
{
    public static class Extensions
    {
        public static bool IsLegalWordToken(this char ch) => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.';
    }
}