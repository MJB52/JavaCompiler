using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace JavaCompiler
{
    public class TACWriter : ITACWriter
    {
        public string FileName { get; private set; }
        public int Offset { get; set; } = 2;
        private List<string> _tacData = new List<string>();

        public TACWriter(string fileName = "")
        {
            if(fileName == "")
            {
                ConsoleLogger.NoFilePassed();
            }

            FileName = fileName;
            Console.WriteLine(FileName);
        }

        public void PrintLine(string line)
        {
            _tacData.Add(line);
            Console.WriteLine(line);
        }

        public void EOFEncountered()
        {
            File.WriteAllLines(FileName, _tacData);
            Console.WriteLine($"TAC file generated at {FileName}");
        }

        public string GenerateTempVar(TypeOfVariable type)
        {
            string name = $"_bp-{Globals.Offset + Offset}";
            Offset += (int)type;
            return name;
        }
    }
}
