namespace GfAlgorithms.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BiVariablePolynomials;
    using CombinationsCountCalculator;
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

            var result = new BiVariablePolynomial(polynomial.Field,
                polynomial.CoefficientsCount*(Math.Max(xSubstitution.CoefficientsCount, ySubstitution.CoefficientsCount) + 1));
            var xCache = new Dictionary<int, BiVariablePolynomial>(polynomial.CoefficientsCount);
            var yCache = new Dictionary<int, BiVariablePolynomial>(polynomial.CoefficientsCount);

            foreach (var coefficient in polynomial)
                result.Add(coefficient.Value, Pow(xCache, xSubstitution, coefficient.Key.Item1)
                                              *Pow(yCache, ySubstitution, coefficient.Key.Item2));

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

            var field = polynomial.Field;
            var resultCoefficients = new int[polynomial.MaxYDegree + 1];
            foreach (var coefficient in polynomial)
                resultCoefficients[coefficient.Key.Item2] = field.Add(resultCoefficients[coefficient.Key.Item2],
                    field.Multiply(coefficient.Value.Representation, field.Pow(xValue.Representation, coefficient.Key.Item1)));

            return new Polynomial(field, resultCoefficients);
        }

        public static Polynomial EvaluateY(this BiVariablePolynomial polynomial, FieldElement yValue)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (yValue == null)
                throw new ArgumentNullException(nameof(yValue));
            if (polynomial.Field.Equals(yValue.Field) == false)
                throw new AggregateException(nameof(yValue));

            var field = polynomial.Field;
            var resultCoefficients = new int[polynomial.MaxXDegree + 1];
            foreach (var coefficient in polynomial)
                resultCoefficients[coefficient.Key.Item1] = field.Add(resultCoefficients[coefficient.Key.Item1],
                    field.Multiply(coefficient.Value.Representation, field.Pow(yValue.Representation, coefficient.Key.Item2)));

            return new Polynomial(field, resultCoefficients);
        }

        public static FieldElement CalculateHasseDerivative(this BiVariablePolynomial polynomial,
            int r, int s, FieldElement xValue, FieldElement yValue,
            ICombinationsCountCalculator combinationsCountCalculator, FieldElement[][] combinationsCache = null)
        {
            var field = polynomial.Field;
            var derivativeValue = 0;

            foreach (var coefficient in polynomial)
            {
                if (coefficient.Key.Item1 < r || coefficient.Key.Item2 < s)
                    continue;

                var currentAddition = combinationsCountCalculator.Calculate(field, coefficient.Key.Item1, r, combinationsCache).Representation;
                currentAddition = field.Multiply(currentAddition,
                    combinationsCountCalculator.Calculate(field, coefficient.Key.Item2, s, combinationsCache).Representation);
                currentAddition = field.Multiply(currentAddition, coefficient.Value.Representation);
                currentAddition = field.Multiply(currentAddition, field.Pow(xValue.Representation, coefficient.Key.Item1 - r));
                currentAddition = field.Multiply(currentAddition, field.Pow(yValue.Representation, coefficient.Key.Item2 - s));

                derivativeValue = field.Add(derivativeValue, currentAddition);
            }

            return new FieldElement(field, derivativeValue);
        }
    }
}