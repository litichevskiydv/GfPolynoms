namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.SourceFiltersCalculator
{
    using System.Collections.Generic;
    using GfPolynoms;

    /// <summary>
    /// Contract for wavelet transform's source filters calculator
    /// </summary>
    public interface ISourceFiltersCalculator
    {
        /// <summary>
        /// Calculates filter banks full definition in the vector style by synthesis filter h <paramref name="h"/>
        /// <param name="h">Synthesis filter h</param>
        /// </summary>
        IEnumerable<FiltersBankVectors> GetSourceFilters(FieldElement[] h);

        /// <summary>
        /// Calculates filter banks full definition in the polynomial style by synthesis filter h <paramref name="h"/>
        /// <param name="h">Synthesis filter h</param>
        /// <param name="expectedDegree">Filters polynomial expected degree</param>
        /// </summary>
        IEnumerable<FiltersBankPolynomials> GetSourceFilters(Polynomial h, int? expectedDegree = null);
    }
}