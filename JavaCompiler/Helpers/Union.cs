using System;
using System.Diagnostics.Tracing;

namespace JavaCompiler
{
    public class Union<T> where T: struct, IConvertible
    {
        public object Value;
        public T Tag;

        public Union(object value, T tag)
        {
            Value = value;
            Tag = tag;
        }

        public TOne As<TOne>()
        {
            string name = Enum.GetName(typeof(T), Tag);

            if (typeof(TOne).Name == name)
                return (TOne) Value;
            
            throw new ArgumentException($"Argument type of {typeof(TOne).Name} did not match Tag value {name}");
        }
    }
}