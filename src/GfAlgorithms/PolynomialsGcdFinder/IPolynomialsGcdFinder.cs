namespace GfAlgorithms.PolynomialsGcdFinder
{
    using GfPolynoms;

    public interface IPolynomialsGcdFinder
    {
        Polynomial Gcd(Polynomial a, Polynomial b);

        GcdWithQuotients GcdWithQuotients(Polynomial a, Polynomial b);

        /// <summary>
        /// Find x and y such that a*x + b*y = gcd(a,b) 
        /// </summary>
        GcdExtendedResult GcdExtended(Polynomial a, Polynomial b);
    }
}