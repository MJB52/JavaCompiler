namespace JavaCompiler.Entry_Types
{
    public class ConstantType
    {
        public int Offset;
        public Union<int, float> type;
        public TypeOfVariable TypeOfConstant; //int or float constant
    }
}