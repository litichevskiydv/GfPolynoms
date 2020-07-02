namespace AppliedAlgebra.RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.BiVariablePolynomials;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Implementation of interpolation polynomials builder based on Kotter's algorithm
    /// </summary>
    public class KotterAlgorithmBasedBuilder : IInterpolationPolynomialBuilder
    {
        private readonly (int xDegree, int yDegree) _zeroMonomial;
        /// <summary>
        /// Implementation of combinations count calculator contract
        /// </summary>
        private readonly ICombinationsCountCalculator _combinationsCountCalculator;

        /// <summary>
        /// Method for finding index of element with minimum lead monomial
        /// </summary>
        /// <typeparam name="TSource">Type of collection element</typeparam>
        /// <param name="source">Collection from which minimum lead monomial will be chosen</param>
        /// <param name="leadMonomialSelector">Selector of lead monomial from collection element</param>
        /// <param name="monomialsComparer">Bivariate monomials comparer</param>
        /// <returns>Found index</returns>
        private static int FindMinimumIndexByLeadMonomial<TSource>(
            ICollection<TSource> source,
            Func<int, (int xDegree, int yDegree)> leadMonomialSelector,
            IComparer<(int xDegree, int yDegree)> monomialsComparer
        )
        {
            var minimumIndex = 0;
            var minLeadMonomial = leadMonomialSelector(0);

            for (var i = 1; i < source.Count; i++)
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

        /// <summary>
        /// Method for calculating monomial's <paramref name="monomial"/> degree weight
        /// </summary>
        /// <param name="monomial">Bivariate monomial for degree weight calculation</param>
        /// <param name="degreeWeight">Weight of bivariate monomials degree</param>
        /// <returns>Calculated degree weight</returns>
        private static int CalculateMonomialWeight((int xDegree, int yDegree) monomial, (int xWeight, int yWeight) degreeWeight)
        {
            return monomial.xDegree * degreeWeight.xWeight + monomial.yDegree * degreeWeight.yWeight;
        }

        /// <summary>
        /// Method for bivariate interpolation polynomial building
        /// </summary>
        /// <param name="degreeWeight">Weight of bivariate monomials degree</param>
        /// <param name="maxWeightedDegree">Maximum value of bivariate monomial degree</param>
        /// <param name="roots">Roots of the interpolation polynomial</param>
        /// <param name="rootsMultiplicity">Multiplicity of bivariate polynomial's roots</param>
        /// <returns>Builded interpolation polynomial</returns>
        public BiVariablePolynomial Build(
            (int xWeight, int yWeight) degreeWeight, 
            int maxWeightedDegree,
            (FieldElement xValue, FieldElement yValue)[] roots, 
            int rootsMultiplicity
            )
        {
            if (roots == null)
                throw new ArgumentNullException(nameof(roots));
            if (roots.Length == 0)
                throw new ArgumentException($"{nameof(roots)} is empty");

            var field = roots[0].xValue.Field;
            var maxXDegree = maxWeightedDegree/degreeWeight.xWeight;
            var maxYDegree = maxWeightedDegree/degreeWeight.yWeight;
            
            var combinationsCache = new FieldElement[Math.Max(maxXDegree, maxYDegree) + 1][].MakeSquare();
            var transformationMultiplier = new BiVariablePolynomial(field) {[(1, 0)] = field.One()};
            var monomialsComparer = new BiVariableMonomialsComparer(degreeWeight);

            var buildingPolynomials = new BiVariablePolynomial[maxYDegree + 1];
            var leadMonomials = new (int xDegree, int yDegree)[maxYDegree + 1];
            for (var i = 0; i < buildingPolynomials.Length; i++)
            {
                var leadMonomial = (0, i);
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
                            var hasseDerivative = buildingPolynomials[i].CalculateHasseDerivative(r, s, root.xValue, root.yValue,
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
                        transformationMultiplier[_zeroMonomial] = -root.xValue;
                        minimumPolynomial
                            .Multiply(minimumDerivative*transformationMultiplier);
                        leadMonomials[minimumPolynomialIndex] = (
                            leadMonomials[minimumPolynomialIndex].xDegree + 1,
                            leadMonomials[minimumPolynomialIndex].yDegree
                        );
                    }

            var resultPolynomialIndex = FindMinimumIndexByLeadMonomial(buildingPolynomials, i => leadMonomials[i], monomialsComparer);
            if (CalculateMonomialWeight(leadMonomials[resultPolynomialIndex], degreeWeight) > maxWeightedDegree)
                throw new NonTrivialPolynomialNotFoundException();
            return buildingPolynomials[resultPolynomialIndex];
        }

        /// <summary>
        /// Constructor for creating interpolation polynomial builder based on Kotter's algorithm
        /// </summary>
        /// <param name="combinationsCountCalculator">Implementation of combinations count calculator contract</param>
        public KotterAlgorithmBasedBuilder(ICombinationsCountCalculator combinationsCountCalculator)
        {
            if(combinationsCountCalculator == null)
                throw new ArgumentNullException(nameof(combinationsCountCalculator));

            _combinationsCountCalculator = combinationsCountCalculator;

            _zeroMonomial = (0, 0);
        }
    }
}