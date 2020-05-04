namespace JavaCompiler
{
    public interface ISymbolTable
    {
        public void Insert(string lex, Tokens token, int depth, EntryType type);
        public TableNode? Lookup(string lex);
        public void DeleteDepth(int depth);
        public void WriteTable(int depth);
        public uint Hash(string lex);
    }
}