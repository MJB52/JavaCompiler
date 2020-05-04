using System;
using System.Data;
using JavaCompiler.Entry_Types;

namespace JavaCompiler
{
    public class Parser
    {
        private readonly IScanner _scanner;
        private readonly ISymbolTable _symTab;
        private readonly IPrinter _printer;

        public Parser(IScanner scanner, ISymbolTable symTab, IPrinter printer)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _symTab = symTab ?? throw new ArgumentNullException(nameof(symTab));
            _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public void Prog()
        {
            try
            {
                MoreClasses();
                MainClass();

                if (Globals.Token == Tokens.EofT)
                    _printer.EOFEncountered(); //ConsoleLogger.SuccessfulParse();
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
            _printer.PrintLine("proc main");
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
            _printer.PrintLine("endp main");

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
            _symTab.Lookup(lex).TypeOfEntry = new Union<EntryType>(Globals.ClassT, EntryType.ClassType);

            Match(Tokens.IdT);
            Globals.Depth++;
            Globals.Offset = 0;
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

            _symTab.DeleteDepth(Globals.Depth);
            Globals.Depth--;
            Globals.Offset = 0;
            var temp = _symTab.Lookup(lex) ?? throw new ParseErrorException();
            temp.TypeOfEntry = new Union<EntryType>(Globals.ClassT, EntryType.ClassType);
        }

        private void VarDecl()
        {
            //if at depth 2 need to add to function type
            if (Globals.Token == Tokens.FinalT)
            {
                Globals.ConstT = new ConstantType();
                Match(Tokens.FinalT);
                Type();
                Globals.ConstT.OffsetName = $"_bp-{Globals.Offset}";
                Globals.ConstT.Size += (int) Globals.TypeOfVar;
                Globals.ConstT.TypeOfConstant = Globals.TypeOfVar;
                Globals.ConstT.Offset = Globals.Offset;
                Globals.Offset += (int) Globals.TypeOfVar;

                if (Globals.Depth == 2) //variable at function level only
                {
                    Globals.FuncT.TotalSize += (int) Globals.TypeOfVar;
                    Globals.FuncT.SizeOfLocal += (int) Globals.TypeOfVar;
                }
                else
                {
                    Globals.ClassT.VariableNames.AddLast(Globals.Lexeme);
                    Globals.ClassT.Size += (int)Globals.TypeOfVar;
                }

                var lex = Globals.Lexeme;
                _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.ConstantType);
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);
                Match(Tokens.NumT);
                Match(Tokens.SemiT);
                var temp = _symTab.Lookup(lex) ?? throw new ParseErrorException();
                temp.TypeOfEntry = new Union<EntryType>(Globals.ConstT, EntryType.ConstantType);
                //should this be printed??
                VarDecl();
            }
            else if (Globals.Token.IsDataType())
            {
                Globals.VarT = new VarType();
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
                Globals.VarT.OffsetName = $"_bp-{Globals.Offset}";
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
                
                var temp = _symTab.Lookup(lex) ?? throw new ParseErrorException();
                temp.TypeOfEntry = new Union<EntryType>(Globals.VarT, EntryType.VarType);
                IdentifierList();
            }
            else if (Globals.Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                Globals.VarT.OffsetName = $"_bp-{Globals.Offset}";
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
            _printer.PrintLine($"proc {Globals.Lexeme}");

            Match(Tokens.IdT);
            Globals.Depth++;
            Match(Tokens.LParenT);
            Globals.FuncT.ParamaterOffsetSize = 4;
            FormalList();//add params
            Globals.Offset = 2;
            _printer.Offset = 2;
            Match(Tokens.RParenT);
            Match(Tokens.LBraceT);
            VarDecl();
            SeqOfStatements();
            Match(Tokens.ReturnT);
            Expr();
            Match(Tokens.SemiT);
            Match(Tokens.RBraceT);

            _symTab.DeleteDepth(Globals.Depth);
            Globals.Depth--;
            Globals.Offset = 0;
            var temp = _symTab.Lookup(lex) ?? throw new ParseErrorException();
            temp.TypeOfEntry = new Union<EntryType>(Globals.FuncT, EntryType.FunctionType);
            _printer.PrintLine($"endp {lex}");

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
            Globals.VarT.OffsetName = $"_bp+{Globals.FuncT.ParamaterOffsetSize}";
            Globals.VarT.Size += (int) Globals.TypeOfVar;
            Globals.VarT.Offset = Globals.Offset;
            Globals.VarT.TypeOfVariable = Globals.TypeOfVar;
            Globals.FuncT.ParamaterOffsetSize += (int)Globals.TypeOfVar;
            Globals.FuncT.TotalSize += (int) Globals.TypeOfVar;
            Globals.FuncT.ParamList.AddLast(paramNode);
            Globals.Offset += (int)Globals.TypeOfVar;

            var lex = Globals.Lexeme;
            _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
            Match(Tokens.IdT);
            var temp = _symTab.Lookup(lex) ?? throw new ParseErrorException();
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
            Globals.VarT.OffsetName = $"_bp+{Globals.FuncT.ParamaterOffsetSize}";
            Globals.VarT.Size += (int)Globals.TypeOfVar;
            Globals.VarT.Offset = Globals.Offset;
            Globals.VarT.TypeOfVariable = Globals.TypeOfVar;
            Globals.FuncT.ParamaterOffsetSize += (int)Globals.TypeOfVar;
            Globals.FuncT.TotalSize += (int)Globals.TypeOfVar;
            Globals.FuncT.ParamList.AddLast(paramNode);
            Globals.Offset += (int)Globals.TypeOfVar;

            var lex = Globals.Lexeme;
            _symTab.Insert(Globals.Lexeme, Globals.Token, Globals.Depth, EntryType.VarType);
            Match(Tokens.IdT);
            var temp = _symTab.Lookup(lex) ?? throw new ParseErrorException();
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
            var assign = _symTab.Lookup(Globals.Lexeme);

            if (assign != null && assign.TypeOfEntry.Tag == EntryType.ClassType)
            {
                Match(Tokens.IdT);
                Match(Tokens.PeriodT);
                var classVal = assign.TypeOfEntry.As<ClassType>();
                if (classVal.MethodNames.Contains(Globals.Lexeme))
                {
                    var methodName = Globals.Lexeme;
                    Match(Tokens.IdT);
                    Match(Tokens.LParenT);
                    ParamsList();
                    Match(Tokens.RParenT);
                    _printer.PrintLine($"call {methodName}");
                }
                else
                {
                    ConsoleLogger.UnknownFunction(assign.Lexeme.Value, Globals.Lexeme, Globals.LineNo);
                }
                // throw??
            }
            else if (assign != null && assign.TypeOfEntry.Tag == EntryType.VarType)
            {
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);

                Expr();
                string temp = string.Empty;
                switch (assign.TypeOfEntry.Tag)
                {
                    case EntryType.ConstantType:
                        temp = assign.TypeOfEntry.As<ConstantType>().OffsetName;
                        break;
                    case EntryType.VarType:
                        temp = assign.TypeOfEntry.As<VarType>().OffsetName;
                        break;
                    default:
                        //idk should never happen
                        break;
                }
                _printer.PrintLine($"{temp} = {Globals.TempOffsetName}");
            }
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
            string line;
            var temp = _printer.GenerateTempVar(TypeOfVariable.IntType);
            line = $"{temp} = {Globals.TempOffsetName} + ";
            Match(Tokens.AddOpT);
            Term();
            line += Globals.TempOffsetName;
            _printer.PrintLine(line);
            MoreTerm();
            Globals.TempOffsetName = temp;
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
                var temp = _symTab.Lookup(Globals.Lexeme);
                if (temp == null)
                {
                    ConsoleLogger.UndeclaredVariable(Globals.Lexeme, Globals.LineNo);
                    throw new ParseErrorException();
                }
                    switch (temp.TypeOfEntry.Tag)
                    {
                        case EntryType.ConstantType:
                            Globals.TempOffsetName = temp.TypeOfEntry.As<ConstantType>().OffsetName;
                            break;
                        case EntryType.VarType:
                            Globals.TempOffsetName = temp.TypeOfEntry.As<VarType>().OffsetName;
                            break;
                        default:
                            //idk should never happen
                            break;
                    }
                Match(Tokens.IdT);
            }

            if (Globals.Token == Tokens.NumT)
            {
                var temp = _printer.GenerateTempVar(TypeOfVariable.IntType);
                _printer.PrintLine(temp + " = " + Globals.Lexeme);
                Globals.TempOffsetName = temp;

                Match(Tokens.NumT);
            }
            
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
                Globals.TempOffsetName = $"!{Globals.TempOffsetName}";
            }

            if (Globals.Token == Tokens.SignOpT)
            {
                var temp = _printer.GenerateTempVar(TypeOfVariable.IntType);
                string line = $"{temp} = 0 - ";
                Match(Tokens.SignOpT);
                Factor();
                line += Globals.TempOffsetName;
                _printer.PrintLine(line);
                Globals.TempOffsetName = temp;
            }

            if (Globals.Token == Tokens.TrueT)
            {
                var temp = _printer.GenerateTempVar(TypeOfVariable.BoolType);
                _printer.PrintLine(temp + "=" + Globals.TempOffsetName);
                Globals.TempOffsetName = temp;
                Match(Tokens.TrueT);
            }
            if (Globals.Token == Tokens.FalseT)
            {
                var temp = _printer.GenerateTempVar(TypeOfVariable.BoolType);
                _printer.PrintLine(temp + "=" + Globals.TempOffsetName);
                Globals.TempOffsetName = temp;
                Match(Tokens.FalseT);
            }
        }

        private void MoreFactor()
        {
            if (Globals.Token != Tokens.MulOpT)
                return;
            string line;
            var temp = _printer.GenerateTempVar(TypeOfVariable.IntType);
            line = $"{temp} = {Globals.TempOffsetName} * ";
            Match(Tokens.MulOpT);
            Factor();
            line += Globals.TempOffsetName;
            _printer.PrintLine(line);
            MoreFactor();
            Globals.TempOffsetName = temp;
        }

        private void ParamsList()
        {
            if (Globals.Token == Tokens.IdT)
            {
                var param = _symTab.Lookup(Globals.Lexeme);

                if (param != null)
                {
                    switch (param.TypeOfEntry.Tag)
                    {
                        case EntryType.ConstantType:
                            _printer.PrintLine($"push {param.TypeOfEntry.As<ConstantType>().OffsetName}");
                            break;
                        case EntryType.VarType:
                            _printer.PrintLine($"push {param.TypeOfEntry.As<VarType>().OffsetName}");
                            break;
                        default:
                            //idk should never happen
                            break;
                    }
                    Match(Tokens.IdT);
                    ParamsRest();
                }
                else
                {
                    ConsoleLogger.UndeclaredVariable(Globals.Lexeme, Globals.LineNo);
                    throw new ParseErrorException();
                }
            }

            if (Globals.Token == Tokens.NumT || Globals.Token == Tokens.TrueT || Globals.Token == Tokens.FalseT)
            {
                _printer.PrintLine($"push {Globals.Lexeme}");

                if (Globals.Token == Tokens.NumT)
                {
                    Match(Tokens.NumT);
                }
                else if (Globals.Token == Tokens.TrueT)
                {
                    Match(Tokens.TrueT);
                }
                else
                {
                    Match(Tokens.FalseT);
                }

                ParamsRest();
            }
        }

        private void ParamsRest()
        {
            if (Globals.Token != Tokens.CommaT)
                return;

            Match(Tokens.CommaT);
            if (Globals.Token == Tokens.IdT)
            {
                var param = _symTab.Lookup(Globals.Lexeme);

                if (param != null)
                {
                    switch (param.TypeOfEntry.Tag)
                    {
                        case EntryType.ConstantType:
                            _printer.PrintLine($"push {param.TypeOfEntry.As<ConstantType>().OffsetName}");
                            break;
                        case EntryType.VarType:
                            _printer.PrintLine($"push {param.TypeOfEntry.As<VarType>().OffsetName}");
                            break;
                        default:
                            //idk should never happen
                            break;
                    }
                    Match(Tokens.IdT);
                    ParamsRest();
                }
                else
                {
                    ConsoleLogger.UndeclaredVariable(Globals.Lexeme, Globals.LineNo);
                    throw new ParseErrorException();
                }
            }

            if (Globals.Token == Tokens.NumT || Globals.Token == Tokens.TrueT || Globals.Token == Tokens.FalseT)
            {
                _printer.PrintLine($"push {Globals.Lexeme}");

                if (Globals.Token == Tokens.NumT)
                {
                    Match(Tokens.NumT);
                }
                else if (Globals.Token == Tokens.TrueT)
                {
                    Match(Tokens.TrueT);
                }
                else
                {
                    Match(Tokens.FalseT);
                }

                ParamsRest();
            }
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