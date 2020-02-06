namespace AppliedAlgebra.GfAlgorithms.ComplementaryFilterBuilder
{
    using System;
    using Extensions;
    using GfPolynoms;
    using PolynomialsGcdFinder;

    /// <summary>
    /// Implementation of complementary polynomials builder contract based on GCD algorithm
    /// </summary>
    public class GcdBasedBuilder : IComplementaryFiltersBuilder
    {
        private readonly IPolynomialsGcdFinder _gcdFinder;

        /// <summary>
        /// Inverting <paramref name="gcd"/> polynomial in ring GF[x]/x^<paramref name="modularPolynomialDegree"/>-1
        /// </summary>
        private static Polynomial InvertGcdInPolynomialsRing(Polynomial gcd, int modularPolynomialDegree)
        {
            var result = new Polynomial(gcd.Field, gcd.Field.InverseForMultiplication(gcd[gcd.Degree]));
            return gcd.Degree == 0 ? result : result.RightShift((modularPolynomialDegree - gcd.Degree) % modularPolynomialDegree);
        }

        /// <summary>
        /// Method for building complementary polynomial for polynomial <paramref name="sourceFilter"/> with coefficients count <paramref name="maxFilterLength"/>
        /// </summary>
        /// <param name="sourceFilter">Polynomial for which complementary polynomial should be built</param>
        /// <param name="maxFilterLength">Coefficients count in complementary polynomial</param>
        /// <returns>Built complementary polynomial</returns>
        public Polynomial Build(Polynomial sourceFilter, int maxFilterLength)
        {
            if(sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));

            var polythaseComponents = sourceFilter.GetPolyphaseComponents();
            var gcdWithQuotients = _gcdFinder.GcdWithQuotients(polythaseComponents.Item1, polythaseComponents.Item2);
            if (gcdWithQuotients.Gcd.IsMonomial() == false || maxFilterLength % 2 == 1 && gcdWithQuotients.Gcd.Degree > 0)
                throw new InvalidOperationException($"{sourceFilter} polyphase components is not relatively prime");

            var field = sourceFilter.Field;
            var complementaryFilterEvenComponent = new Polynomial(field);
            var complementaryFilterOddComponent = field.Pow(field.InverseForAddition(1), gcdWithQuotients.Quotients.Length)
                                                  * InvertGcdInPolynomialsRing(gcdWithQuotients.Gcd, maxFilterLength / 2);
            for (var i = gcdWithQuotients.Quotients.Length - 1; i > -1; i--)
            {
                var complementaryFilterEvenComponentOld = complementaryFilterEvenComponent;

                complementaryFilterEvenComponent = gcdWithQuotients.Quotients[i]*complementaryFilterEvenComponentOld
                                                   + complementaryFilterOddComponent;
                complementaryFilterOddComponent = complementaryFilterEvenComponentOld;
            }

            var m = new Polynomial(field, 1).RightShift(maxFilterLength);
            m[0] = field.InverseForAddition(1);
            return PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(
                       complementaryFilterEvenComponent,
                       complementaryFilterOddComponent
                   ) % m;
        }

        /// <summary>
        /// Constructor for builder creation
        /// </summary>
        /// <param name="gcdFinder">Implementation of GCD algorithm contract</param>
        public GcdBasedBuilder(IPolynomialsGcdFinder gcdFinder)
        {
            if(gcdFinder == null)
                throw new ArgumentNullException(nameof(gcdFinder));

            _gcdFinder = gcdFinder;
        }
    }
}