using System;
using System.Collections.Generic;

namespace FeatureOne
{
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> equalityFunction;

        public LambdaComparer(Func<T, T, bool> equalityFunction)
        {
            this.equalityFunction = equalityFunction ?? throw new ArgumentNullException();
        }

        public bool Equals(T x, T y)
        {
            return equalityFunction(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}