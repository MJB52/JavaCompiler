using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public interface ITACWriter
    {
        public string FileName { get; }
        public int Offset { get; set; }

        public void PrintLine(string line);
        public string GenerateTempVar(TypeOfVariable type);
        public void EOFEncountered();
    }
}
