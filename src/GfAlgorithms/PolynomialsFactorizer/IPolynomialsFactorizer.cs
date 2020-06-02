namespace AppliedAlgebra.GfAlgorithms.PolynomialsFactorizer
{
    using System.Collections.Generic;
    using GfPolynoms;

    /// <summary>
    /// Contract for polynomials factorizer
    /// </summary>
    public interface IPolynomialsFactorizer
    {
        /// <summary>
        /// Factors polynomial <paramref name="polynomial"/>
        /// </summary>
        /// <param name="polynomial">Polynomial for factorization</param>
        IEnumerable<(Polynomial factor, int degree)> Factorize(Polynomial polynomial);
    }
}