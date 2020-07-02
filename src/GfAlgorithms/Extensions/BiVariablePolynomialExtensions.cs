namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BiVariablePolynomials;
    using CombinationsCountCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Class for bivariate polynomials extensions
    /// </summary>
    public static class BiVariablePolynomialExtensions
    {
        /// <summary>
        /// Exponentiation of bivariate polynomial <paramref name="polynomial"/> to the degree <paramref name="degree"/>
        /// </summary>
        /// <param name="powersCache">Cache for storing Exponentiation results </param>
        /// <param name="polynomial">Bivariate polynomial for exponentiation</param>
        /// <param name="degree">Power for exponentiation</param>
        private static BiVariablePolynomial Pow(
            IDictionary<int, BiVariablePolynomial> powersCache,
            BiVariablePolynomial polynomial, int degree
        )
        {
            if (powersCache.TryGetValue(degree, out var result) == false)
            {
                if (degree == 0)
                    result = new BiVariablePolynomial(polynomial.Field) {[(0, 0)] = polynomial.Field.One()};
                else
                    result = Pow(powersCache, polynomial, degree - 1) * polynomial;
                powersCache[degree] = result;
            }

            return result;
        }

        /// <summary>
        /// Method for calculating results of replacement bivariate polynomial <paramref name="polynomial"/> variable x by <paramref name="xSubstitution"/> and y by <paramref name="ySubstitution"/>
        /// </summary>
        /// <param name="polynomial">Modified polynomial</param>
        /// <param name="xSubstitution">Substitution for x variable</param>
        /// <param name="ySubstitution">Substitution for x variable</param>
        /// <returns>Replacement results</returns>
        public static BiVariablePolynomial PerformVariablesSubstitution(
            this BiVariablePolynomial polynomial,
            BiVariablePolynomial xSubstitution, 
            BiVariablePolynomial ySubstitution
        )
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
                result.Add(coefficient.Value, Pow(xCache, xSubstitution, coefficient.Key.xDegree)
                                              *Pow(yCache, ySubstitution, coefficient.Key.yDegree));

            return result;
        }

        /// <summary>
        /// Method for dividing bivariate polynomial <paramref name="polynomial"/> by variable x in maximum possible degree
        /// </summary>
        /// <param name="polynomial">Modified polynomial</param>
        /// <returns>Dividing result</returns>
        public static BiVariablePolynomial DivideByMaxPossibleXDegree(this BiVariablePolynomial polynomial)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            var result = new BiVariablePolynomial(polynomial.Field);
            if (polynomial.IsZero)
                return result;

            var minXDegree = polynomial.Min(x => x.Key.xDegree);
            foreach (var coefficient in polynomial)
                result[(coefficient.Key.xDegree - minXDegree, coefficient.Key.yDegree)] = new FieldElement(coefficient.Value);

            return result;
        }

        /// <summary>
        /// Method Calculating results of replacement bivariate polynomial <paramref name="polynomial"/> variable x by value <paramref name="xValue"/>
        /// </summary>
        /// <param name="polynomial">Modified polynomial</param>
        /// <param name="xValue">Value for x variable</param>
        /// <returns>Replacement results</returns>
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
                resultCoefficients[coefficient.Key.yDegree] = field.Add(resultCoefficients[coefficient.Key.yDegree],
                    field.Multiply(coefficient.Value.Representation, field.Pow(xValue.Representation, coefficient.Key.xDegree)));

            return new Polynomial(field, resultCoefficients);
        }

        /// <summary>
        /// Method Calculating results of replacement bivariate polynomial <paramref name="polynomial"/> variable y by value <paramref name="yValue"/>
        /// </summary>
        /// <param name="polynomial">Modified polynomial</param>
        /// <param name="yValue">Value for y variable</param>
        /// <returns>Replacement results</returns>
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
                resultCoefficients[coefficient.Key.xDegree] = field.Add(resultCoefficients[coefficient.Key.xDegree],
                    field.Multiply(coefficient.Value.Representation, field.Pow(yValue.Representation, coefficient.Key.yDegree)));

            return new Polynomial(field, resultCoefficients);
        }

        /// <summary>
        /// Method for calculatin (<paramref name="r"/>, <paramref name="s"/>) Hasse derivative value for bivariate polynomial <paramref name="polynomial"/> at point the (<paramref name="xValue"/>, <paramref name="yValue"/>)
        /// </summary>
        /// <param name="polynomial">Polynomial which Hasse derivative'll be calculated</param>
        /// <param name="r">Hasse derivative order by x variable</param>
        /// <param name="s">Hasse derivative order by y variable</param>
        /// <param name="xValue">Value for x variable</param>
        /// <param name="yValue">Value fro y variable</param>
        /// <param name="combinationsCountCalculator">Combinations calculator contract implementation</param>
        /// <param name="combinationsCache">Cache for storing calculated numbers of combinations</param>
        /// <returns>Hasse derivative value</returns>
        public static FieldElement CalculateHasseDerivative(this BiVariablePolynomial polynomial,
            int r, int s, FieldElement xValue, FieldElement yValue,
            ICombinationsCountCalculator combinationsCountCalculator, FieldElement[][] combinationsCache = null)
        {
            var field = polynomial.Field;
            var derivativeValue = 0;

            foreach (var coefficient in polynomial)
            {
                if (coefficient.Key.xDegree < r || coefficient.Key.yDegree < s)
                    continue;

                var currentAddition = combinationsCountCalculator.Calculate(field, coefficient.Key.xDegree, r, combinationsCache).Representation;
                currentAddition = field.Multiply(currentAddition,
                    combinationsCountCalculator.Calculate(field, coefficient.Key.yDegree, s, combinationsCache).Representation);
                currentAddition = field.Multiply(currentAddition, coefficient.Value.Representation);
                currentAddition = field.Multiply(currentAddition, field.Pow(xValue.Representation, coefficient.Key.xDegree - r));
                currentAddition = field.Multiply(currentAddition, field.Pow(yValue.Representation, coefficient.Key.yDegree - s));

                derivativeValue = field.Add(derivativeValue, currentAddition);
            }

            return new FieldElement(field, derivativeValue);
        }
    }
}