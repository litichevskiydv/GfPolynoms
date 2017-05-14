namespace AppliedAlgebra.GfAlgorithms.CombinationsCountCalculator
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Contract for calculator of combinations count
    /// </summary>
    public interface ICombinationsCountCalculator
    {
        /// <summary>
        /// Method for calculating number of combinations from <paramref name="n"/> by <paramref name="k"/> over field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field over which combinations count'll calculated</param>
        /// <param name="n">Total elements count</param>
        /// <param name="k">Selected elements count</param>
        /// <param name="combinationsCache">Cache for storing combinations count</param>
        /// <returns>Combinations count</returns>
        FieldElement Calculate(GaloisField field, int n, int k, FieldElement[][] combinationsCache = null);
    }
}