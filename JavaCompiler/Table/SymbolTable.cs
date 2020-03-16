using System.Collections.Generic;
using JavaCompiler.Entry_Types;

namespace JavaCompiler
{
    public class SymbolTable : ISymbolTable
    {
        private LinkedList<TableNode>[] theTree;

        public void Insert(string lex, Tokens token, int depth)
        {
            //create node
            //var temp = 
        }

        public bool Lookup() => false;

        public void DeleteDepth(int depth)
        {
        }

        public void WriteTable(int depth)
        {
        }

        public void Hash(string lex)
        {
        }

        private TableNode CreateConstantNode(string lex, Tokens token, int depth)
        {
            var type = new ConstantType
            {
                type = new Union<int, float>(5.5F), TypeOfConstant = TypeOfVariable.FloatType, Offset = 6
            };

            return new TableNode
            {
                TypeOfEntry = new Union<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }

        private TableNode CreateFunctionNode(string lex, Tokens token, int depth)
        {
            //idk get stuff

            var type = new FunctionType
            {
                ParamList =
                {
                    Next = new ParameterNode(), TypeOfParameter = TypeOfVariable.IntType,
                    PassMode = ParameterPassMode.Value
                },
                NumberOfParameters = 1
            };

            return new TableNode
            {
                TypeOfEntry = new Union<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }

        private TableNode CreateVarNode(string lex, Tokens token, int depth)
        {
            var type = new VarType
            {
                TypeOfVariable = TypeOfVariable.FloatType, Offset = 6
            };

            return new TableNode
            {
                TypeOfEntry = new Union<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }

        private TableNode CreateClassNode(string lex, Tokens token, int depth)
        {
            var type = new ClassType
            {
                Size = 4, VariableNames = new LinkedList<string>(), MethodNames = new LinkedList<string>()
            };

            return new TableNode
            {
                TypeOfEntry = new Union<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }
    }
}