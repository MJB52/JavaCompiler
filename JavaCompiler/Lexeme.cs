using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public enum ValueType { None, Value, ValueR }

    public interface ILexeme
    {
        public string Value { get; set; }
        public ValueType Type { get; set; }
    }

    public class Lexeme : ILexeme
    {
        public Lexeme()
        {
            Type = ValueType.None;
        }

        public Lexeme(string str, ValueType type = ValueType.None)
        {
            Value = str;
            Type = type;
        }

        public string Value { get ; set; }
        public ValueType Type { get; set; }
    }
}
