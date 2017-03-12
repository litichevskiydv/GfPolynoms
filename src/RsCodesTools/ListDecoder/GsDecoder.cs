namespace AppliedAlgebra.RsCodesTools.ListDecoder
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;
    using GsDecoderDependencies.InterpolationPolynomialBuilder;
    using GsDecoderDependencies.InterpolationPolynomialFactorisator;

    public class GsDecoder : IListDecoder
    {
        private readonly IInterpolationPolynomialBuilder _interpolationPolynomialBuilder;
        private readonly IInterpolationPolynomialFactorizator _interpolationPolynomialFactorizator;

        private static Polynomial[] SelectCorrectInformationPolynomials(IEnumerable<Polynomial> possiblePolynomials,
            IReadOnlyList<Tuple<FieldElement, FieldElement>> decodedCodeword, int minCorrectValuesCount)
        {
            var correctPolynomials = new List<Polynomial>();

            foreach (var possiblePolynomial in possiblePolynomials)
            {
                var matchesCount = 0;
                for (var i = 0; i < decodedCodeword.Count && matchesCount < minCorrectValuesCount; i++)
                    if (possiblePolynomial.Evaluate(decodedCodeword[i].Item1.Representation) == decodedCodeword[i].Item2.Representation)
                        matchesCount++;

                if (matchesCount == minCorrectValuesCount)
                    correctPolynomials.Add(possiblePolynomial);
            }

            return correctPolynomials.ToArray();
        }

        public Polynomial[] Decode(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount)
        {
            if (n <= 0)
                throw new ArgumentException(nameof(n));
            if (k <= 0 || k >= n)
                throw new ArithmeticException(nameof(k));
            if (minCorrectValuesCount <= 0 || minCorrectValuesCount*minCorrectValuesCount <= n*(k - 1))
                throw new ArgumentException($"{nameof(minCorrectValuesCount)} is to small");
            if(decodedCodeword == null)
                throw new ArgumentNullException(nameof(decodedCodeword));
            if (decodedCodeword.Length != n)
                throw new AggregateException(nameof(decodedCodeword));

            var decrementedK = k - 1;

            var temp = minCorrectValuesCount*minCorrectValuesCount - n*decrementedK;
            var rootsMultiplicity = 1 + (int) Math.Floor((n*decrementedK + Math.Sqrt(n*n*decrementedK*decrementedK + 4d*temp))/(2d*temp));
            var maxWeightedDegree = rootsMultiplicity*minCorrectValuesCount - 1;
            var interpolationPolynomial = _interpolationPolynomialBuilder.Build(new Tuple<int, int>(1, decrementedK), maxWeightedDegree,
                decodedCodeword, rootsMultiplicity);

            var possibleInformationPolynomials = _interpolationPolynomialFactorizator.Factorize(interpolationPolynomial, decrementedK);

            return SelectCorrectInformationPolynomials(possibleInformationPolynomials, decodedCodeword, minCorrectValuesCount);
        }

        public GsDecoder(IInterpolationPolynomialBuilder interpolationPolynomialBuilder, IInterpolationPolynomialFactorizator interpolationPolynomialFactorizator)
        {
            if (interpolationPolynomialBuilder == null)
                throw new ArgumentNullException(nameof(interpolationPolynomialBuilder));
            if (interpolationPolynomialFactorizator == null)
                throw new ArgumentNullException(nameof(interpolationPolynomialFactorizator));

            _interpolationPolynomialBuilder = interpolationPolynomialBuilder;
            _interpolationPolynomialFactorizator = interpolationPolynomialFactorizator;
        }
    }
}