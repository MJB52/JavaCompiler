using FunctionalSharp.DiscriminatedUnions;

namespace JavaCompiler.Entry_Types
{
    public struct ConstantType
    {
        public int Offset;
        public DiscriminatedUnion<int, float> Type;
        public TypeOfVariable TypeOfConstant; //int or float constant
    }
}