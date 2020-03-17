using System;

namespace JavaCompiler
{
    public class Parser
    {
        private readonly IScanner _scanner;
        private readonly ISymbolTable _symTab;

        public Parser(IScanner scanner, ISymbolTable symTab)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _symTab = symTab ?? throw new ArgumentNullException(nameof(symTab));
        }

        public void Prog()
        {
            try
            {
                MoreClasses();
                MainClass();
                if (Globals.Token == Tokens.EofT)
                    ConsoleLogger.SuccessfulParse();
                else
                    ConsoleLogger.ParseError(Tokens.EofT, Globals.Token, Globals.LineNo);
            }
            catch (ParseErrorException)
            {
            }
        }

        private void MainClass()
        {
            Match(Tokens.FinalT);
            Match(Tokens.ClassT);
            Match(Tokens.IdT);
            Match(Tokens.LBraceT);
            Match(Tokens.PublicT);
            Match(Tokens.StaticT);
            Match(Tokens.VoidT);
            Match(Tokens.MainT);
            Match(Tokens.LParenT);
            Match(Tokens.StringT);
            Match(Tokens.LBrackT);
            Match(Tokens.RBrackT);
            Match(Tokens.IdT);
            Match(Tokens.RParenT);
            Match(Tokens.LBraceT);
            SeqOfStatements();
            Match(Tokens.RBraceT);
            Match(Tokens.RBraceT);
        }

        private void MoreClasses()
        {
            ClassDecl();
            if (Globals.Token == Tokens.ClassT)
                MoreClasses();
        }

        private void ClassDecl()
        {
            if (Globals.Token != Tokens.ClassT)
                return;
            Match(Tokens.ClassT);
            Match(Tokens.IdT);
            if (Globals.Token == Tokens.ExtendsT)
            {
                Match(Tokens.ExtendsT);
                Match(Tokens.IdT);
            }

            Match(Tokens.LBraceT);
            VarDecl();
            MethodDecl();
            Match(Tokens.RBraceT);
        }

        private void VarDecl()
        {
            if (Globals.Token == Tokens.FinalT)
            {
                Match(Tokens.FinalT);
                Type();
                Match(Tokens.IdT);
                Match(Tokens.RelOpT);
                Match(Tokens.NumT);
                Match(Tokens.SemiT);
                VarDecl();
            }
            else if (Globals.Token.IsDataType())
            {
                Type();
                IdentifierList();
                Match(Tokens.SemiT);
                VarDecl();
            }
        }

        private void IdentifierList()
        {
            if (Globals.Token == Tokens.IdT)
            {
                Match(Tokens.IdT);
                IdentifierList();
            }
            else if (Globals.Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                Match(Tokens.IdT);
                IdentifierList();
            }
        }

        private void Type()
        {
            if (Globals.Token == Tokens.IntT)
                Match(Tokens.IntT);
            else if (Globals.Token == Tokens.BooleanT)
                Match(Tokens.BooleanT);
            else if (Globals.Token == Tokens.VoidT)
                Match(Tokens.VoidT);
            else
                return;
        }

        private void MethodDecl()
        {
            if (Globals.Token != Tokens.PublicT)
                return;

            Match(Tokens.PublicT);
            Type();
            Match(Tokens.IdT);
            Match(Tokens.LParenT);
            FormalList();
            Match(Tokens.RParenT);
            Match(Tokens.LBraceT);
            VarDecl();
            SeqOfStatements();
            Match(Tokens.ReturnT);
            Expr();
            Match(Tokens.SemiT);
            Match(Tokens.RBraceT);
        }

        private void FormalList()
        {
            if (!Globals.Token.IsDataType())
                return;

            Type();
            Match(Tokens.IdT);
            FormalRest();
        }

        private void FormalRest()
        {
            if (Globals.Token != Tokens.CommaT)
                return;

            Match(Tokens.CommaT);
            Type();
            Match(Tokens.IdT);
            FormalRest();
        }

        private void SeqOfStatements()
        {
        }

        private void Expr()
        {
        }

        private void Match(Tokens token)
        {
            if (Globals.Token == token)
            {
                _scanner.GetNextToken();
            }
            else
            {
                ConsoleLogger.ParseError(token, Globals.Token, Globals.LineNo);
                throw new ParseErrorException(); //do this to exit the call stack essentially
            }
        }
    }
}