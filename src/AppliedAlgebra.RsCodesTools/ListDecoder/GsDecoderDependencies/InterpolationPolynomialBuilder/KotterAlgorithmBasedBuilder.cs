namespace AppliedAlgebra.RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.BiVariablePolynomials;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class KotterAlgorithmBasedBuilder : IInterpolationPolynomialBuilder
    {
        private readonly Tuple<int, int> _zeroMonomial;
        private readonly ICombinationsCountCalculator _combinationsCountCalculator;

        private static int FindMinimumIndexByLeadMonomial<TSourse>(ICollection<TSourse> sourse, 
            Func<int, Tuple<int, int>> leadMonomialSelector, IComparer<Tuple<int, int>> monomialsComparer)
        {
            var minimumIndex = 0;
            var minLeadMonomial = leadMonomialSelector(0);

            for (var i = 1; i < sourse.Count; i++)
            {
                var leadMonomial = leadMonomialSelector(i);
                if (monomialsComparer.Compare(leadMonomial, minLeadMonomial) < 0)
                {
                    minLeadMonomial = leadMonomial;
                    minimumIndex = i;
                }
            }

            return minimumIndex;
        }

        private static int CalculateMonomialWeight(Tuple<int, int> monomial, Tuple<int, int> degreeWeight)
        {
            return monomial.Item1*degreeWeight.Item1 + monomial.Item2*degreeWeight.Item2;
        }

        public BiVariablePolynomial Build(Tuple<int, int> degreeWeight, int maxWeightedDegree,
            Tuple<FieldElement, FieldElement>[] roots, int rootsMultiplicity)
        {
            if (degreeWeight == null)
                throw new ArgumentNullException(nameof(degreeWeight));
            if (roots == null)
                throw new ArgumentNullException(nameof(roots));
            if (roots.Length == 0)
                throw new ArgumentException($"{nameof(roots)} is empty");

            var field = roots[0].Item1.Field;
            var maxXDegree = maxWeightedDegree/degreeWeight.Item1;
            var maxYDegree = maxWeightedDegree/degreeWeight.Item2;
            
            var combinationsCache = new FieldElement[Math.Max(maxXDegree, maxYDegree) + 1][].MakeSquare();
            var transformationMultiplier = new BiVariablePolynomial(field) {[new Tuple<int, int>(1, 0)] = field.One()};
            var monomialsComparer = new BiVariableMonomialsComparer(degreeWeight);

            var buildingPolynomials = new BiVariablePolynomial[maxYDegree + 1];
            var leadMonomials = new Tuple<int, int>[maxYDegree + 1];
            for (var i = 0; i < buildingPolynomials.Length; i++)
            {
                var leadMonomial = new Tuple<int, int>(0, i);
                buildingPolynomials[i] = new BiVariablePolynomial(field, (maxXDegree + 1)*(maxYDegree + 1)) {[leadMonomial] = field.One()};
                leadMonomials[i] = leadMonomial;
            }

            foreach (var root in roots)
                for (var r = 0; r < rootsMultiplicity; r++)
                    for (var s = 0; r + s < rootsMultiplicity; s++)
                    {
                        var anyCompatiblePolynomialFound = false;
                        var nonZeroDerivatives = new List<Tuple<int, FieldElement>>();
                        for (var i = 0; i < buildingPolynomials.Length; i++)
                        {
                            if (CalculateMonomialWeight(leadMonomials[i], degreeWeight) > maxWeightedDegree)
                                continue;

                            anyCompatiblePolynomialFound = true;
                            var hasseDerivative = buildingPolynomials[i].CalculateHasseDerivative(r, s, root.Item1, root.Item2,
                                _combinationsCountCalculator, combinationsCache);
                            if (hasseDerivative.Representation != 0)
                                nonZeroDerivatives.Add(new Tuple<int, FieldElement>(i, hasseDerivative));
                        }

                        if (anyCompatiblePolynomialFound == false)
                            throw new NonTrivialPolynomialNotFoundException();
                        if (nonZeroDerivatives.Count == 0)
                            continue;

                        var minimumIndex = FindMinimumIndexByLeadMonomial(nonZeroDerivatives,
                            i => leadMonomials[nonZeroDerivatives[i].Item1], monomialsComparer);
                        var minimumPolynomialIndex = nonZeroDerivatives[minimumIndex].Item1;
                        var minimumPolynomial = buildingPolynomials[minimumPolynomialIndex];
                        var minimumDerivative = nonZeroDerivatives[minimumIndex].Item2;

                        for (var i = 0; i < nonZeroDerivatives.Count; i++)
                            if (i != minimumIndex)
                                buildingPolynomials[nonZeroDerivatives[i].Item1]
                                    .Subtract(nonZeroDerivatives[i].Item2.Divide(minimumDerivative), minimumPolynomial);
                        transformationMultiplier[_zeroMonomial] = FieldElement.InverseForAddition(root.Item1);
                        minimumPolynomial
                            .Multiply(minimumDerivative*transformationMultiplier);
                        leadMonomials[minimumPolynomialIndex] = new Tuple<int, int>(leadMonomials[minimumPolynomialIndex].Item1 + 1,
                            leadMonomials[minimumPolynomialIndex].Item2);
                    }

            var resultPolynomialIndex = FindMinimumIndexByLeadMonomial(buildingPolynomials, i => leadMonomials[i], monomialsComparer);
            if (CalculateMonomialWeight(leadMonomials[resultPolynomialIndex], degreeWeight) > maxWeightedDegree)
                throw new NonTrivialPolynomialNotFoundException();
            return buildingPolynomials[resultPolynomialIndex];
        }

        public KotterAlgorithmBasedBuilder(ICombinationsCountCalculator combinationsCountCalculator)
        {
            if(combinationsCountCalculator == null)
                throw new ArgumentNullException(nameof(combinationsCountCalculator));

            _combinationsCountCalculator = combinationsCountCalculator;

            _zeroMonomial = new Tuple<int, int>(0, 0);
        }
    }
}