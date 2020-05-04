using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    class ASMWriter
    {
        public string Filename { get; set; }
        private List<string> _asmData = new List<string>();
        public LinkedList<TableNode> Functions = new LinkedList<TableNode>();

        public ASMWriter(string filename)
        {
            if (filename == "")
            {
                ConsoleLogger.NoFilePassed();
            }

            Filename = filename;
        }
        public void GenerateASMFile()
        {
            GenerateProgBeginningASM();
            GenerateStartProcASM();
            GenerateProcsASM();

            _asmData.Add("END start");
            AssemblyFile.CreateASMFile();
        }

        private void GenerateProgBeginningASM()
        {
            _asmData.Add("     .model small");
             _asmData.Add("     .586");
             _asmData.Add("     .stack 100h");
             _asmData.Add("     .data");

            AssemblyFile.AddLiteralsToASMFile();

             _asmData.Add("     .code");
             _asmData.Add("     include io.asm");
        }

        private void GenerateStartProcASM()
        {
             _asmData.Add("");
             _asmData.Add("start PROC");
             _asmData.Add("     mov ax, @data");
             _asmData.Add("     mov ds, ax");
             _asmData.Add($"     call {AssemblyFile.mainName}");
             _asmData.Add("     mov ah, 04ch");
             _asmData.Add("     int 21h");
             _asmData.Add("start ENDP");
        }

        private void GenerateProcsASM()
        {
            var enumerator = Functions.GetEnumerator()
            do
            {
                var node = enumerator.Current;
                var function = node.TypeOfEntry.As<FunctionType>();
                _asmData.Add("");
                _asmData.Add($"{node.Lexeme} PROC");
                _asmData.Add("     push bp");
                _asmData.Add("     mov bp, sp");
                _asmData.Add($"     sub sp, {function.SizeOfLocal}");

                GenerateProcBodyASM();

                _asmData.Add($"     add sp, {function.SizeOfLocal}");
                _asmData.Add("     pop bp");
                _asmData.Add($"     ret {function.ParamaterOffsetSize - 4}");
                _asmData.Add($"{node.Lexemel} ENDP");
            } while (enumerator.MoveNext());
        }

        private void GenerateProcBodyASM()
        {
            string tacWord = GetAndFormatWord();

            while (tacWord != "endp")
            {
                if (TACKeywords.Contains(tacWord))
                {
                    GenerateProcLineUsingKeywordASM(tacWord);
                }
                else
                {
                    GenerateProcLineUsingVariableASM(tacWord);
                }

                tacWord = GetAndFormatWord();
            }

            TACFile.GetNextWord();
        }

        private void GenerateProcLineUsingKeywordASM(string tacWord)
        {
            if (tacWord == "proc")
            {
                TACFile.GetNextWord();
            }
            else if (tacWord == "wrs")
            {
                 _asmData.Add($"     mov dx, offset {TACFile.GetNextWord()}");
                 _asmData.Add("     call writestr");
            }
            else if (tacWord == "wri")
            {
                string bpWord = GetAndFormatWord();
                 _asmData.Add($"     mov dx, {bpWord}");
                 _asmData.Add($"     call writeint");
            }
            else if (tacWord == "wrln")
            {
                 _asmData.Add($"     call writeln");
            }
            else if (tacWord == "rdi")
            {
                string bpWord = GetAndFormatWord();
                 _asmData.Add($"     call readint");
                 _asmData.Add($"     mov {bpWord}, bx");
            }
            else if (tacWord == "call")
            {
                 _asmData.Add($"     call {TACFile.GetNextWord()}");
            }
            else if (tacWord == "push")
            {
                 _asmData.Add($"     mov ax, {GetAndFormatWord()}");
                 _asmData.Add($"     push ax");
            }
        }

        private void GenerateProcLineUsingVariableASM(string tacWord)
        {
            if (tacWord == "_ax")
            {
                TACFile.GetNextWord();
                 _asmData.Add($"     mov ax, {GetAndFormatWord()}");
            }
            else
            {
                TACFile.GetNextWord(); // skip equal sign
                string axReg = GetAndFormatWord();
                char opChar = TACFile.PeekNextChar();

                if (opChar == '*' || opChar == '/' || opChar == '-' || opChar == '+')
                {
                    GenerateOperationLineASM(opChar, axReg, tacWord);
                }
                else
                {
                    GenerateAssignmentLineASM(axReg, tacWord);
                }
            }
        }

        private void GenerateOperationLineASM(char opChar, string axReg, string tacWord)
        {
            TACFile.GetNextWord(); // skip operator

            if (opChar == '+')
            {
                 _asmData.Add($"     mov ax, {axReg}");
                 _asmData.Add($"     add ax, {GetAndFormatWord()}");
                 _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (opChar == '-')
            {
                 _asmData.Add($"     mov ax, {axReg}");
                 _asmData.Add($"     sub ax, {GetAndFormatWord()}");
                 _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (opChar == '/')
            {
                 _asmData.Add($"     mov ax, {axReg}");
                 _asmData.Add($"     mov bx, {GetAndFormatWord()}");
                 _asmData.Add($"     idiv bx");
                 _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (opChar == '*')
            {
                 _asmData.Add($"     mov ax, {axReg}");
                 _asmData.Add($"     mov bx, {GetAndFormatWord()}");
                 _asmData.Add($"     imul bx");
                 _asmData.Add($"     mov {tacWord}, ax");
            }
        }

        private void GenerateAssignmentLineASM(string axReg, string tacWord)
        {
            if (axReg.Contains("ax"))
            {
                 _asmData.Add($"     mov {tacWord}, ax");
            }
            else if (tacWord.Contains("ax"))
            {
                 _asmData.Add($"     mov ax, {axReg}");
            }
            else
            {
                 _asmData.Add($"     mov ax, {axReg}");
                 _asmData.Add($"     mov {tacWord}, ax");

                if (!axReg.Contains("bp"))
                {
                    tacWord = GetAndFormatWord();
                    TACFile.GetNextWord(); // skip equal sign
                     _asmData.Add($"     mov ax, {GetAndFormatWord()}");
                     _asmData.Add($"     mov {tacWord}, ax");
                }
            }
        }

        private string GetAndFormatWord()
        {
            string word = TACFile.GetNextWord();

            if (word[0] == '_')
            {
                word = word.Replace('_', '[');
                word += ']';
            }

            return word;
        }
    }
}
