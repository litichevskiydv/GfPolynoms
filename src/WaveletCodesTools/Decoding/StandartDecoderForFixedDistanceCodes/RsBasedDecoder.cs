namespace AppliedAlgebra.WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes
{
    using System;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using RsCodesTools.Decoding.StandartDecoder;
    using RsInformationPolynomialNotFoundException = RsCodesTools.Decoding.StandartDecoder.InformationPolynomialWasNotFoundException;

    /// <summary>
    /// Implementation of standart wavelet codes decoder contract based on Reed-Solomon codes standart decoder
    /// </summary>
    public class RsBasedDecoder : FixedDistanceCodesDecoderBase, IDecoder
    {
        /// <summary>
        /// Implementation of the Reed-Solomon code standard decoder
        /// </summary>
        private readonly IStandardDecoder _rsStandardDecoder;

        /// <summary>
        /// Method for decoding received codeword in the frequency domain
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="d">Code distance</param>
        /// <param name="generationPolynomialLeadZeroValuesCount">Generating polynomial's first zeros count on received points</param>
        /// <param name="decodedCodeword">Received codeword for decoding</param>
        /// <param name="errorsCount">Number of errors in received codeword</param>
        /// <returns>Decoding result in frequency domain</returns>
        private Polynomial GetFrequencyDecodingResults(int n, int d, int generationPolynomialLeadZeroValuesCount,
            Tuple<FieldElement, FieldElement>[] decodedCodeword, int? errorsCount)
        {
            var preparedCodeword = PrepareCodewordForFrequenceDecoding(n, generationPolynomialLeadZeroValuesCount, decodedCodeword);
            try
            {
                return _rsStandardDecoder.Decode(n, n - d + 1, preparedCodeword, errorsCount);
            }
            catch (RsInformationPolynomialNotFoundException e)
            {
                throw new InformationPolynomialWasNotFoundException("No frequency decoding result available", e);
            }
        }

        /// <summary>
        /// Method for performing list decoding of the wavelet code codeword
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="d">Code distance</param>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="errorsCount">Number of errors in received codeword</param>
        /// <returns>Decoding result</returns>
        public Polynomial Decode(int n, int k, int d, Polynomial generatingPolynomial, Tuple<FieldElement, FieldElement>[] decodedCodeword, int? errorsCount = null)
        {
            if (n <= 0)
                throw new ArgumentException(nameof(n));
            if (k <= 0 || k >= n)
                throw new ArithmeticException(nameof(k));
            if (generatingPolynomial == null)
                throw new ArgumentNullException(nameof(generatingPolynomial));
            if (decodedCodeword == null)
                throw new ArgumentNullException(nameof(decodedCodeword));
            if (decodedCodeword.Length != n)
                throw new AggregateException(nameof(decodedCodeword));

            var leadZerosCount = CalculateGeneratingPolynomialLeadZeroValuesCount(generatingPolynomial, decodedCodeword);

            var frequencyDecodingResult = GetFrequencyDecodingResults(n, d, leadZerosCount, decodedCodeword, errorsCount);

            var decodingResultInTimeDomain = ComputeTimeDecodingResult(
                n, k, d,
                generatingPolynomial, leadZerosCount,
                decodedCodeword, frequencyDecodingResult);
            if(decodingResultInTimeDomain == null)
                throw new InformationPolynomialWasNotFoundException();
            return decodingResultInTimeDomain;
        }

        /// <summary>
        /// Constructor for initializing internal structures of the decoder
        /// </summary>
        /// <param name="rsStandardDecoder">Implementation of the Reed-Solomon code standard decoder</param>
        /// <param name="linearSystemSolver">Implementation of the linear equations system solver</param>
        public RsBasedDecoder(IStandardDecoder rsStandardDecoder, ILinearSystemSolver linearSystemSolver) : base(linearSystemSolver)
        {
            if (rsStandardDecoder == null)
                throw new ArgumentNullException(nameof(rsStandardDecoder));

            _rsStandardDecoder = rsStandardDecoder;
        }
    }
}