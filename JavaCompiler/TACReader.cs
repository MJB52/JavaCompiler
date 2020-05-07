using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace JavaCompiler
{
    public class TACReader : ITACReader
    {
        public string FileName { get; private set; }
        public string CurrentWord { get; private set; } = string.Empty;
        private StreamReader _streamReader;

        public TACReader(string fileName = "")
        {
            if (fileName == "")
            {
                ConsoleLogger.NoFilePassed();
            }

            FileName = fileName;
        }

        ~TACReader()
        {
            _streamReader.Dispose();
        }

        public bool Open()
        {
            try
            {
                var mappedFile = MemoryMappedFile.CreateFromFile(FileName);
                var mmStream = mappedFile.CreateViewStream();

                _streamReader = new StreamReader(mmStream, Encoding.ASCII);
                return true;
            }
            catch(FileNotFoundException ex)
            {
                ConsoleLogger.FileNotFound(FileName);
                return false;
            }
        }

        public string GetNextWord()
        {
            CurrentWord = string.Empty;
            while (char.IsWhiteSpace(Globals.Ch))
            {
                Globals.Ch = (char)_streamReader.Read();
            }

            while (!char.IsWhiteSpace(Globals.Ch))
            {
                CurrentWord += Globals.Ch;
                Globals.Ch = (char)_streamReader.Read();
            }

            return CurrentWord;
        }

        public char PeekNextChar()
        {
            while (char.IsWhiteSpace((char)_streamReader.Peek()))
            {
                Globals.Ch = (char)_streamReader.Read();
            }

            return (char)_streamReader.Peek();
        }
    }
}
