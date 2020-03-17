using System;
using System.Diagnostics;

namespace JavaCompiler
{
    public class UnionBase
    {
        private readonly object value;

        protected UnionBase(object x)
        {
            value = x;
        }

        protected T InternalMatch<T>(params Delegate[] ds)
        {
            var vt = value.GetType();
            foreach (var d in ds)
            {
                var mi = d.Method;

                // These are always true if InternalMatch is used correctly.
                Debug.Assert(mi.GetParameters().Length == 1);
                Debug.Assert(typeof(T).IsAssignableFrom(mi.ReturnType));

                var pt = mi.GetParameters()[0].ParameterType;
                if (pt.IsAssignableFrom(vt))
                    return (T) mi.Invoke(null, new[] {value});
            }

            throw new Exception("No appropriate matching function was provided");
        }
    }

    public class Union<A, B> : UnionBase
    {
        public Union(A a) : base(a)
        {
        }

        public Union(B b) : base(b)
        {
        }

        protected Union(object x) : base(x)
        {
        }

        public T Match<T>(Func<A, T> fa, Func<B, T> fb) => InternalMatch<T>(fa, fb);
    }

    public class Union<A, B, C> : UnionBase
    {
        public Union(A a) : base(a)
        {
        }

        public Union(B b) : base(b)
        {
        }

        public Union(C c) : base(c)
        {
        }

        protected Union(object x) : base(x)
        {
        }

        public T Match<T>(Func<A, T> fa, Func<B, T> fb, Func<C, T> fc) => InternalMatch<T>(fa, fb, fc);
    }

    public class Union<A, B, C, D> : UnionBase
    {
        public Union(A a) : base(a)
        {
        }

        public Union(B b) : base(b)
        {
        }

        public Union(C c) : base(c)
        {
        }

        public Union(D d) : base(d)
        {
        }

        protected Union(object x) : base(x)
        {
        }

        public T Match<T>(Func<A, T> fa, Func<B, T> fb, Func<C, T> fc, Func<D, T> fd) =>
            InternalMatch<T>(fa, fb, fc, fd);
    }
}