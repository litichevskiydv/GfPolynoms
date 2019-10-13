namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using VectorsIterator;

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
        public IEnumerable<(Polynomial h, Polynomial g)> Find(Polynomial polynomial, int maxDegree)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if(maxDegree <= 0)
                throw new ArgumentException($"{nameof(maxDegree)} must be positive");
            if(maxDegree < polynomial.Degree)
                throw new ArgumentException($"{nameof(maxDegree)} must not be less than {nameof(polynomial)} degree");

            var one = new Polynomial(polynomial.Field.One());
            foreach (var h in _variantsIterator.IteratePolynomials(polynomial.Field, maxDegree).Skip(1))
            {
                var diff = (polynomial - h).GetCoefficients(maxDegree);
                var g = new Polynomial(diff.Skip(2).Concat(diff.Take(2)).ToArray());

                var (he, ho) = h.GetPolyphaseComponents();
                var (ge, go) = g.GetPolyphaseComponents();
                if (Equals(he * go - ge * ho, one))
                    yield return (h, g);
            }
        }
    }
}