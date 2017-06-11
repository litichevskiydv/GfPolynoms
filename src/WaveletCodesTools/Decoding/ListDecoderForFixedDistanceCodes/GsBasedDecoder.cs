namespace AppliedAlgebra.WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GsBasedDecoderDependencies;

    /// <summary>
    /// Implementation of the wavelet code list decoding contract based on Guruswami–Sudan algorithm
    /// </summary>
    public class GsBasedDecoder : FixedDistanceCodesDecoderBase, IListDecoder
    {
        /// <summary>
        /// Implementation of the Reed-Solomon code list decoding contract
        /// </summary>
        private readonly RsCodesTools.ListDecoder.IListDecoder _rsListDecoder;

        /// <summary>
        /// Implementation of telemetry receiver's contract
        /// </summary>
        public IGsBasedDecoderTelemetryCollector TelemetryCollector { get; set; }

        /// <summary>
        /// Method for decoding received codeword in the frequency domain
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="d">Code distance</param>
        /// <param name="generationPolynomialLeadZeroValuesCount">Generating polynomial's first zeros count on received points</param>
        /// <param name="decodedCodeword">Received codeword for decoding</param>
        /// <param name="minCorrectValuesCount">Minimum number of valid values</param>
        /// <returns>Decoding result in frequency domain</returns>
        private Polynomial[] GetFrequencyDecodingResults(int n, int d, int generationPolynomialLeadZeroValuesCount,
            Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount)
        {
            var preparedCodeword = PrepareCodewordForFrequenceDecoding(n, generationPolynomialLeadZeroValuesCount, decodedCodeword);
            return _rsListDecoder.Decode(n, n - d + 1, preparedCodeword, minCorrectValuesCount);
        }

        /// <summary>
        /// Method for obtaining decoding result in time domain
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="d">Code distance</param>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="generationPolynomialLeadZeroValuesCount">Generating polynomial's first zeros count on received points</param>
        /// <param name="decodedCodeword">Received codeword for decoding</param>
        /// <param name="frequencyDecodingResults">Decoding result in the frequency domain</param>
        /// <returns>Decoding result in the time domain</returns>
        private Polynomial[] SelectCorrectInformationPolynomials(int n, int k, int d, Polynomial generatingPolynomial, int generationPolynomialLeadZeroValuesCount,
            Tuple<FieldElement, FieldElement>[] decodedCodeword, IEnumerable<Polynomial> frequencyDecodingResults)
        {
            return
                frequencyDecodingResults
                    .Select(x =>
                                ComputeTimeDecodingResult(
                                    n, k, d,
                                    generatingPolynomial, generationPolynomialLeadZeroValuesCount,
                                    decodedCodeword, x))
                    .Where(x => x != null)
                    .ToArray();
        }

        /// <summary>
        /// Method for performing list decoding of the wavelet code codeword
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="d">Code distance</param>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="minCorrectValuesCount">Minimum number of valid values</param>
        /// <returns>Decoding result</returns>
        public Polynomial[] Decode(int n, int k, int d, Polynomial generatingPolynomial, 
            Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount)
        {
            if (n <= 0)
                throw new ArgumentException(nameof(n));
            if (k <= 0 || k >= n)
                throw new ArithmeticException(nameof(k));
            if(generatingPolynomial == null)
                throw new ArgumentNullException(nameof(generatingPolynomial));
            if (decodedCodeword == null)
                throw new ArgumentNullException(nameof(decodedCodeword));
            if (decodedCodeword.Length != n)
                throw new AggregateException(nameof(decodedCodeword));

            var leadZerosCount = CalculateGeneratingPolynomialLeadZeroValuesCount(generatingPolynomial, decodedCodeword);

            var frequencyDecodingResults = GetFrequencyDecodingResults(n, d, leadZerosCount, decodedCodeword, minCorrectValuesCount);

            var resultList = SelectCorrectInformationPolynomials(n, k, d, generatingPolynomial, leadZerosCount, decodedCodeword,
                frequencyDecodingResults);
            TelemetryCollector?.ReportDecodingListsSizes(decodedCodeword, frequencyDecodingResults.Length, resultList.Length);
            return resultList;
        }

        /// <summary>
        /// Constructor for creation instance of the implementation of the wavelet code list decoding contract based on Guruswami–Sudan algorithm
        /// </summary>
        /// <param name="rsListDecoder">Implementation of the Reed-Solomon code list decoding contract</param>
        /// <param name="linearSystemSolver">Implementation of the linear equations system solver</param>
        public GsBasedDecoder(RsCodesTools.ListDecoder.IListDecoder rsListDecoder, ILinearSystemSolver linearSystemSolver) : base(linearSystemSolver)
        {
            if (rsListDecoder == null)
                throw new ArgumentNullException(nameof(rsListDecoder));

            _rsListDecoder = rsListDecoder;
        }
    }
}