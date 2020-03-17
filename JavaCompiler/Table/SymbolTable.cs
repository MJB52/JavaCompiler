using System;
using System.Collections.Generic;
using FunctionalSharp.DiscriminatedUnions;
using JavaCompiler.Entry_Types;

namespace JavaCompiler
{
    public class SymbolTable : ISymbolTable
    {
        /// <summary>
        ///     array of linkedlists that contain a TableNode. Not the best might change in future
        /// </summary>
        private readonly LinkedList<TableNode>[] _table;

        private readonly uint _tableSize;

        public SymbolTable(uint size)
        {
            _tableSize = size;

            //initalize the action
            _table = new LinkedList<TableNode> [_tableSize];
            for (var i = 0; i < _tableSize; i++) _table[i] = new LinkedList<TableNode>();
        }

        public void Insert(string lex, Tokens token, int depth)
        {
            var node = CreateVarNode(lex, token, depth);
            /* need to find a way to differentiate to get the right TableNode
            // var node = CreateClassNode(lex, token, depth);
            // var node = CreateFunctionNode(lex, token, depth);
            // var node = CreateConstantNode(lex, token, depth);*/

            var hash = Hash(lex);
            _table[(int) hash].AddLast(node);
        }

        /// <summary>
        ///     Returns the node that matches the lexeme
        /// </summary>
        /// <param name="lex"></param>
        /// <returns>
        ///     TableNode? which means it's signature makes it known to the caller that it can return null thus
        ///     should do their own checking for null
        /// </returns>
        public TableNode? Lookup(string lex)
        {
            foreach (var hashLoc in _table)
                if (hashLoc.Count > 0)
                    foreach (var item in hashLoc)
                        if (item.Lexeme.Value == lex)
                            return item;

            return null;
        }

        public void DeleteDepth(int depth)
        {
            foreach (var hashLoc in _table)
                if (hashLoc.Count > 0)
                    foreach (var item in hashLoc)
                        if (item.Depth == depth)
                            hashLoc.Remove(item); //bad should not modify collection in a foreach buuuuuttt....
        }

        public void WriteTable(int depth)
        {
            foreach (var hashLoc in _table)
                if (hashLoc.Count > 0)
                    foreach (var item in hashLoc)
                    {
                        Console.Write(item.Lexeme.Value + ", " + Enum.GetName(typeof(Tokens), item.Token) + " at " +
                                      item.Depth + ". ");
                        Print(item.TypeOfEntry);
                    }
        }

        public uint Hash(string lex)
        {
            uint hash = 0, test = 0;
            for (uint i = 0; i < lex.Length; i++)
            {
                hash = (hash << 24) + (byte) lex[(int) i];
                if ((test = hash & 0xf0000000) != 0)
                {
                    hash = hash ^ (test >> 24);
                    hash = hash ^ test;
                }
            }

            return hash % _tableSize;
        }

        private void Print(DiscriminatedUnion<ConstantType, FunctionType, VarType, ClassType> union)
        {
            var text = union.Match(
                c => "This is an ConstantType ",
                f => "This is a FunctionType ",
                v => "This is a VarType",
                c => "This is a ClassType ");
            Console.WriteLine(text);
        }

        /// <summary>
        ///     All of these Node Creation methods will return a TableNode with the correct "Type" assigned in the TypeOfEntry
        ///     union. They could probably all be consolidated into one method but i can do that in the future
        /// </summary>
        /// <param name="lex"></param>
        /// <param name="token"></param>
        /// <param name="depth"></param>
        /// <returns>TableNode</returns>
        private TableNode CreateConstantNode(string lex, Tokens token, int depth)
        {
            var type = new ConstantType
            {
                Type = new DiscriminatedUnion<int, float>(5.5F), TypeOfConstant = TypeOfVariable.FloatType, Offset = 6
            };

            return new TableNode
            {
                TypeOfEntry = new DiscriminatedUnion<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }

        /// <summary>
        ///     All of these Node Creation methods will return a TableNode with the correct "Type" assigned in the TypeOfEntry
        ///     union. They could probably all be consolidated into one method but i can do that in the future
        /// </summary>
        /// <param name="lex"></param>
        /// <param name="token"></param>
        /// <param name="depth"></param>
        /// <returns>TableNode</returns>
        private TableNode CreateFunctionNode(string lex, Tokens token, int depth)
        {
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
                TypeOfEntry = new DiscriminatedUnion<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }

        /// <summary>
        ///     All of these Node Creation methods will return a TableNode with the correct "Type" assigned in the TypeOfEntry
        ///     union. They could probably all be consolidated into one method but i can do that in the future
        /// </summary>
        /// <param name="lex"></param>
        /// <param name="token"></param>
        /// <param name="depth"></param>
        /// <returns>TableNode</returns>
        private TableNode CreateVarNode(string lex, Tokens token, int depth)
        {
            var type = new VarType
            {
                TypeOfVariable = TypeOfVariable.FloatType, Offset = 6
            };

            return new TableNode
            {
                TypeOfEntry = new DiscriminatedUnion<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }

        /// <summary>
        ///     All of these Node Creation methods will return a TableNode with the correct "Type" assigned in the TypeOfEntry
        ///     union. They could probably all be consolidated into one method but i can do that in the future
        /// </summary>
        /// <param name="lex"></param>
        /// <param name="token"></param>
        /// <param name="depth"></param>
        /// <returns>TableNode</returns>
        private TableNode CreateClassNode(string lex, Tokens token, int depth)
        {
            var type = new ClassType
            {
                Size = 4, VariableNames = new LinkedList<string>(), MethodNames = new LinkedList<string>()
            };

            return new TableNode
            {
                TypeOfEntry = new DiscriminatedUnion<ConstantType, FunctionType, VarType, ClassType>(type),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }
    }
}