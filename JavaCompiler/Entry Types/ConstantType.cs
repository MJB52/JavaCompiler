namespace JavaCompiler.Entry_Types
{
    public class ConstantType
    {
        public int Offset;
        public int Size;
        public object Value;
        public TypeOfVariable TypeOfConstant; //int or float constant
        public string OffsetName;
    }
}