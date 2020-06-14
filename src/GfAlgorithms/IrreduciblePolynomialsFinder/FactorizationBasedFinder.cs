namespace AppliedAlgebra.GfAlgorithms.IrreduciblePolynomialsFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using GfPolynoms.IrreduciblePolynomialsFinder;
    using PolynomialsFactorizer;

    /// <summary>
    /// Irreducible polynomials finder based on factorization procedure
    /// </summary>
    public class FactorizationBasedFinder : IIrreduciblePolynomialsFinder
    {
        private readonly IPolynomialsFactorizer _polynomialsFactorizer;

        public FactorizationBasedFinder(IPolynomialsFactorizer polynomialsFactorizer)
        {
            if (polynomialsFactorizer == null)
                throw new ArgumentNullException(nameof(polynomialsFactorizer));

            _polynomialsFactorizer = polynomialsFactorizer;
        }


        /// <inheritdoc />
        public IEnumerable<Polynomial> Find(GaloisField field, int degree)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (degree < 2)
                throw new ArgumentException(nameof(degree));

            var one = new Polynomial(field.One());
            var polynomial = (one >> field.Order.Pow(degree)) - (one >> 1);
            return _polynomialsFactorizer.Factorize(polynomial).Where(x => x.factor.Degree == degree).Select(x => x.factor);
        }
    }
}