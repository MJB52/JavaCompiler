using JavaCompiler.Entry_Types;

namespace JavaCompiler
{
    public class TableNode
    {
        public int Depth;
        public Lexeme Lexeme;
        public Tokens Token;

        public Union<EntryType> TypeOfEntry;
    }
}