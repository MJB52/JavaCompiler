using JavaCompiler.Entry_Types;

namespace JavaCompiler
{
    // //not great but essentially ensures and will overwrite the first address everytime so one of them is actually in mem
    // [StructLayout(LayoutKind.Explicit)] 
    // public struct EntryTypeUnion
    // {
    //     [FieldOffset(0)]
    //     public ConstantType? constantType;
    //     [FieldOffset(0)] 
    //     public FunctionType? funcType;
    //     [FieldOffset(0)] 
    //     public VarType? varType;
    // }
    public class TableNode
    {
        public int Depth;
        public Lexeme Lexeme;
        public Tokens Token;
        public Union<ConstantType, FunctionType, VarType, ClassType> TypeOfEntry; // tag field for the union
    }
}