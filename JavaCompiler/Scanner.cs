using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaCompiler
{
    public class Scanner
    {
        private MemoryMappedFile _mappedFile;
        private StreamReader _streamReader;

        public Scanner(string fileName)
        {
            _mappedFile = MemoryMappedFile.CreateFromFile(fileName);
        }

        public void GetNextToken()
        {
            using (_mappedFile)
            {
                using (Stream mmStream = _mappedFile.CreateViewStream())
                {
                    using (_streamReader = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                    {
                        while (!_streamReader.EndOfStream)
                        {
                            GetNextCh();
                            ProcessToken();
                        }
                    }
                }
            }
            ILexeme x = new Lexeme(Globals.Lexeme);
            Globals.FileTokens.Add(KeyValuePair.Create(Tokens.EofT, x));
        }

        private void GetNextCh()
        {
            char ch;
            do
            {
                ch = (char)_streamReader.Read();
                if (ch == '\n')
                    Globals.LineNo++;
            } while (char.IsWhiteSpace(ch) || ch == '\0');

            Globals.Ch = ch;
        }

        private void ProcessToken()
        {
            Globals.Lexeme = Globals.Ch.ToString();
            GetNextCh();

            switch (Globals.Lexeme[0])
            {
                case char a when char.IsLetter(a) || (a == '_'):
                    ProcessWordToken();
                    break;
                case char b when char.IsDigit(b):
                    ProcessNumToken();
                    break;
                case char c when c == '/' && Globals.Ch == '*' || Globals.Ch == '/':
                    ProcessComment();
                    break;
                case char d when new Regex(@"[-+/*;,.{}()\[\]!=><]").IsMatch(d.ToString()):
                    if (Globals.Ch == '=' || Globals.Ch == '|' || Globals.Ch == '&')
                        ProcessDoubleToken();
                    else
                        ProcessSingleToken();
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
        }

        private void ProcessWordToken()
        {
            ILexeme lexeme = new Lexeme();
            var letterCount = 1;
            if (Globals.Ch.IsLegalWordToken())
            {
                Globals.Lexeme += Globals.Ch;
                letterCount++;
            }

            var ch = (char)_streamReader.Peek();
            while (ch.IsLegalWordToken())
            {
                ch = (char)_streamReader.Read();
                Globals.Lexeme += ch;
                letterCount++;
                ch = (char)_streamReader.Peek();
            }

            if (letterCount <= 31)
            {
                switch (Globals.Lexeme)
                {
                    case "class":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ClassT, lexeme));
                        break;
                    case "public":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.PublicT, lexeme));
                        break;
                    case "static":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.StaticT, lexeme));
                        break;
                    case "void":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.VoidT, lexeme));
                        break;
                    case "main":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.MainT, lexeme));
                        break;
                    case "String":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.StringT, lexeme));
                        break;
                    case "extends":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ExtendsT, lexeme));
                        break;
                    case "return":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ReturnT, lexeme));
                        break;
                    case "int":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.IntT, lexeme));
                        break;
                    case "boolean":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.BooleanT, lexeme));
                        break;
                    case "if":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.IfT, lexeme));
                        break;
                    case "else":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ElseT, lexeme));
                        break;
                    case "while":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.WhileT, lexeme));
                        break;
                        lexeme.Value = Globals.Lexeme;
                    case "System.out.println":
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.PrintT, lexeme));
                        break;
                    case "length":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LengthT, lexeme));
                        break;
                    case "true":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.TrueT, lexeme));
                        break;
                    case "false":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.FalseT, lexeme));
                        break;
                    case "this":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ThisT, lexeme));
                        break;
                    case "new":
                        lexeme.Value = Globals.Lexeme;
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.NewT, lexeme));
                        break;
                    default:
                        Globals.FileTokens.Add(KeyValuePair.Create(Tokens.IdT, lexeme));
                        break;
                }
            }
            else
            {
                ConsoleLogger.IllegalLexeme(Globals.Lexeme, Globals.LineNo);
            }

            Globals.Lexeme = string.Empty;
        }

        private void ProcessNumToken()
        {
            if (char.IsDigit(Globals.Ch))
            {
                Globals.Lexeme += Globals.Ch;
            }

            var ch = (char)_streamReader.Peek();
            while (char.IsDigit(ch))
            {
                ch = (char)_streamReader.Read();
                Globals.Lexeme += ch;
                ch = (char)_streamReader.Peek();
            }
            ILexeme lexeme = new NumLexeme(Globals.Lexeme, Globals.Lexeme.Contains('.') ? ValueType.ValueR : ValueType.Value);
            Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LiteralT, lexeme));
            Globals.Lexeme = string.Empty;
        }

        private void ProcessComment()
        {

            if (Globals.Ch == '*')
            {
                var ch = (char)_streamReader.Peek();
                while (ch != '*' && (char)_streamReader.Peek() != '/')
                {
                    ch = (char)_streamReader.Read();
                }
            }
            else
            {
                var ch = (char)_streamReader.Peek();
                while (ch != '\n')
                {
                    ch = (char)_streamReader.Read();
                }
                Globals.LineNo++;
            }
        }

        private void ProcessDoubleToken()
        {
            ILexeme lexeme = new Lexeme();
            Globals.Lexeme += Globals.Ch;
            switch (Globals.Lexeme)
            {
                case "<=":
                case ">=":
                case "==":
                case "!=":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RelOpT, lexeme));
                    break;
                case "&&":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.MulOpT, lexeme));
                    break;
                case "||":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.AddOpT, lexeme));
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
            Globals.Lexeme = string.Empty;
        }

        private void ProcessSingleToken()
        {
            ILexeme lexeme = new Lexeme();
            switch (Globals.Lexeme)
            {
                case "<":
                case ">":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RelOpT, lexeme));
                    break;
                case "=":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.AssignOpT, lexeme));
                    break;
                case "+":
                case "-":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.AddOpT, lexeme));
                    break;
                case "/":
                case "*":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.MulOpT, lexeme));
                    break;
                case ".":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.PeriodT, lexeme));
                    break;
                case ",":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.CommaT, lexeme));
                    break;
                case ";":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.SemiT, lexeme));
                    break;
                case "\"":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.QuoteT, lexeme));
                    break;
                case "{":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LBraceT, lexeme));
                    break;
                case "}":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RBraceT, lexeme));
                    break;
                case "(":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LParenT, lexeme));
                    break;
                case ")":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RParenT, lexeme));
                    break;
                case "[":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LBrackT, lexeme));
                    break;
                case "]":
                    lexeme.Value = Globals.Lexeme;
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RBrackT, lexeme));
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
            Globals.Lexeme = string.Empty;
        }
    }
}
