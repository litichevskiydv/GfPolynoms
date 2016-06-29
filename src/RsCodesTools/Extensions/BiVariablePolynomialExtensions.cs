namespace RsCodesTools.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.CombinationsCountCalculator;
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

        public static Polynomial EvaluateY(this BiVariablePolynomial polynomial, FieldElement yValue)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (yValue == null)
                throw new ArgumentNullException(nameof(yValue));
            if (polynomial.Field.Equals(yValue.Field) == false)
                throw new AggregateException(nameof(yValue));

            var resultCoefficients = new int[polynomial.MaxXDegree + 1];
            foreach (var coefficient in polynomial)
                resultCoefficients[coefficient.Key.Item1] = polynomial.Field.Add(resultCoefficients[coefficient.Key.Item1],
                    (coefficient.Value * FieldElement.Pow(yValue, coefficient.Key.Item2)).Representation);

            return new Polynomial(polynomial.Field, resultCoefficients);
        }

        public static FieldElement CalculateHasseDerivative(this BiVariablePolynomial polynomial,
            int r, int s, FieldElement xValue, FieldElement yValue,
            ICombinationsCountCalculator combinationsCountCalculator, IDictionary<Tuple<int, int>, FieldElement> combinationsCache = null)
        {
            var field = polynomial.Field;
            var derivativeValue = field.Zero();

            foreach (var coefficient in polynomial)
            {
                if (coefficient.Key.Item1 < r || coefficient.Key.Item2 < s)
                    continue;

                derivativeValue += combinationsCountCalculator.Calculate(field, coefficient.Key.Item1, r, combinationsCache)
                                   *combinationsCountCalculator.Calculate(field, coefficient.Key.Item2, s, combinationsCache)
                                   *coefficient.Value
                                   *FieldElement.Pow(xValue, coefficient.Key.Item1 - r)
                                   *FieldElement.Pow(yValue, coefficient.Key.Item2 - s);
            }

            return derivativeValue;
        }
    }
}