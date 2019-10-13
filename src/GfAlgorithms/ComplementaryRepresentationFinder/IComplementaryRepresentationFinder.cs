namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
{
    using System.Collections.Generic;
    using GfPolynoms;

    /// <summary>
    /// Contract of the polynomial complementary representation finder
    /// </summary>
    public interface IComplementaryRepresentationFinder
    {
        /// <summary>
        /// Finds all variants of the polynomial <paramref name="polynomial"/> complementary representations
        /// </summary>
        /// <param name="polynomial">Polynomial for which complementary representations should be found</param>
        /// <param name="maxDegree">Maximum possible degree of the complementary polynomials</param>
        /// <returns>Enumeration consisting of the complementary representation</returns>
        IEnumerable<(Polynomial h, Polynomial g)> Find(Polynomial polynomial, int maxDegree);
    }
}