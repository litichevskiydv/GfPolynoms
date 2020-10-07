namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.IterationFiltersCalculator
{
    using GfPolynoms;

    /// <summary>
    /// Contract for wavelet transform's iteration filters calculator
    /// </summary>
    public interface IIterationFiltersCalculator
    {
        /// <summary>
        /// Calculates filter for iteration <paramref name="iterationNumber"/> from <paramref name="sourceFilter"/>
        /// </summary>
        /// <param name="iterationNumber">Transformation process iteration number, numbering starts from zero</param>
        /// <param name="sourceFilter">Source filter defined by components array</param>
        FieldElement[] GetIterationFilter(int iterationNumber, FieldElement[] sourceFilter);

        /// <summary>
        /// Calculates filter for iteration <paramref name="iterationNumber"/> from <paramref name="sourceFilter"/>
        /// </summary>
        /// <param name="iterationNumber">Transformation process iteration number, numbering starts from zero</param>
        /// <param name="sourceFilter">Source filter defined by polynomial</param>
        /// <param name="expectedDegree">Filters polynomial expected degree</param>
        Polynomial GetIterationFilter(int iterationNumber, Polynomial sourceFilter, int? expectedDegree = null);
    }
}