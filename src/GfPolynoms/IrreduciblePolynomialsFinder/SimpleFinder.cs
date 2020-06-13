namespace AppliedAlgebra.GfPolynoms.IrreduciblePolynomialsFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GaloisFields;

    /// <summary>
    /// Irreducible polynomial finder based on brute force
    /// </summary>
    public class SimpleFinder : IIrreduciblePolynomialsFinder
    {
        private static IEnumerable<Polynomial> Generate(Polynomial templatePolynomial, int position, Func<Polynomial, bool> predicate = null)
        {
            if (templatePolynomial.Degree == position)
            {
                if (predicate == null || predicate(templatePolynomial))
                    yield return new Polynomial(templatePolynomial);
                yield break;
            }

            var originalValue = templatePolynomial[position];
            for (var i = originalValue; i < templatePolynomial.Field.Order; i++)
            {
                templatePolynomial[position] = i;
                foreach (var polynomial in Generate(templatePolynomial, position + 1, predicate))
                    yield return polynomial;
            }

            templatePolynomial[position] = originalValue;
        }

        private static Polynomial GetTemplatePolynomial(GaloisField field, int degree)
        {
            var one = new Polynomial(field, 1);
            return one + (one >> degree);
        }

        /// <inheritdoc />
        public IEnumerable<Polynomial> Find(GaloisField field, int degree)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));
            if (degree < 2)
                throw new ArgumentException(nameof(degree));

            var possibleDivisors = new List<Polynomial>();
            for (var i = 1; i * i <= degree; i++)
                possibleDivisors.AddRange(Generate(GetTemplatePolynomial(field, i), 0));

            Func<Polynomial, bool> predicate = x => possibleDivisors.All(d => (x % d).IsZero == false);
            foreach (var polynomial in Generate(GetTemplatePolynomial(field, degree), 0, predicate))
                yield return polynomial;
        }
    }
}