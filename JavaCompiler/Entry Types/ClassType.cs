using System.Collections.Generic;

namespace JavaCompiler.Entry_Types
{
    public struct ClassType
    {
        public LinkedList<string> MethodNames;
        public int Size;
        public LinkedList<string> VariableNames;
    }
}