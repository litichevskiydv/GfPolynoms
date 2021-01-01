namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.FiltersBanksIterator
{
    using System.Collections.Generic;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Contract for iterator of the filter banks with required properties
    /// </summary>
    public interface IFiltersBanksIterator
    {
        /// <summary>
        /// Iterates filter banks in vector form
        /// </summary>
        /// <param name="field">Galois field</param>
        /// <param name="filtersLength">Filters length</param>
        IEnumerable<FiltersBankVectors> IterateFiltersBanksVectors(GaloisField field, int filtersLength);

        /// <summary>
        /// Iterates filter banks in polynomial form
        /// </summary>
        /// <param name="field">Galois field</param>
        /// <param name="expectedDegree">Filters polynomials expected degree</param>
        IEnumerable<FiltersBankPolynomials> IterateFiltersBanksPolynomials(GaloisField field, int expectedDegree);
    }
}