namespace AppliedAlgebra.WaveletCodesTools.GeneratingPolynomialsBuilder
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// Generating polynomials factory implementation
    /// </summary>
    public class GeneratingPolynomialsFactory : IGeneratingPolynomialsFactory
    {
        /// <inheritdoc />
        public Polynomial Create(Polynomial h, Polynomial g, int expectedDegree)
        {
            if (h == null)
                throw new ArgumentNullException(nameof(h));
            if (g == null)
                throw new ArgumentNullException(nameof(g));
            if (expectedDegree <= 0)
                throw new ArgumentException($"{expectedDegree} must be positive");

            var one = new Polynomial(h.Field, 1);
            var modularPolynomial = (one >> (expectedDegree + 1)) - one;
            return (h + (g >> 2)) % modularPolynomial;
        }
    }
}