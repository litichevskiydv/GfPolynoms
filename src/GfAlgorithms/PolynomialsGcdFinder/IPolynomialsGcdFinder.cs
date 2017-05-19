namespace AppliedAlgebra.GfAlgorithms.PolynomialsGcdFinder
{
    using GfPolynoms;

    /// <summary>
    /// Contract for greatest common divisor finder
    /// </summary>
    public interface IPolynomialsGcdFinder
    {
        /// <summary>
        /// Method for calculation greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <returns>Calculated greatest common divisor</returns>
        Polynomial Gcd(Polynomial a, Polynomial b);

        /// <summary>
        /// Method for calculating greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/> and produces list of quotients
        /// </summary>
        GcdWithQuotients GcdWithQuotients(Polynomial a, Polynomial b);

        /// <summary>
        /// Method for calculating greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/> and coefficients x and y such that <paramref name="a"/>*x+<paramref name="b"/>*y=gcd(a,b)
        /// </summary>
        GcdExtendedResult GcdExtended(Polynomial a, Polynomial b);
    }
}