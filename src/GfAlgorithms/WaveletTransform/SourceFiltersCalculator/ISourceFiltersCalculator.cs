namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.SourceFiltersCalculator
{
    using GfPolynoms;

    /// <summary>
    /// Contract for wavelet transform's source filters calculator
    /// </summary>
    public interface ISourceFiltersCalculator
    {
        /// <summary>
        /// Calculates filter bank full definition in the vector style by synthesis filter h <paramref name="h"/>
        /// <param name="h">Synthesis filter h</param>
        /// </summary>
        FiltersBankVectors GetSourceFilters(FieldElement[] h);

        /// <summary>
        /// Calculates filter bank full definition in the polynomial style by synthesis filter h <paramref name="h"/>
        /// <param name="h">Synthesis filter h</param>
        /// <param name="expectedDegree">Filters polynomial expected degree</param>
        /// </summary>
        FiltersBankPolynomials GetSourceFilters(Polynomial h, int? expectedDegree = null);
    }
}