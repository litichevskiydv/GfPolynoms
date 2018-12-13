namespace AppliedAlgebra.GfPolynoms.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class with extensions for polynomial
    /// </summary>
    public static class PolynomialExtensions
    {
        /// <summary>
        /// Method for performing replacing x->x^<paramref name="variableDegree"/>
        /// </summary>
        /// <param name="polynomial">Transformed polynomial</param>
        /// <param name="variableDegree">New variable degree</param>
        /// <returns>Replacement result</returns>
        public static Polynomial RaiseVariableDegree(this Polynomial polynomial, int variableDegree)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (variableDegree < 1)
                throw new ArgumentException(nameof(variableDegree));

            if (variableDegree == 1)
                return new Polynomial(polynomial);

            var resultCoefficients = new int[polynomial.Degree*variableDegree + 1];
            for (var i = 0; i <= polynomial.Degree; i++)
                resultCoefficients[i*variableDegree] = polynomial[i];
            return new Polynomial(polynomial.Field, resultCoefficients);
        }

        /// <summary>
        /// Method for obtaining polynomial coefficients
        /// </summary>
        public static FieldElement[] GetCoefficients(this Polynomial polynomial)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            var coefficients = new FieldElement[polynomial.Degree + 1];
            for (var i = 0; i <= polynomial.Degree; i++)
                coefficients[i] = polynomial.Field.CreateElement(polynomial[i]);

            return coefficients;
        }
    }
}