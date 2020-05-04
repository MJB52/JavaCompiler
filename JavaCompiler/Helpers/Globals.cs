namespace JavaCompiler
{
    public static class Globals
    {
        public const int PrimeNo = 307;
        public static string Lexeme { get; set; }
        public static char Ch { get; set; }
        public static bool IsLiteral { get; set; } = false;
        public static Tokens Token { get; set; }
        public static int Depth { get; set; } = 0;
        public static int LineNo { get; set; } = 1;
        
        public static ClassType? ClassT = null;
        public static ConstantType? ConstT = null;
        public static FunctionType? FuncT = null;
        public static VarType? VarT = null;
        public static TypeOfVariable TypeOfVar { get; set; }
        public static int Offset { get; set; } = 0;
        public static string TempOffsetName { get; set; } = string.Empty;
    }

    public enum TypeOfVariable
    {
        VoidType,
        BoolType,
        IntType,
    }

    public enum EntryType
    {
        ClassType,
        ConstantType,
        FunctionType,
        VarType,
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
        LiteralT,
        NotT,
        SignOpT,
        WriteT,
        WritelnT, 
        ReadT
    }
}