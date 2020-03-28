namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using VariantsIterator;

    /// <summary>
    /// Complementary representation finder based on brute force
    /// </summary>
    public class BruteForceBasedFinder : IComplementaryRepresentationFinder
    {
        private readonly IVariantsIterator _variantsIterator;

        /// <summary>
        /// Creates complementary representation finder based on brute force
        /// </summary>
        /// <param name="variantsIterator">Iterator of the polynomials over a given field</param>
        public BruteForceBasedFinder(IVariantsIterator variantsIterator)
        {
            _variantsIterator = variantsIterator;
        }

        /// <inheritdoc />
        public IEnumerable<(Polynomial h, Polynomial g)> Find(Polynomial polynomial, int maxDegree, FieldElement lambda = null)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if(maxDegree <= 0)
                throw new ArgumentException($"{nameof(maxDegree)} must be positive");
            if(maxDegree < polynomial.Degree)
                throw new ArgumentException($"{nameof(maxDegree)} must not be less than {nameof(polynomial)} degree");
            if (lambda != null && polynomial.Field.Equals(lambda.Field) == false)
                throw new ArgumentException($"{nameof(lambda)} must belong to the field of the polynomial {nameof(polynomial)}");

            var componentsCoefficientsCount = (maxDegree + 1) / 2;
            var (polynomialEvenComponent, polynomialOddComponent) = polynomial.GetPolyphaseComponents();
            var polynomialEvenComponentSpectrum = polynomialEvenComponent.GetSpectrum(componentsCoefficientsCount - 1);
            var polynomialOddComponentSpectrum = polynomialOddComponent.GetSpectrum(componentsCoefficientsCount - 1);
            for (var i = 0; i < polynomialEvenComponentSpectrum.Length; i++)
                if (polynomialEvenComponentSpectrum[i].Representation == 0 && polynomialOddComponentSpectrum[i].Representation == 0)
                    throw new ArgumentException($"Even and odd component of {polynomial} can't have zero in one point simultaneously");

            var field = polynomial.Field;
            var one = new Polynomial(field.One());
            var lambdaValue = lambda ?? field.One();
            var componentsModularPolynomial = (one >> componentsCoefficientsCount) - one; 
            foreach (var h in _variantsIterator.IteratePolynomials(polynomial.Field, maxDegree).Skip(1))
            {
                var diff = (polynomial - h).GetCoefficients(maxDegree);
                var g = new Polynomial(diff.Skip(2).Concat(diff.Take(2)).Select(x => x / lambdaValue).ToArray());
                if (h.Degree == 0 || g.Degree == 0)
                    continue;

                var (he, ho) = h.GetPolyphaseComponents();
                var (ge, go) = g.GetPolyphaseComponents();
                if (Equals(one, (he * go - ge * ho) % componentsModularPolynomial))
                    yield return (h, g);
            }
        }
    }
}