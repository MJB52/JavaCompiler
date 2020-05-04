using System.Collections.Generic;

namespace JavaCompiler
{
    public class ClassType
    {
        public LinkedList<string> MethodNames;
        public int Size = 0;
        public LinkedList<string> VariableNames;

        public ClassType()
        {
            MethodNames = new LinkedList<string>();
            VariableNames = new LinkedList<string>();
        }
    }
}