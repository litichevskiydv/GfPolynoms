namespace AppliedAlgebra.GfAlgorithms.CombinationsCountCalculator
{
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Combinations calculator contract implementation via Pascal's triangle
    /// </summary>
    public class PascalsTriangleBasedCalculator : ICombinationsCountCalculator
    {
        /// <summary>
        /// Method for calculating number of combinations from <paramref name="n"/> by <paramref name="k"/> over field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field over which combinations count'll calculated</param>
        /// <param name="n">Total elements count</param>
        /// <param name="k">Selected elements count</param>
        /// <param name="combinationsCache">Cache for storing combinations count</param>
        /// <returns>Combinations count</returns>
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