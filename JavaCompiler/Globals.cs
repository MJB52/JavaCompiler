using System;
using System.Collections.Generic;

namespace JavaCompiler
{
    public static class Globals
    {
        public static string Lexeme { get; set; }
        public static char Ch { get; set; }
        public static bool IsLiteral { get; set; } = false;

        public static int LineNo { get; set; } = 1;
        //public static List<KeyValuePair<Tokens, ILexeme>> FileTokens { get; } = new List<KeyValuePair<Tokens, ILexeme>>();

        public static void Print(KeyValuePair<Tokens, ILexeme> keyPair)
        {
            var token = Enum.GetName(typeof(Tokens), keyPair.Key);
            var line = $"{token,-10} | {keyPair.Value.Value,-20} | " + (keyPair.Value.Type != ValueType.None
                           ? Enum.GetName(typeof(ValueType), keyPair.Value.Type)
                           : "");
            Console.WriteLine(line);
        }
    }

    public enum Tokens
    {
        ClassT,
        PublicT,
        StaticT,
        VoidT,
        MainT,
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