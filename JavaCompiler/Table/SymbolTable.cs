using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Insert(string lex, Tokens token, int depth, EntryType type)
        {
            TableNode node = new TableNode
            {
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
            //check for duplicates 
            var hash = Hash(lex);
            CheckForDupes(hash, lex, depth);
            _table[(int) hash].AddLast(node);
        }

        private void CheckForDupes(uint hash, string lex, int depth)
        {
            foreach (var hashLoc in _table[(int)hash])
            {
                if (hashLoc.Depth == depth)
                {
                    ConsoleLogger.DuplicateLexeme(lex, depth);
                    throw new ParseErrorException();
                }
            }
        }

        /// <summary>
        ///     Returns the node that matches the lexeme
        /// </summary>
        /// <param name="lex"></param>
        /// <param name="depth"></param>
        /// <returns>
        ///     TableNode? which means it's signature makes it known to the caller that it can return null thus
        ///     should do their own checking for null
        /// </returns>
        public TableNode? Lookup(string lex)
        {
            return _table[Hash(lex)].FirstOrDefault(entry => entry.Lexeme.Value == lex);
        }

        public void DeleteDepth(int depth)
        {
            foreach (var hashLoc in _table)
                if (hashLoc.Count > 0)
                {
                    var node = hashLoc.First;
                    while (node != null)
                    {
                        var nextNode = node.Next;
                        if (node.Value.Depth == depth)
                            hashLoc.Remove(node);
                        node = nextNode;
                    }
                }
        }

        public void WriteTable(int depth)
        {
            foreach (var hashLoc in _table)
                if (hashLoc.Count > 0)
                    foreach (var item in hashLoc)
                    {
                        if (item.Depth != depth)
                            continue;
                        
                        switch (item.TypeOfEntry.Tag)
                        {
                            case EntryType.ClassType:
                                var classDetails = item.TypeOfEntry.As<ClassType>();
                                Console.WriteLine($"Class {item.Lexeme.Value} at depth {item.Depth}");
                                Console.WriteLine($"Size: {classDetails.Size}");
                                Console.Write("Method Names: ");
                                classDetails.MethodNames.ToList().ForEach(x => Console.Write($"{x} "));
                                Console.WriteLine();
                                Console.Write("Variable Names: ");
                                classDetails.VariableNames.ToList().ForEach(x => Console.Write($"{x} "));
                                Console.WriteLine();
                                Console.WriteLine();
                                break;
                            case EntryType.ConstantType:
                                var constDetails = item.TypeOfEntry.As<ConstantType>();
                                Console.WriteLine($"Constant {item.Lexeme.Value} at depth {item.Depth}");
                                Console.WriteLine();
                                break;
                            case EntryType.FunctionType:
                                var functionDetails = item.TypeOfEntry.As<FunctionType>();
                                Console.WriteLine($"Function {item.Lexeme.Value} at depth {item.Depth}");
                                Console.WriteLine($"Total size of method: {functionDetails.TotalSize}");
                                Console.Write("Parameter Types: ");
                                functionDetails.ParamList.ToList().ForEach(x => Console.Write($"{Enum.GetName(typeof(TypeOfVariable), x.TypeOfParameter)} "));
                                Console.WriteLine();
                                Console.WriteLine();
                                break;
                            case EntryType.VarType:
                                var varDetails = item.TypeOfEntry.As<VarType>();
                                Console.WriteLine($"Var {item.Lexeme.Value} at depth {item.Depth} is type: {Enum.GetName(typeof(TypeOfVariable), varDetails.TypeOfVariable)}");
                                Console.WriteLine($"Offset: {varDetails.Offset} | Size: {varDetails.Size}");
                                Console.WriteLine();
                                break;
                            default:
                                break;
                        }
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
                Offset = 0
            };

            return new TableNode
            {
                TypeOfEntry = new Union<EntryType>(type, EntryType.ConstantType),
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
                ParamList = new LinkedList<ParameterNode>(),
                ParamaterOffsetSize = 0,
                SizeOfLocal = 0,
                TotalSize = 0
            };

            return new TableNode
            {
                TypeOfEntry = new Union<EntryType>(type, EntryType.FunctionType),
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
                Offset = 0,
                Size = 0,
            };

            return new TableNode
            {
                TypeOfEntry = new Union<EntryType>(type, EntryType.VarType),
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
                Size = 0,
                VariableNames = new LinkedList<string>(),
                MethodNames = new LinkedList<string>()
            };

            return new TableNode
            {
                TypeOfEntry = new Union<EntryType>(type, EntryType.ClassType),
                Token = token,
                Depth = depth,
                Lexeme = new Lexeme(lex)
            };
        }
    }
}