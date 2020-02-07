using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public class Scanner
    {
        private readonly MemoryMappedFile _mappedFile;
        private ILexeme _lexeme;
        private StreamReader _streamReader;

        public Scanner(string fileName)
        {
            _mappedFile = MemoryMappedFile.CreateFromFile(fileName);
        }

        public void GetNextToken()
        {
            using (_mappedFile)
            {
                using Stream mmStream = _mappedFile.CreateViewStream();
                using (_streamReader = new StreamReader(mmStream, Encoding.ASCII))
                {
                    do
                    {
                        GetNextCh();
                        if (Globals.Ch == '\uffff')
                            break;
                        ProcessToken();
                    } while (!_streamReader.EndOfStream);
                }
            }

            _lexeme = new Lexeme("eof");
            Globals.Print(KeyValuePair.Create(Tokens.EofT, _lexeme));
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
                case { } c when c == '/' && ch == '*' || ch == '/':
                    ProcessComment();
                    break;
                case { } d when new Regex(@"[-+/*|&;,.{}()\[\]!=><]").IsMatch(d.ToString()) || d == '"':
                    if (ch == '=' || ch == '|' || ch == '&')
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
            _lexeme = new Lexeme();
            var letterCount = 1;

            var ch = (char) _streamReader.Peek();
            while (ch.IsLegalWordToken())
            {
                ch = (char) _streamReader.Read();
                Globals.Lexeme += ch;
                letterCount++;
                ch = (char) _streamReader.Peek();
            }

            if (letterCount <= 31 || Globals.IsLiteral)
                switch (Globals.Lexeme)
                {
                    case "class":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.ClassT, _lexeme));
                        break;
                    case "public":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.PublicT, _lexeme));
                        break;
                    case "static":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.StaticT, _lexeme));
                        break;
                    case "void":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.VoidT, _lexeme));
                        break;
                    case "main":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.MainT, _lexeme));
                        break;
                    case "String":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.StringT, _lexeme));
                        break;
                    case "extends":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.ExtendsT, _lexeme));
                        break;
                    case "return":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.ReturnT, _lexeme));
                        break;
                    case "int":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.IntT, _lexeme));
                        break;
                    case "boolean":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.BooleanT, _lexeme));
                        break;
                    case "if":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.IfT, _lexeme));
                        break;
                    case "else":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.ElseT, _lexeme));
                        break;
                    case "while":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.WhileT, _lexeme));
                        break;
                    case "System.out.println":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.PrintT, _lexeme));
                        break;
                    case "length":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.LengthT, _lexeme));
                        break;
                    case "true":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.TrueT, _lexeme));
                        break;
                    case "false":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.FalseT, _lexeme));
                        break;
                    case "this":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.ThisT, _lexeme));
                        break;
                    case "new":
                        _lexeme.Value = Globals.Lexeme;
                        Globals.Print(KeyValuePair.Create(Tokens.NewT, _lexeme));
                        break;
                    default:
                        _lexeme.Value = Globals.Lexeme;
                        if (Globals.IsLiteral)
                        {
                            _lexeme.Type = ValueType.Literal;
                            Globals.Print(KeyValuePair.Create(Tokens.LiteralT, _lexeme));
                            Globals.IsLiteral = false;
                        }
                        else
                        {
                            Globals.Print(KeyValuePair.Create(Tokens.IdT, _lexeme));
                        }

                        break;
                }
            else
                ConsoleLogger.IllegalLexeme(Globals.Lexeme, Globals.LineNo);

            Globals.Lexeme = string.Empty;
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
            {
                _lexeme = new Lexeme(Globals.Lexeme, Globals.Lexeme.Contains('.') ? ValueType.ValueR : ValueType.Value);
                Globals.Print(KeyValuePair.Create(Tokens.NumT, _lexeme));
            }
            else
            {
                ConsoleLogger.IllegalLexeme(Globals.Lexeme, Globals.LineNo);
            }

            Globals.Lexeme = string.Empty;
        }

        private void ProcessComment()
        {
            var ch = (char) _streamReader.Read();
            if (ch == '*')
            {
                ch = (char) _streamReader.Peek();
                while (ch != '*' && (char) _streamReader.Peek() != '/' && !_streamReader.EndOfStream)
                {
                    ch = (char) _streamReader.Read();
                    if (ch == '\n')
                        Globals.LineNo++;
                }
            }
            else
            {
                ch = (char) _streamReader.Peek();
                while (!_streamReader.EndOfStream && ch != '\n') ch = (char) _streamReader.Read();
                Globals.LineNo++;
            }
        }

        private void ProcessDoubleToken()
        {
            _lexeme = new Lexeme();
            Globals.Lexeme += (char) _streamReader.Read();
            switch (Globals.Lexeme)
            {
                case "<=":
                case ">=":
                case "==":
                case "!=":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.RelOpT, _lexeme));
                    break;
                case "&&":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.MulOpT, _lexeme));
                    break;
                case "||":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.AddOpT, _lexeme));
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }

            Globals.Lexeme = string.Empty;
        }

        private void ProcessSingleToken()
        {
            _lexeme = new Lexeme();
            switch (Globals.Lexeme)
            {
                case "<":
                case ">":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.RelOpT, _lexeme));
                    break;
                case "=":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.AssignOpT, _lexeme));
                    break;
                case "+":
                case "-":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.AddOpT, _lexeme));
                    break;
                case "/":
                case "*":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.MulOpT, _lexeme));
                    break;
                case ".":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.PeriodT, _lexeme));
                    break;
                case ",":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.CommaT, _lexeme));
                    break;
                case ";":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.SemiT, _lexeme));
                    break;
                case "\"":
                    Globals.IsLiteral = true;
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.QuoteT, _lexeme));
                    break;
                case "{":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.LBraceT, _lexeme));
                    break;
                case "}":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.RBraceT, _lexeme));
                    break;
                case "(":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.LParenT, _lexeme));
                    break;
                case ")":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.RParenT, _lexeme));
                    break;
                case "[":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.LBrackT, _lexeme));
                    break;
                case "]":
                    _lexeme.Value = Globals.Lexeme;
                    Globals.Print(KeyValuePair.Create(Tokens.RBrackT, _lexeme));
                    break;
                default:
                    ConsoleLogger.UnknownLexeme(Globals.Lexeme, Globals.LineNo);
                    break;
            }

            Globals.Lexeme = string.Empty;
        }
    }
}