namespace JavaCompiler
{
    public interface ISymbolTable
    {
        public void Insert();
        public bool Lookup();
        public void DeleteDepth(int depth);
        public void WriteTable(int depth);
    }
}