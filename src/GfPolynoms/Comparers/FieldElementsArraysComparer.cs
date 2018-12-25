namespace AppliedAlgebra.GfPolynoms.Comparers
{
    using System.Collections.Generic;
    using System.Linq;

    public class FieldElementsArraysComparer : IEqualityComparer<FieldElement[]>
    {
        public bool Equals(FieldElement[] x, FieldElement[] y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return x.SequenceEqual(y);
        }

        public int GetHashCode(FieldElement[] obj)
        {
            unchecked
            {
                return obj.Aggregate(0, (hash, x) => hash * 31 ^ x.GetHashCode());
            }
        }
    }
}
