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
        /// <summary>
        /// Method for performing brute force search of irreducible polynomial
        /// </summary>
        /// <param name="templatePolynomial">Polynomial on which search is performed</param>
        /// <param name="position">Analyzed monomial degree</param>
        /// <param name="onCompleteAction">Results checker</param>
        /// <returns>Search status</returns>
        private static bool Generate(Polynomial templatePolynomial, int position, Func<Polynomial, bool> onCompleteAction)
        {
            if (templatePolynomial.Degree == position)
                return onCompleteAction(templatePolynomial);

            var stopGeneration = false;
            var originalValue = templatePolynomial[position];
            for (var i = originalValue; i < templatePolynomial.Field.Order && stopGeneration == false; i++)
            {
                templatePolynomial[position] = i;
                stopGeneration = Generate(templatePolynomial, position + 1, onCompleteAction);
            }

            if (stopGeneration == false)
                templatePolynomial[position] = originalValue;
            return stopGeneration;
        }

        /// <summary>
        /// Method for creation initialization polynomial for brute force search
        /// </summary>
        /// <param name="field">Polynomial field</param>
        /// <param name="degree">Polynomial degree</param>
        private static Polynomial GenerateTemplatePolynomial(GaloisField field, int degree)
        {
            var templatePolynomial = new Polynomial(field, 1).RightShift(degree);
            templatePolynomial[0] = 1;

            return templatePolynomial;
        }

        /// <inheritdoc />
        public Polynomial Find(GaloisField field, int degree)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));
            if (degree < 2)
                throw new ArgumentException(nameof(degree));

            var possibleDivisors = new List<Polynomial>();
            for (var i = 1; i*i <= degree; i++)
            {
                var templatePolynomial = GenerateTemplatePolynomial(field, i);
                Generate(templatePolynomial, 0,
                    x =>
                    {
                        possibleDivisors.Add(new Polynomial(x));
                        return false;
                    });
            }

            var result = GenerateTemplatePolynomial(field, degree);
            Generate(result, 0, x => possibleDivisors.All(d => (x%d).IsZero == false));

            return result;
        }
    }
}