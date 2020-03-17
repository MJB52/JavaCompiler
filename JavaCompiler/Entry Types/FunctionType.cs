namespace JavaCompiler.Entry_Types
{
    public struct FunctionType
    {
        public int NumberOfParameters;
        public ParameterNode ParamList; //linked list of paramter types
        public int SizeOfLocal;
        public int TotalSize;
    }
}