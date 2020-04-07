using System;
using System.Data;
using JavaCompiler.Entry_Types;

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
                _symTab.WriteTable(Globals.Depth);
                if (Globals.Token == Tokens.EofT)
                    ; //ConsoleLogger.SuccessfulParse();
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
            _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.ClassType);
            var type = _symTab.Lookup(Globals.Lexeme);
            type.TypeOfEntry = new Union<EntryType>(new ClassType(), EntryType.ClassType);
            Match(Tokens.IdT);
            //Globals.Depth++;
            Match(Tokens.LBraceT);
            Match(Tokens.PublicT);
            Match(Tokens.StaticT);
            Match(Tokens.VoidT);
            //_symTab.Insert("Main", Tokens.IdT, Globals.Depth, EntryType.FunctionType);
            Match(Tokens.MainT);
            //Globals.Depth++;
            Match(Tokens.LParenT);
            Match(Tokens.StringT);
            Match(Tokens.LBrackT);
            Match(Tokens.RBrackT);
           // _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
            Match(Tokens.IdT);
            Match(Tokens.RParenT);
            //Globals.Depth--;
            Match(Tokens.LBraceT);
            SeqOfStatements();
            Match(Tokens.RBraceT);
            Match(Tokens.RBraceT);
            //Globals.Depth--;
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
            
            Globals.ClassT = new ClassType();
            Match(Tokens.ClassT);
            var lex = Globals.Lexeme;
            _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.ClassType);
            Match(Tokens.IdT);
            Globals.Depth++;
            if (Globals.Token == Tokens.ExtendsT)
            {
                Match(Tokens.ExtendsT);
                //_symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, );
                Match(Tokens.IdT);
            }

            Match(Tokens.LBraceT);
            VarDecl();
            MethodDecl();
            Match(Tokens.RBraceT);
            _symTab.WriteTable(Globals.Depth);
            _symTab.DeleteDepth(Globals.Depth);
            Globals.Depth--;
            Globals.Offset = 0;
            var temp = _symTab.Lookup(lex);
            temp.TypeOfEntry = new Union<EntryType>(Globals.ClassT, EntryType.ClassType);
        }

        private void VarDecl()
        {
            //if at depth 2 need to add to function type
            Globals.VarT = new VarType();
            if (Globals.Token == Tokens.FinalT)
            {
                Match(Tokens.FinalT);
                Type();
                Globals.VarT.Size += (int) Globals.TypeOfVar;
                Globals.VarT.TypeOfVariable = Globals.TypeOfVar;
                Globals.VarT.Offset = Globals.Offset;
                Globals.Offset += (int) Globals.TypeOfVar;
                Globals.ClassT.VariableNames.AddLast(Globals.Lexeme);
                Globals.ClassT.Size += (int)Globals.TypeOfVar;
                if (Globals.Depth == 2)
                {
                    Globals.FuncT.TotalSize += (int) Globals.TypeOfVar;
                    Globals.FuncT.SizeOfLocal += (int) Globals.TypeOfVar;
                }

                var lex = Globals.Lexeme;
                _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);
                Match(Tokens.NumT);
                Match(Tokens.SemiT);
                var temp = _symTab.Lookup(lex);
                temp.TypeOfEntry = new Union<EntryType>(Globals.VarT, EntryType.VarType);
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
            //if at depth 2 need to add to function type
            Globals.VarT = new VarType();
            if (Globals.Token == Tokens.IdT)
            {
                Globals.ClassT.Size += (int)Globals.TypeOfVar;
                Globals.ClassT.VariableNames.AddLast(Globals.Lexeme);
                Globals.VarT.Size += (int) Globals.TypeOfVar;
                Globals.VarT.Offset = Globals.Offset;
                Globals.VarT.TypeOfVariable = Globals.TypeOfVar;
                Globals.Offset += (int) Globals.TypeOfVar;
                if (Globals.Depth == 2)
                {
                    Globals.FuncT.TotalSize += (int) Globals.TypeOfVar;
                    Globals.FuncT.SizeOfLocal += (int) Globals.TypeOfVar;
                }

                var lex = Globals.Lexeme;
                _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
                Match(Tokens.IdT);
                
                var temp = _symTab.Lookup(lex);
                temp.TypeOfEntry = new Union<EntryType>(Globals.VarT, EntryType.VarType);
                IdentifierList();
            }
            else if (Globals.Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                Globals.ClassT.Size += (int)Globals.TypeOfVar;
                Globals.ClassT.VariableNames.AddLast(Globals.Lexeme);
                Globals.VarT.Size += (int) Globals.TypeOfVar;
                Globals.VarT.Offset = Globals.Offset;
                Globals.VarT.TypeOfVariable = Globals.TypeOfVar;
                Globals.Offset += (int) Globals.TypeOfVar;
                if (Globals.Depth == 2)
                {
                    Globals.FuncT.TotalSize += (int) Globals.TypeOfVar;
                    Globals.FuncT.SizeOfLocal += (int) Globals.TypeOfVar;
                }

                var lex = Globals.Lexeme;
                _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
                Match(Tokens.IdT);
                var temp = _symTab.Lookup(lex);
                temp.TypeOfEntry = new Union<EntryType>(Globals.VarT, EntryType.VarType);
                IdentifierList();
            }
        }

        private void Type()
        {
            if (Globals.Token == Tokens.IntT)
            {
                Match(Tokens.IntT);
                Globals.TypeOfVar = TypeOfVariable.IntType;
            }
            else if (Globals.Token == Tokens.BooleanT)
            {
                Match(Tokens.BooleanT);
                Globals.TypeOfVar = TypeOfVariable.BoolType;
            }
            else if (Globals.Token == Tokens.VoidT)
            {
                Match(Tokens.VoidT);
                Globals.TypeOfVar = TypeOfVariable.VoidType;
            }
            else
                return;
        }

        private void MethodDecl()
        {
            if (Globals.Token != Tokens.PublicT)
                return;

            Globals.FuncT = new FunctionType();
            Match(Tokens.PublicT);
            Type();
            var lex = Globals.Lexeme;
            Globals.ClassT.MethodNames.AddLast(Globals.Lexeme);
            _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.FunctionType);
            Match(Tokens.IdT);
            Globals.Depth++;
            Match(Tokens.LParenT);
            FormalList();//add params
            Match(Tokens.RParenT);
            Match(Tokens.LBraceT);
            VarDecl();
            SeqOfStatements();
            Match(Tokens.ReturnT);
            Expr();
            Match(Tokens.SemiT);
            Match(Tokens.RBraceT);
            Globals.Offset = 0;
            _symTab.WriteTable(Globals.Depth);
            _symTab.DeleteDepth(Globals.Depth);
            Globals.Depth--;
            Globals.Offset = 0;
            var temp = _symTab.Lookup(lex);
            temp.TypeOfEntry = new Union<EntryType>(Globals.FuncT, EntryType.FunctionType);
            MethodDecl();
        }

        private void FormalList()
        {
            if (!Globals.Token.IsDataType())
                return;
            Globals.VarT = new VarType();
            var paramNode = new ParameterNode();
            Type();
            paramNode.TypeOfParameter = Globals.TypeOfVar;
            //paramNode.PassMode
            Globals.VarT.Size += (int) Globals.TypeOfVar;
            Globals.VarT.Offset = Globals.Offset;
            Globals.VarT.TypeOfVariable = Globals.TypeOfVar;
            Globals.Offset += (int) Globals.TypeOfVar;
            Globals.FuncT.TotalSize += (int) Globals.TypeOfVar;
            Globals.FuncT.ParamList.AddLast(paramNode);
            var lex = Globals.Lexeme;
            _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
            Match(Tokens.IdT);
            var temp = _symTab.Lookup(lex);
            temp.TypeOfEntry = new Union<EntryType>(Globals.VarT, EntryType.VarType);
            FormalRest();
        }

        private void FormalRest()
        {
            if (Globals.Token != Tokens.CommaT)
                return;
            
            Globals.VarT = new VarType();
            var paramNode = new ParameterNode();
            Match(Tokens.CommaT);
            Type();
            paramNode.TypeOfParameter = Globals.TypeOfVar;
            //paramNode.PassMode
            Globals.VarT.Size += (int) Globals.TypeOfVar;
            Globals.VarT.Offset = Globals.Offset;
            Globals.VarT.TypeOfVariable = Globals.TypeOfVar;
            Globals.Offset += (int) Globals.TypeOfVar;
            Globals.FuncT.TotalSize += (int) Globals.TypeOfVar;
            Globals.FuncT.ParamList.AddLast(paramNode);
            var lex = Globals.Lexeme;
            _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
            Match(Tokens.IdT);
            var temp = _symTab.Lookup(lex);
            temp.TypeOfEntry = new Union<EntryType>(Globals.VarT, EntryType.VarType);
            FormalRest();
        }

        private void SeqOfStatements()
        {
            if (Globals.Token != Tokens.IdT) // will need to add more here when IOStat gets defined
                return;
            
            Statement();
            Match(Tokens.SemiT);
            SeqOfStatements();
        }

        private void Statement()
        {
            AssignStat();
            IOStat();
        }

        private void AssignStat()
        {
            if (Globals.Token != Tokens.IdT)
                return;

            if (_symTab.Lookup(Globals.Lexeme) == null && _symTab.Lookup(Globals.Lexeme, (Globals.Depth - 1)) == null)
            {
                ConsoleLogger.UndeclaredVariable(Globals.Lexeme, Globals.LineNo);
                throw new ParseErrorException();
            }
            
            Match(Tokens.IdT);
            Match(Tokens.AssignOpT);
            Expr();
        }

        private void IOStat()
        {
        }

        private void Expr()
        {
            Relation();
        }

        private void Relation()
        {
            SimpleExpr();
        }

        private void SimpleExpr()
        {
            Term();
            MoreTerm();
        }

        private void MoreTerm()
        {
            if (Globals.Token != Tokens.AddOpT)
                return;
            Match(Tokens.AddOpT);
            Term();
            MoreTerm();
        }

        private void Term()
        {
            Factor();
            MoreFactor();
        }

        private void Factor()
        {
            if (Globals.Token == Tokens.IdT)
            {
                if (_symTab.Lookup(Globals.Lexeme) == null && _symTab.Lookup(Globals.Lexeme, (Globals.Depth - 1)) == null)
                {
                    ConsoleLogger.UndeclaredVariable(Globals.Lexeme, Globals.LineNo);
                    throw new ParseErrorException();
                }
                
                Match(Tokens.IdT);
            }

            if (Globals.Token == Tokens.NumT)
                Match(Tokens.NumT);
            
            if (Globals.Token == Tokens.LParenT)
            {
                Match(Tokens.LParenT);
                Expr();
                Match(Tokens.RParenT);
            }

            if (Globals.Token == Tokens.NotT)
            {
                Match(Tokens.NotT);
                Factor();
            }

            if (Globals.Token == Tokens.SignOpT)
            {
                Match(Tokens.SignOpT);
                Factor();
            }

            if (Globals.Token == Tokens.TrueT)
                Match(Tokens.TrueT);
            
            if (Globals.Token == Tokens.FalseT)
                Match(Tokens.FalseT);
        }

        private void MoreFactor()
        {
            if (Globals.Token != Tokens.MulOpT)
                return;
            Match(Tokens.MulOpT);
            Factor();
            MoreFactor();
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