using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public class Printer : IPrinter
    {
        public string FileName { get; set; }

        public Printer(string fileName = "")
        {
            if(fileName == "")
            {
                ConsoleLogger.NoFilePassed();
            }

            FileName = fileName;
        }

        public void PrintProg()
        {
            
        }

        public void PrintLine()
        {

        }
        public void FinishPrint()
        {

        }

        public void GenerateExpression(bool printLine = false)
        {
            if (printLine)
                PrintLine(); //might have to do some logic before this 
        }
    }
}
