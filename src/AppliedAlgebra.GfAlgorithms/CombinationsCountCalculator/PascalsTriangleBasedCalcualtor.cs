namespace AppliedAlgebra.GfAlgorithms.CombinationsCountCalculator
{
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class PascalsTriangleBasedCalcualtor : ICombinationsCountCalculator
    {
        public FieldElement Calculate(GaloisField field, int n, int k, FieldElement[][] combinationsCache = null)
        {
            var combinationsCount = combinationsCache?[n][k];

            if (combinationsCount == null)
            {
                if (k == 0 || n == k)
                    combinationsCount = field.One();
                else
                {
                    if (n < k)
                        combinationsCount = field.Zero();
                    else
                        combinationsCount = Calculate(field, n - 1, k - 1, combinationsCache)
                                            + Calculate(field, n - 1, k, combinationsCache);
                }

                if (combinationsCache != null && n < combinationsCache.Length && k < combinationsCache[n].Length)
                    combinationsCache[n][k] = combinationsCount;
            }

            return combinationsCount;
        }
    }
}