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
            //store reswords
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
            //Lexemes[1] = Globals.Ch;
            GetNextCh();
            switch (Globals.Lexeme[1])
            {
                case char a when (a >= 'A' && a <= 'Z') || (a >= 'a' && a <= 'z') || (a == '_'):
                    ProcessWordToken();
                    break;
                case char b when b >= '0' && b <= '9':
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
