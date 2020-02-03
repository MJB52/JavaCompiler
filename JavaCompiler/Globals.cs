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
        public static List<KeyValuePair<Tokens, ILexeme>> FileTokens { get; } = new List<KeyValuePair<Tokens, ILexeme>>();
    }

    public enum Tokens {
        ClassT, PublicT, StaticT, VoidT, MainT, StringT,
        ExtendsT, ReturnT, IntT, BooleanT, IfT, ElseT,
        WhileT, PrintT, LengthT, TrueT, FalseT, ThisT, NewT,
        LParenT, RParenT, LBrackT, RBrackT, LBraceT, RBraceT,
        CommaT, SemiT, PeriodT, IdT, NumT, LiteralT, QuoteT,
        AssignOpT, AddOpT, MulOpT, RelOpT, EofT, UnknownT
    };
}
