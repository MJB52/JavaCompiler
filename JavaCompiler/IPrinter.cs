using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public interface IPrinter
    {
        public void PrintProg();

        public void GenerateExpression(bool printLine = false); //in general, dont print line unless user calls print line

        public void PrintLine();

        public void FinishPrint();
    }
}
