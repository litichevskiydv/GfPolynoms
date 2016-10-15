namespace AppliedAlgebra.RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.BiVariablePolynomials;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class RrFactorizator : IInterpolationPolynomialFactorizator
    {
        private readonly Tuple<int, int> _zeroMonomial;

        private static IEnumerable<FieldElement> FindPolynomialRoots(Polynomial polynomial)
        {
            var roots = new List<FieldElement>();
            for (var i = 0; i < polynomial.Field.Order; i++)
                if (polynomial.Evaluate(i) == 0)
                    roots.Add(new FieldElement(polynomial.Field, i));

            return roots;
        }

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

        public Polynomial[] Factorize(BiVariablePolynomial interpolationPolynomial, int maxFactorDegree)
        {
            var xSubstitution = new BiVariablePolynomial(interpolationPolynomial.Field)
                                {
                                    [new Tuple<int, int>(1, 0)] = interpolationPolynomial.Field.One()
                                };
            var ySubstitution = new BiVariablePolynomial(interpolationPolynomial.Field)
                                {
                                    [new Tuple<int, int>(0, 0)] = interpolationPolynomial.Field.One(),
                                    [new Tuple<int, int>(1, 1)] = interpolationPolynomial.Field.One()
                                };

            var factors = new HashSet<Polynomial>();
            var currentFactorCoefficients = new Stack<int>();

            Factorize(interpolationPolynomial, maxFactorDegree,
                xSubstitution, ySubstitution,
                factors, currentFactorCoefficients);

            return factors.ToArray();
        }

        public RrFactorizator()
        {
            _zeroMonomial = new Tuple<int, int>(0, 0);
        }
    }
}