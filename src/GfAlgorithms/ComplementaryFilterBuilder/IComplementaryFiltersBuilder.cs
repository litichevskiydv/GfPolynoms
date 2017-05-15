namespace AppliedAlgebra.GfAlgorithms.ComplementaryFilterBuilder
{
    using GfPolynoms;

    /// <summary>
    /// Contract for complementary polynomials builder
    /// </summary>
    public interface IComplementaryFiltersBuilder
    {
        /// <summary>
        /// Method for building complementary polynomial for polynomial <paramref name="sourceFilter"/> with coefficients count <paramref name="maxFilterLength"/>
        /// </summary>
        /// <param name="sourceFilter">Polynomial for which complementary polynomial should be built</param>
        /// <param name="maxFilterLength">Coefficients count in complementary polynomial</param>
        /// <returns>Built complementary polynomial</returns>
        Polynomial Build(Polynomial sourceFilter, int maxFilterLength);
    }
}