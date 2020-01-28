using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading.Tasks;

namespace JavaCompiler
{
    public class Scanner
    {
        private MemoryMappedFile _mappedFile;
        private StreamReader _streamReader;

        public Scanner(string fileName)
        {
            //open input file
            _mappedFile =  MemoryMappedFile.CreateFromFile(fileName);
            //TODO: Store ResWords
        }

        public void GetNextToken()
        {
            using (_mappedFile)
            {
                using (Stream mmStream = _mappedFile.CreateViewStream())
                {
                    using (_streamReader = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                    {
                        while (!_streamReader.EndOfStream)
                        {
                            GetNextCh();
                            ProcessToken();
                        }
                        Globals.Token = "eof";
                    }
                }
            }
        }

        private void GetNextCh()
        {
            var ch = (char)_streamReader.Read();
            while (char.IsWhiteSpace(ch))
            {
                if(ch == '\n')
                    Globals.LineNo++;
                ch = (char)_streamReader.Read();
            }

            Globals.Ch = ch;
        }

        private void ProcessToken()
        {
            Globals.Lexeme = Globals.Ch.ToString();
            GetNextCh();
            switch (Globals.Lexeme[0])
            {
                case char a when char.IsLetter(char.ToLower(a)) || (a == '_'):
                    ProcessWordToken();
                    break;
                case char b when char.IsDigit(b):
                    ProcessNumToken();
                    break;
                case char c when c == '/':
                    ProcessComment();
                    break;
                case char d when d == '=' || d == '<' || d == '>':
                    if (Globals.Ch == '=')
                        ProcessDoubleToken();
                    else
                        ProcessSingleToken();
                    break;
                default:
                    break;
            }
        }

        private void ProcessWordToken()
        {
            //Fill Lexeme
            throw new NotImplementedException();
        }

        private void ProcessNumToken()
        {
            throw new NotImplementedException();
        }

        private void ProcessComment()
        {

            throw new NotImplementedException();
        }

        private void ProcessDoubleToken()
        {

            throw new NotImplementedException();
        }

        private void ProcessSingleToken()
        {

            throw new NotImplementedException();
        }
    }
}
