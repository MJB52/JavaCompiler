using System.Collections.Generic;

namespace JavaCompiler.Entry_Types
{
    public class FunctionType
    {
        public int NumberOfParameters;
        public LinkedList<ParameterNode> ParamList; //linked list of paramter types
        public int SizeOfLocal;
        public int TotalSize;
        public TypeOfVariable ReturnType;

        public FunctionType()
        {
            ParamList = new LinkedList<ParameterNode>();
        }
    }
}