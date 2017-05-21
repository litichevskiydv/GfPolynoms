namespace AppliedAlgebra.RsCodesTools.ListDecoder
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;
    using GsDecoderDependencies.InterpolationPolynomialBuilder;
    using GsDecoderDependencies.InterpolationPolynomialFactorisator;

    /// <summary>
    /// Implementation of Guruswami–Sudan algorithm for Reed-Solomon's code list decoding
    /// </summary>
    public class GsDecoder : IListDecoder
    {
        /// <summary>
        /// Implementation of interpolation polynomial builder contract
        /// </summary>
        private readonly IInterpolationPolynomialBuilder _interpolationPolynomialBuilder;
        /// <summary>
        /// Implementation of bivariate polynomial's factorization algorithm contract
        /// </summary>
        private readonly IInterpolationPolynomialFactorizator _interpolationPolynomialFactorizator;

        /// <summary>
        /// Method for validating decoding results
        /// </summary>
        /// <param name="possiblePolynomials">Possible decoding resultse</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="minCorrectValuesCount">Minimum number of valid values</param>
        /// <returns>Valid decoding results</returns>
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

        /// <summary>
        /// Method for performing list decoding of Reed-Solomon code codeword
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="minCorrectValuesCount">Minimum number of valid values</param>
        /// <returns>Decoding result</returns>
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

        /// <summary>
        /// Constructor for creation 
        /// </summary>
        /// <param name="interpolationPolynomialBuilder"></param>
        /// <param name="interpolationPolynomialFactorizator"></param>
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