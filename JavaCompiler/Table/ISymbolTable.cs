namespace JavaCompiler
{
    public interface ISymbolTable
    {
        public void Insert(string lex, Tokens token, int depth);
        public bool Lookup();
        public void DeleteDepth(int depth);
        public void WriteTable(int depth);
        public void Hash(string lex);
    }
}