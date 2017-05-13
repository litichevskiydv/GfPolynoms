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
        /// <param name="position">Analysed monomial degree</param>
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

        /// <summary>
        /// Method for finding irreducible polynomial degree <paramref name="degree"/> with coefficients from field with order <paramref name="fieldOrder"/>
        /// </summary>
        /// <param name="fieldOrder">Field order from which irreducible polynomials coefficients come</param>
        /// <param name="degree">Irreducible polynomial degree</param>
        /// <returns>Irreducible polynomial with specified properties</returns>
        public Polynomial Find(int fieldOrder, int degree)
        {
            if (degree < 2)
                throw new ArgumentException(nameof(degree));

            var field = new PrimeOrderField(fieldOrder);

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