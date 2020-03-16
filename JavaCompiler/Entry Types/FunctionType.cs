namespace JavaCompiler.Entry_Types
{
    public class FunctionType
    {
        public int NumberOfParameters;
        public ParameterNode ParamList; //linked list of paramter types

        //this is the entry for a function
        public int SizeOfLocal;
        public int TotalSize;
    }
}