﻿namespace AppliedAlgebra.GfPolynoms.Extensions
{
    using System;
    using System.Linq;
    using GaloisFields;

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
        public static FieldElement[] GetCoefficients(this Polynomial polynomial, int? expectedDegree = null)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if(expectedDegree.HasValue && expectedDegree.Value < 0)
                throw new ArgumentException($"{nameof(expectedDegree)} must be not negative");
            if(expectedDegree.HasValue && expectedDegree.Value < polynomial.Degree)
                throw new ArgumentException($"{nameof(expectedDegree)} must be greater or equal polynomial's degree");

            var coefficients = Enumerable.Repeat(polynomial.Field.Zero(), (expectedDegree ?? polynomial.Degree) + 1).ToArray();
            for (var i = 0; i <= polynomial.Degree; i++)
                coefficients[i] = polynomial.Field.CreateElement(polynomial[i]);

            return coefficients;
        }

        /// <summary>
        /// Method calculates <paramref name="polynomial"/> spectrum
        /// </summary>
        public static FieldElement[] GetSpectrum(this Polynomial polynomial)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            return Enumerable.Range(0, polynomial.Field.Order - 1)
                .Select(x => polynomial.Field.CreateElement(polynomial.Evaluate(polynomial.Field.PowGeneratingElement(x))))
                .ToArray();
        }

        public static Polynomial TransferToSubfield(this Polynomial polynomial, GaloisField fieldExtension)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (fieldExtension == null)
                throw new ArgumentNullException(nameof(fieldExtension));

            return new Polynomial(
                fieldExtension,
                Enumerable.Range(0, polynomial.Degree + 1)
                    .Select(i => polynomial.Field.TransferElementToSubfield(polynomial[i], fieldExtension))
                    .ToArray()
            );
        }
    }
}