using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public class Scanner : IScanner
    {
        private readonly StreamReader _streamReader;

        public Scanner(string fileName)
        {
            var mappedFile = MemoryMappedFile.CreateFromFile(fileName);
            var mmStream = mappedFile.CreateViewStream();
            
            _streamReader = new StreamReader(mmStream, Encoding.ASCII);
        }

        public void GetNextToken()
        {
            Globals.Lexeme = string.Empty;
            if (_streamReader.EndOfStream)
            {
                Globals.Token = Tokens.EofT;
                return;
            }

            GetNextCh();
            if (Globals.Ch == '\uffff')
                GetNextToken();
            else
                ProcessToken();
        }

        ~Scanner()
        {
            _streamReader.Dispose();
        }

        private void GetNextCh()
        {
            char ch;
            do
            {
                ch = (char) _streamReader.Read();
                if (ch == '\n')
                    Globals.LineNo++;
            } while (char.IsWhiteSpace(ch) || ch == '\0');

            Globals.Ch = ch;
        }

        private void ProcessToken()
        {
            Globals.Lexeme = Globals.Ch.ToString();

            var ch = (char) _streamReader.Peek();
            switch (Globals.Lexeme[0])
            {
                case { } a when char.IsLetter(a):
                    ProcessWordToken();
                    break;
                case { } b when char.IsDigit(b):
                    ProcessNumToken();
                    break;
                case { } c when c == '/' && ch == '*' || c == '/' && ch == '/':
                    ProcessComment();
                    break;
                case { } d when new Regex(@"[-+/*|&;,.{}()\[\]!=><]").IsMatch(d.ToString()) || d == '"':
                    if (ch == '=' || ch == '|' || ch == '&')
                        ProcessDoubleToken();
                    else
                        ProcessSingleToken();
                    break;
                case {} e when _streamReader.EndOfStream:
                    Globals.Lexeme = "eof";
                    Globals.Token = Tokens.EofT;
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
        }

        private void ProcessWordToken()
        {
            var letterCount = 1;
            if (!Globals.IsLiteral)
            {
                var ch = (char) _streamReader.Peek();
                while (ch.IsLegalWordToken())
                {
                    ch = (char) _streamReader.Read();
                    Globals.Lexeme += ch;
                    letterCount++;
                    ch = (char) _streamReader.Peek();
                }
            }
            else
            {
                var ch = (char) _streamReader.Peek();
                while (ch != '"' && ch != '\n' && !_streamReader.EndOfStream)
                {
                    ch = (char) _streamReader.Read();
                    Globals.Lexeme += ch;
                    ch = (char) _streamReader.Peek();
                }
            }

            if (letterCount <= 31 || Globals.IsLiteral && Globals.Lexeme.Last() == '"')
                switch (Globals.Lexeme)
                {
                    case "class":
                        Globals.Token = Tokens.ClassT;
                        break;
                    case "public":
                        Globals.Token = Tokens.PublicT;
                        break;
                    case "final":
                        Globals.Token = Tokens.FinalT;
                        break;
                    case "static":
                        Globals.Token = Tokens.StaticT;
                        break;
                    case "void":
                        Globals.Token = Tokens.VoidT;
                        break;
                    case "main":
                        Globals.Token = Tokens.MainT;
                        break;
                    case "String":
                        Globals.Token = Tokens.StringT;
                        break;
                    case "extends":
                        Globals.Token = Tokens.ExtendsT;
                        break;
                    case "return":
                        Globals.Token = Tokens.ReturnT;
                        break;
                    case "int":
                        Globals.Token = Tokens.IntT;
                        break;
                    case "boolean":
                        Globals.Token = Tokens.BooleanT;
                        break;
                    case "if":
                        Globals.Token = Tokens.IfT;
                        break;
                    case "else":
                        Globals.Token = Tokens.ElseT;
                        break;
                    case "while":
                        Globals.Token = Tokens.WhileT;
                        break;
                    case "System.out.println":
                        Globals.Token = Tokens.PrintT;
                        break;
                    case "length":
                        Globals.Token = Tokens.LengthT;
                        break;
                    case "true":
                        Globals.Token = Tokens.TrueT;
                        break;
                    case "false":
                        Globals.Token = Tokens.FalseT;
                        break;
                    case "this":
                        Globals.Token = Tokens.ThisT;
                        break;
                    case "new":
                        Globals.Token = Tokens.NewT;
                        break;
                    default:
                        if (Globals.IsLiteral)
                        {
                            Globals.Token = Tokens.LiteralT;
                            Globals.IsLiteral = false;
                        }
                        else
                        {
                            Globals.Token = Tokens.IdT;
                        }

                        break;
                }
            else
                ConsoleLogger.IllegalLexeme(Globals.Lexeme, Globals.LineNo);
        }

        private void ProcessNumToken()
        {
            var ch = (char) _streamReader.Peek();
            while (char.IsDigit(ch) || ch == '.')
            {
                ch = (char) _streamReader.Read();
                Globals.Lexeme += ch;
                ch = (char) _streamReader.Peek();
            }

            if (Globals.Lexeme.Last() != '.')
                Globals.Token = Tokens.NumT;
            else
                ConsoleLogger.IllegalLexeme(Globals.Lexeme, Globals.LineNo);
        }

        private void ProcessComment()
        {
            string innerComment;
            var ch = (char) _streamReader.Peek();
            if (ch == '*')
            {
                _streamReader.Read();
                do
                {
                    ch = (char) _streamReader.Read();
                    if (ch == '\n')
                        Globals.LineNo++;

                    if (ch == '*' && _streamReader.Peek() == '/')
                    {
                        _streamReader.Read();
                        break;
                    }
                } while (!_streamReader.EndOfStream);
            }
            else
            {
                _streamReader.ReadLine();
                Globals.LineNo++;
            }

            GetNextToken();
        }

        private void ProcessDoubleToken()
        {
            Globals.Lexeme += (char) _streamReader.Read();
            switch (Globals.Lexeme)
            {
                case "<=":
                case ">=":
                case "==":
                case "!=":
                    Globals.Token = Tokens.RelOpT;
                    break;
                case "&&":
                    Globals.Token = Tokens.MulOpT;
                    break;
                case "||":
                    Globals.Token = Tokens.AddOpT;
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
        }

        private void ProcessSingleToken()
        {
            switch (Globals.Lexeme)
            {
                case "<":
                case ">":
                    Globals.Token = Tokens.RelOpT;
                    break;
                case "=":
                    Globals.Token = Tokens.AssignOpT;
                    break;
                case "+":
                case "-":
                    Globals.Token = Tokens.AddOpT;
                    break;
                case "/":
                case "*":
                    Globals.Token = Tokens.MulOpT;
                    break;
                case ".":
                    Globals.Token = Tokens.PeriodT;
                    break;
                case ",":
                    Globals.Token = Tokens.CommaT;
                    break;
                case ";":
                    Globals.Token = Tokens.SemiT;
                    break;
                case "!":
                    Globals.Token = Tokens.NotT;
                    break;
                case "\"":
                    Globals.IsLiteral = true;
                    Globals.Token = Tokens.QuoteT;
                    break;
                case "{":
                    Globals.Token = Tokens.LBraceT;
                    break;
                case "}":
                    Globals.Token = Tokens.RBraceT;
                    break;
                case "(":
                    Globals.Token = Tokens.LParenT;
                    break;
                case ")":
                    Globals.Token = Tokens.RParenT;
                    break;
                case "[":
                    Globals.Token = Tokens.LBrackT;
                    break;
                case "]":
                    Globals.Token = Tokens.RBrackT;
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }
        }
    }
}