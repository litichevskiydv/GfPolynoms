namespace RsListDecoding.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public static class BiVariablePolynomialExtensions
    {
        private static BiVariablePolynomial Pow(IDictionary<int, BiVariablePolynomial> powersCache, BiVariablePolynomial polynomial, int degree)
        {
            BiVariablePolynomial result;

            if (powersCache.TryGetValue(degree, out result) == false)
            {
                if (degree == 0)
                    result = new BiVariablePolynomial(polynomial.Field) {[new Tuple<int, int>(0, 0)] = polynomial.Field.One()};
                else
                    result = Pow(powersCache, polynomial, degree - 1)*polynomial;
                powersCache[degree] = result;
            }

            return result;
        }

        public static BiVariablePolynomial PerformVariablesSubstitution(this BiVariablePolynomial polynomial,
            BiVariablePolynomial xSubstitution, BiVariablePolynomial ySubstitution)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (xSubstitution == null)
                throw new ArgumentNullException(nameof(xSubstitution));
            if (ySubstitution == null)
                throw new ArgumentNullException(nameof(ySubstitution));
            if(polynomial.Field.Equals(xSubstitution.Field) == false)
                throw new ArithmeticException(nameof(xSubstitution));
            if (polynomial.Field.Equals(ySubstitution.Field) == false)
                throw new ArithmeticException(nameof(ySubstitution));

            var result = new BiVariablePolynomial(polynomial.Field);
            var xCache = new Dictionary<int, BiVariablePolynomial>();
            var yCache = new Dictionary<int, BiVariablePolynomial>();

            foreach (var coefficient in polynomial)
                result += coefficient.Value
                          *Pow(xCache, xSubstitution, coefficient.Key.Item1)
                          *Pow(yCache, ySubstitution, coefficient.Key.Item2);

            return result;
        }

        public static BiVariablePolynomial DivideByMaxPossibleXDegree(this BiVariablePolynomial polynomial)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            var result = new BiVariablePolynomial(polynomial.Field);
            if (polynomial.IsZero)
                return result;

            var minXDegree = polynomial.Min(x => x.Key.Item1);
            foreach (var coefficient in polynomial)
                result[new Tuple<int, int>(coefficient.Key.Item1 - minXDegree, coefficient.Key.Item2)] = new FieldElement(coefficient.Value);

            return result;
        }

        public static Polynomial EvaluateX(this BiVariablePolynomial polynomial, FieldElement xValue)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if(xValue == null)
                throw new ArgumentNullException(nameof(xValue));
            if (polynomial.Field.Equals(xValue.Field) == false)
                throw new AggregateException(nameof(xValue));

            var resultCoefficients = new int[polynomial.MaxYDegree + 1];
            foreach (var coefficient in polynomial)
                resultCoefficients[coefficient.Key.Item2] = polynomial.Field.Add(resultCoefficients[coefficient.Key.Item2],
                    (coefficient.Value*FieldElement.Pow(xValue, coefficient.Key.Item1)).Representation);

            return new Polynomial(polynomial.Field, resultCoefficients);
        }
    }
}