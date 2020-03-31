﻿namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public static class PolynomialExtensions
    {
        /// <summary>
        /// Converts source polynomial <paramref name="polynomial"/> to form p(x) = p_e(x^2)+x*p_o(x^2)
        /// </summary>
        /// <param name="polynomial">Polynomial for processing</param>
        /// <returns>Pair (p_e(x), p_o(x))</returns>
        public static Tuple<Polynomial, Polynomial> GetPolyphaseComponents(this Polynomial polynomial)
        {
            if(polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            var evenDegreesCoefficients = new List<int>();
            var oddDegreesCoefficients = new List<int>();

            for (var i = 0; i <= polynomial.Degree; i++)
                if (i%2 == 0)
                    evenDegreesCoefficients.Add(polynomial[i]);
                else
                    oddDegreesCoefficients.Add(polynomial[i]);

            return new Tuple<Polynomial, Polynomial>(new Polynomial(polynomial.Field, evenDegreesCoefficients.ToArray()),
                new Polynomial(polynomial.Field, oddDegreesCoefficients.ToArray()));
        }

        /// <summary>
        /// Creates polynomial p(x) = p_e(x^2)+x*p_o(x^2)
        /// </summary>
        /// <param name="evenComponent">p_e(x)</param>
        /// <param name="oddComponent">p_o(x)</param>
        /// <returns>Reconstructed polynomial</returns>
        public static Polynomial CreateFormPolyphaseComponents(Polynomial evenComponent, Polynomial oddComponent)
        {
            if(evenComponent == null)
                throw new ArgumentNullException(nameof(evenComponent));
            if(oddComponent == null)
                throw new ArgumentNullException(nameof(oddComponent));
            if(evenComponent.Field.Equals(oddComponent.Field) == false)
                throw new ArgumentException("Fields mismatch");

            return evenComponent.RaiseVariableDegree(2) + (oddComponent.RaiseVariableDegree(2) >> 1);
        }

        /// <summary>
        /// Method for checking if <paramref name="polynomial"/> is a monomial
        /// </summary>
        /// <param name="polynomial">Polynomial for verification</param>
        /// <returns>Verification result</returns>
        public static bool IsMonomial(this Polynomial polynomial)
        {
            var isMonomial = true;
            for (var i = 0; i < polynomial.Degree && isMonomial; i++)
                isMonomial = polynomial[i] == 0;
            return isMonomial;
        }

        /// <summary>
        /// Method calculates <paramref name="polynomial"/> spectrum
        /// </summary>
        public static FieldElement[] GetSpectrum(this Polynomial polynomial, int? expectedDegree = null)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (expectedDegree.HasValue && expectedDegree.Value < 0)
                throw new ArgumentException($"{nameof(expectedDegree)} must be not negative");
            if (expectedDegree.HasValue && expectedDegree.Value < polynomial.Degree)
                throw new ArgumentException($"{nameof(expectedDegree)} must be greater or equal polynomial's degree");

            return polynomial.GetCoefficients(expectedDegree).GetSpectrum();
        }
    }
}