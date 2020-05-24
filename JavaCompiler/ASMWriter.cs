using System;
using System.Collections.Generic;
using System.IO;

namespace JavaCompiler
{
    internal class ASMWriter : IASMWriter
    {
        private readonly List<string> _asmData = new List<string>();
        private readonly List<string> _literals = new List<string>();
        private readonly ITACReader _tacReader;

        public LinkedList<TableNode> Functions = new LinkedList<TableNode>();

        public ASMWriter(string filename, ITACReader reader)
        {
            if (filename == "") ConsoleLogger.NoFilePassed();

            Filename = filename;
            _tacReader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public string Filename { get; set; }
        public string MainProc { get; set; } = string.Empty;
        public int LiteralCount { get; set; }

        public void AddLiteral(string literal)
        {
            _literals.Add(literal);
            LiteralCount++;
        }

        public void AddFunction(TableNode node)
        {
            Functions.AddLast(node);
        }

        public void StartPrint()
        {
            if (!_tacReader.Open())
                return;
            Globals.Ch = ' ';
            PrintBeginningFile();
            PrintDefaultProc();
            PrintProcs();

            _asmData.Add("END start");
            File.WriteAllLines(Filename, _asmData);
        }

        private void PrintBeginningFile()
        {
            _asmData.Add("     .model small");
            _asmData.Add("     .586");
            _asmData.Add("     .stack 100h");
            _asmData.Add("     .data");

            for (var i = 0; i < _literals.Count; i++)
                _asmData.Add(string.Format("S{0, -4} DB    \"{1, 0}\", \"$\"", i, _literals[i]));

            _asmData.Add("     .code");
            _asmData.Add("     include io.asm");
        }

        private void PrintDefaultProc()
        {
            _asmData.Add("");
            _asmData.Add("start PROC");
            _asmData.Add("     mov ax, @data");
            _asmData.Add("     mov ds, ax");
            _asmData.Add($"     call {MainProc}");
            _asmData.Add("     mov ah, 04ch");
            _asmData.Add("     int 21h");
            _asmData.Add("start ENDP");
        }

        private void PrintProcs()
        {
            var enumerator = Functions.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var node = enumerator.Current;
                var function = node.TypeOfEntry.As<FunctionType>();
                _asmData.Add("");
                _asmData.Add($"{node.Lexeme.Value} PROC");
                _asmData.Add("     push bp");
                _asmData.Add("     mov bp, sp");
                _asmData.Add($"     sub sp, {function.SizeOfLocal}");

                PrintTac();

                _asmData.Add($"     add sp, {function.SizeOfLocal}");
                _asmData.Add("     pop bp");
                _asmData.Add($"     ret {function.ParamaterOffsetSize - 4}");
                _asmData.Add($"{node.Lexeme.Value} ENDP");
            }
        }

        private void PrintTac()
        {
            var tacWord = GetWord();

            while (tacWord != "endp")
            {
                var keywords = new List<string> {"proc", "endp", "wrs", "wri", "wrln", "rdi", "call", "push"};
                if (keywords.Contains(tacWord))
                    GenerateTac(tacWord);
                else
                    BuildVarLine(tacWord);

                tacWord = GetWord();
            }

            _tacReader.GetNextWord();
        }

        private void GenerateTac(string tacWord)
        {
            if (tacWord == "proc")
            {
                _tacReader.GetNextWord();
            }
            else if (tacWord == "wrs")
            {
                _asmData.Add($"     mov dx, offset {_tacReader.GetNextWord()}");
                _asmData.Add("     call writestr");
            }
            else if (tacWord == "wri")
            {
                var bpWord = GetWord();
                _asmData.Add($"     mov dx, {bpWord}");
                _asmData.Add("     call writeint");
            }
            else if (tacWord == "wrln")
            {
                _asmData.Add("     call writeln");
            }
            else if (tacWord == "rdi")
            {
                var bpWord = GetWord();
                _asmData.Add("     call readint");
                _asmData.Add($"     mov {bpWord}, bx");
            }
            else if (tacWord == "call")
            {
                _asmData.Add($"     call {_tacReader.GetNextWord()}");
            }
            else if (tacWord == "push")
            {
                _asmData.Add($"     mov ax, {GetWord()}");
                _asmData.Add("     push ax");
            }
        }

        private void BuildVarLine(string tacWord)
        {
            if (tacWord == "_ax")
            {
                _tacReader.GetNextWord();
                _asmData.Add($"     mov ax, {GetWord()}");
            }
            else
            {
                _tacReader.GetNextWord();
                var reg = GetWord();
                var operation = _tacReader.PeekNextChar();

                if (operation == '*' || operation == '/' || operation == '-' || operation == '+')
                    PrintOperation(operation, reg, tacWord);
                else
                    PrintAssign(reg, tacWord);
            }
        }

        private void PrintOperation(char operation, string reg, string tacWord)
        {
            _tacReader.GetNextWord();

            if (operation == '+')
            {
                _asmData.Add($"     mov ax, {reg}");
                _asmData.Add($"     add ax, {GetWord()}");
                _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (operation == '-')
            {
                _asmData.Add($"     mov ax, {reg}");
                _asmData.Add($"     sub ax, {GetWord()}");
                _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (operation == '/')
            {
                _asmData.Add($"     mov ax, {reg}");
                _asmData.Add("     cwd");
                _asmData.Add($"     mov bx, {GetWord()}");
                _asmData.Add("     idiv bx");
                _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (operation == '*')
            {
                _asmData.Add($"     mov ax, {reg}");
                _asmData.Add($"     mov bx, {GetWord()}");
                _asmData.Add("     imul bx");
                _asmData.Add($"     mov {tacWord}, ax");
            }
        }

        private void PrintAssign(string reg, string tacWord)
        {
            if (reg.Contains("ax"))
            {
                _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (tacWord.Contains("ax"))
            {
                _asmData.Add($"     mov ax, {reg}");
            }
            else
            {
                _asmData.Add($"     mov ax, {reg}");
                _asmData.Add($"     mov {tacWord}, ax");

                if (reg.Contains("bp") || int.TryParse(reg, out _))
                    return;
                tacWord = GetWord();
                _tacReader.GetNextWord();
                _asmData.Add($"     mov ax, {GetWord()}");
                _asmData.Add($"     mov {tacWord}, ax");
            }
        }

        private string GetWord()
        {
            var word = _tacReader.GetNextWord();

            if (word[0] == '_')
            {
                word = word.Replace('_', '[');
                word += ']';
            }

            return word;
        }
    }
}