using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public interface ILexeme
    {
        public string Value { get; set; }
    }

    public class Lexeme : ILexeme
    {
        public Lexeme()
        {
        }

        public Lexeme(string str)
        {
            Value = str;
        }
        public string Value { get ; set; }
    }

    public enum ValueType { Value, ValueR }

    public class NumLexeme : Lexeme 
    {
        public ValueType Type { get; set; }

        public NumLexeme(string str, ValueType val) : base(str)
        {

        }
    }
}
