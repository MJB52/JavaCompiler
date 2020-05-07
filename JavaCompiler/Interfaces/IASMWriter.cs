using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public interface IASMWriter
    {
        public string Filename { get; set; }
        public string MainProc { get; set; }
        public int LiteralCount { get; set; }
        public void AddLiteral(string lit);
        public void AddFunction(TableNode node);
    }
}
