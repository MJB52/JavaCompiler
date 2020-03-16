namespace JavaCompiler
{
    public class SymbolTable : ISymbolTable
    {
        private Node theTree;

        public void Insert()
        {
            //create node
            var temp = new Node();
            Insert(temp, ref theTree);
        }

        public bool Lookup() => false;

        public void DeleteDepth(int depth)
        {
        }

        public void WriteTable(int depth)
        {
        }

        private void Insert(Node temp, ref Node root)
        {
            // int compare = root.Symbol.CompareTo(temp.Symbol);
            if (root == null)
            {
                temp.left = null;
                temp.right = null;
                root = temp; // Insert the node.
            }
            else if (root.Symbol.CompareTo(temp.Symbol) > 0)
            {
                Insert(temp, ref root.left); // Search the left branch
            }
            else if (root.Symbol.CompareTo(temp.Symbol) < 0)
            {
                Insert(temp, ref root.right); // Search the right branch
            }
        }
    }
}