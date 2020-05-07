using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public interface ITACReader
    {
        public string FileName { get; }
        public string CurrentWord { get; }
        public bool Open();
        public string GetNextWord();
        public char PeekNextChar();
    }
}
