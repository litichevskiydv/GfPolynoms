﻿namespace AppliedAlgebra.RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator
{
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.BiVariablePolynomials;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Implementation of bivariate polynomials factorization contract
    /// </summary>
    public class RrFactorizator : IInterpolationPolynomialFactorizator
    {
        private readonly (int xDegee, int yDegree) _zeroMonomial;

        private static IEnumerable<FieldElement> FindPolynomialRoots(Polynomial polynomial)
        {
            var roots = new List<FieldElement>();
            for (var i = 0; i < polynomial.Field.Order; i++)
                if (polynomial.Evaluate(i) == 0)
                    roots.Add(new FieldElement(polynomial.Field, i));

            return roots;
        }

        /// <summary>
        /// Method for recursive search of bivariate polynomial's <paramref name="polynomial"/> factors with max degree <paramref name="maxFactorDegree"/>
        /// </summary>
        /// <param name="polynomial">Bivariate polynomial for factorization</param>
        /// <param name="maxFactorDegree">Maximum degree of factor</param>
        /// <param name="xSubstitution">Substitution for x variable</param>
        /// <param name="ySubstitution">Substitution for y variable</param>
        /// <param name="factors">Array of finded factors</param>
        /// <param name="currentFactorCoefficients">Stack with current factor's coefficients</param>
        private void Factorize(BiVariablePolynomial polynomial, int maxFactorDegree,
            BiVariablePolynomial xSubstitution, BiVariablePolynomial ySubstitution,
            ISet<Polynomial> factors, Stack<int> currentFactorCoefficients)
        {
            if (polynomial.EvaluateY(polynomial.Field.Zero()).IsZero)
                factors.Add(new Polynomial(polynomial.Field, currentFactorCoefficients.Reverse().ToArray()));

            if (currentFactorCoefficients.Count > maxFactorDegree)
                return;
            var roots = FindPolynomialRoots(polynomial.EvaluateX(polynomial.Field.Zero()));
            foreach (var root in roots)
            {
                currentFactorCoefficients.Push(root.Representation);
                ySubstitution[_zeroMonomial] = root;
                Factorize(polynomial
                    .PerformVariablesSubstitution(xSubstitution, ySubstitution)
                    .DivideByMaxPossibleXDegree(), maxFactorDegree,
                    xSubstitution, ySubstitution,
                    factors, currentFactorCoefficients);

                currentFactorCoefficients.Pop();
            }
        }

        /// <summary>
        /// Method for finding bivariate polynomial's <paramref name="interpolationPolynomial"/> factors with max degree <paramref name="maxFactorDegree"/>
        /// </summary>
        /// <param name="interpolationPolynomial">Bivariate polynomial for factorization</param>
        /// <param name="maxFactorDegree">Maximum degree of factor</param>
        /// <returns>Array of finded factors</returns>
        public Polynomial[] Factorize(BiVariablePolynomial interpolationPolynomial, int maxFactorDegree)
        {
            var xSubstitution = new BiVariablePolynomial(interpolationPolynomial.Field)
                                {
                                    [(1, 0)] = interpolationPolynomial.Field.One()
                                };
            var ySubstitution = new BiVariablePolynomial(interpolationPolynomial.Field)
                                {
                                    [(0, 0)] = interpolationPolynomial.Field.One(),
                                    [(1, 1)] = interpolationPolynomial.Field.One()
                                };

            var factors = new HashSet<Polynomial>();
            var currentFactorCoefficients = new Stack<int>();

            Factorize(interpolationPolynomial, maxFactorDegree,
                xSubstitution, ySubstitution,
                factors, currentFactorCoefficients);

            return factors.ToArray();
        }

        /// <summary>
        /// Constructor for creation implementation of bivariate polynomials factorization contract
        /// </summary>
        public RrFactorizator()
        {
            _zeroMonomial = (0, 0);
        }
    }
}