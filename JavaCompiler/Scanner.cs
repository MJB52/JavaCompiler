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
            //open input file
            _mappedFile = MemoryMappedFile.CreateFromFile(fileName);
            //TODO: Store ResWords
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
            Globals.FileTokens.Add(KeyValuePair.Create(Tokens.EofT, Globals.Lexeme));
        }

        private void GetNextCh()
        {
            var ch = (char)_streamReader.Read();
            while (char.IsWhiteSpace(ch))
            {
                if (ch == '\n')
                    Globals.LineNo++;
                ch = (char)_streamReader.Read();
            }

            Globals.Ch = ch;
        }

        private void ProcessToken()
        {
            Globals.Lexeme = Globals.Ch.ToString();
            GetNextCh();
            switch (Globals.Lexeme[0])
            {
                case char a when char.IsLetter(char.ToLower(a)) || (a == '_'):
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
                    ConsoleLogger.LogUnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
        }

        private void ProcessWordToken()
        {
            if (!char.IsWhiteSpace(Globals.Ch))
            {
                Globals.Lexeme += Globals.Ch;
                var ch = (char)_streamReader.Read();
                while (!char.IsWhiteSpace(ch))
                {
                    Globals.Lexeme += ch;
                    ch = (char)_streamReader.Read();
                }
            }

            switch (Globals.Lexeme)
            {
                case "class":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ClassT, Globals.Lexeme));
                    break;
                case "public":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.PublicT, Globals.Lexeme));
                    break;
                case "static":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.StaticT, Globals.Lexeme));
                    break;
                case "void":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.VoidT, Globals.Lexeme));
                    break;
                case "main":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.MainT, Globals.Lexeme));
                    break;
                case "String":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.StringT, Globals.Lexeme));
                    break;
                case "extends":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ExtendsT, Globals.Lexeme));
                    break;
                case "return":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ReturnT, Globals.Lexeme));
                    break;
                case "int":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.IntT, Globals.Lexeme));
                    break;
                case "boolean":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.BooleanT, Globals.Lexeme));
                    break;
                case "if":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.IfT, Globals.Lexeme));
                    break;
                case "else":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ElseT, Globals.Lexeme));
                    break;
                case "while":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.WhileT, Globals.Lexeme));
                    break;
                case "System.out.println":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.PrintT, Globals.Lexeme));
                    break;
                case "length":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LengthT, Globals.Lexeme));
                    break;
                case "true":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.TrueT, Globals.Lexeme));
                    break;
                case "false":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.FalseT, Globals.Lexeme));
                    break;
                case "this":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.ThisT, Globals.Lexeme));
                    break;
                case "new":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.NewT, Globals.Lexeme));
                    break;
                default:
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.IdT, Globals.Lexeme));
                    break;
            }

            Globals.Lexeme = string.Empty;
        }

        private void ProcessNumToken()
        {
            if (!char.IsWhiteSpace(Globals.Ch))
            {
                Globals.Lexeme += Globals.Ch;
                var ch = (char)_streamReader.Read();
                while (char.IsDigit(ch) || ch == '.')
                {
                    Globals.Lexeme += ch;
                    ch = (char)_streamReader.Read();
                }
            }

            Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LiteralT, Globals.Lexeme));
            Globals.Lexeme = string.Empty;
        }

        private void ProcessComment()
        {

            if (Globals.Ch == '*')
            {
                var ch = (char)_streamReader.Read();
                while (ch != '*' && (char)_streamReader.Peek() != '/')
                {
                    ch = (char)_streamReader.Read();
                }
            }
            else
            {
                var ch = (char)_streamReader.Read();
                while (ch != '\n')
                {
                    ch = (char)_streamReader.Read();
                }
            }
        }

        private void ProcessDoubleToken()
        {
            Globals.Lexeme += Globals.Ch;
            switch (Globals.Lexeme)
            {
                case "<=":
                case ">=":
                case "==":
                case "!=":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RelOpT, Globals.Lexeme));
                    break;
                case "&&":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.MulOpT, Globals.Lexeme));
                    break;
                case "||":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.AddOpT, Globals.Lexeme));
                    break;
                default:
                    ConsoleLogger.LogUnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
            Globals.Lexeme = string.Empty;
        }

        private void ProcessSingleToken()
        {
            switch (Globals.Lexeme)
            {
                case "<":
                case ">":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RelOpT, Globals.Lexeme));
                    break;
                case "=":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.AssignOpT, Globals.Lexeme));
                    break;
                case ".":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.PeriodT, Globals.Lexeme));
                    break;
                case ",":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.CommaT, Globals.Lexeme));
                    break;
                case ";":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.SemiT, Globals.Lexeme));
                    break;
                case "\"":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.EofT, Globals.Lexeme));
                    break;
                case "{":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LBraceT, Globals.Lexeme));
                    break;
                case "}":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RBraceT, Globals.Lexeme));
                    break;
                case "(":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LParenT, Globals.Lexeme));
                    break;
                case ")":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RParenT, Globals.Lexeme));
                    break;
                case "[":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.LBrackT, Globals.Lexeme));
                    break;
                case "]":
                    Globals.FileTokens.Add(KeyValuePair.Create(Tokens.RBrackT, Globals.Lexeme));
                    break;
                default:
                    ConsoleLogger.LogUnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
            Globals.Lexeme = string.Empty;
        }
    }
}
