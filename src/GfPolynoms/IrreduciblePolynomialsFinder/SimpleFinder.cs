namespace GfPolynoms.IrreduciblePolynomialsFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GaloisFields;

    public class SimpleFinder : IIrreduciblePolynomialsFinder
    {
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

        private static Polynomial GenerateTemplatePolynomial(GaloisField field, int degree)
        {
            var templatePolynomial = new Polynomial(field, 1).RightShift(degree);
            templatePolynomial[0] = 1;

            return templatePolynomial;
        }

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