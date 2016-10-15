namespace AppliedAlgebra.GfAlgorithms.ComplementaryFilterBuilder
{
    using System;
    using Extensions;
    using GfPolynoms;
    using PolynomialsGcdFinder;

    public class GcdBasedBuilder : IComplementaryFiltersBuilder
    {
        private readonly IPolynomialsGcdFinder _gcdFinder;

        /// <summary>
        /// Inverting <paramref name="gcd"/> polynomial in ring GF[x]/x^<paramref name="modularPolynomialDegree"/>-1
        /// </summary>
        private static Polynomial InvertGcdInPolynomialsRing(Polynomial gcd, int modularPolynomialDegree)
        {
            return new Polynomial(gcd.Field, gcd.Field.InverseForMultiplication(gcd[gcd.Degree]))
                .RightShift((modularPolynomialDegree - gcd.Degree)%modularPolynomialDegree);
        }

        public Polynomial Build(Polynomial sourceFilter, int maxFilterLength)
        {
            if(sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));

            var polythaseComponents = sourceFilter.GetPolyphaseComponents();
            var gcdWithQuotients = _gcdFinder.GcdWithQuotients(polythaseComponents.Item1, polythaseComponents.Item2);
            if (gcdWithQuotients.Gcd.IsMonomial() == false)
                throw new InvalidOperationException($"{sourceFilter} polyphase components is not relatively prime");

            var field = sourceFilter.Field;
            var complementaryFilterEvenComponent = new Polynomial(field);
            var complementaryFilterOddComponent = field.Pow(field.InverseForAddition(1), gcdWithQuotients.Quotients.Length)
                                                  *InvertGcdInPolynomialsRing(gcdWithQuotients.Gcd, maxFilterLength/2);
            for (var i = gcdWithQuotients.Quotients.Length - 1; i > -1; i--)
            {
                var complementaryFilterEvenComponentOld = complementaryFilterEvenComponent;

                complementaryFilterEvenComponent = gcdWithQuotients.Quotients[i]*complementaryFilterEvenComponentOld
                                                   + complementaryFilterOddComponent;
                complementaryFilterOddComponent = complementaryFilterEvenComponentOld;
            }

            return PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(complementaryFilterEvenComponent, complementaryFilterOddComponent);
        }

        public GcdBasedBuilder(IPolynomialsGcdFinder gcdFinder)
        {
            if(gcdFinder == null)
                throw new ArgumentNullException(nameof(gcdFinder));

            _gcdFinder = gcdFinder;
        }
    }
}