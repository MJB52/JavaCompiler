using System.Collections.Generic;

namespace JavaCompiler
{
    public class FunctionType
    {
        public int ParamaterOffsetSize = 0;
        public LinkedList<ParameterNode> ParamList; //linked list of paramter types
        public int SizeOfLocal = 0;
        public int TotalSize = 0;
        public TypeOfVariable ReturnType;

        public FunctionType()
        {
            ParamList = new LinkedList<ParameterNode>();
        }
    }
}