namespace JavaCompiler
{
    public static class Globals
    {
        public static string Lexeme { get; set; }
        public static char Ch { get; set; }
        public static bool IsLiteral { get; set; } = false;
        public static Tokens Token { get; set; }
        public static int LineNo { get; set; } = 1;
    }

    public enum TypeOfVariable
    {
        CharType,
        IntType,
        FloatType
    }

    public enum ParameterPassMode
    {
        Value,
        Reference,
        Out
    }

    public enum Tokens
    {
        ClassT,
        PublicT,
        StaticT,
        VoidT,
        MainT,
        FinalT,
        StringT,
        ExtendsT,
        ReturnT,
        IntT,
        BooleanT,
        IfT,
        ElseT,
        WhileT,
        PrintT,
        LengthT,
        TrueT,
        FalseT,
        ThisT,
        NewT,
        LParenT,
        RParenT,
        LBrackT,
        RBrackT,
        LBraceT,
        RBraceT,
        CommaT,
        SemiT,
        PeriodT,
        IdT,
        NumT,
        QuoteT,
        AssignOpT,
        AddOpT,
        MulOpT,
        RelOpT,
        EofT,
        UnknownT,
        LiteralT
    }
}