namespace GfAlgorithms.CombinationsCountCalculator
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class PascalsTriangleBasedCalcualtor : ICombinationsCountCalculator
    {
        public FieldElement Calculate(GaloisField field, int n, int k, IDictionary<Tuple<int, int>, FieldElement> combinationsCache = null)
        {
            var key = new Tuple<int, int>(n, k);
            FieldElement combinationsCount;

            if (combinationsCache == null || combinationsCache.TryGetValue(key, out combinationsCount) == false)
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

                if (combinationsCache != null)
                    combinationsCache[key] = combinationsCount;
            }

            return combinationsCount;
        }
    }
}