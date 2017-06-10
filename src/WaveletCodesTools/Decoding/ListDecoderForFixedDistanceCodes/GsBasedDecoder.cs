namespace AppliedAlgebra.WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GsBasedDecoderDependencies;

    /// <summary>
    /// Implementation of the wavelet code list decoding contract based on Guruswami–Sudan algorithm
    /// </summary>
    public class GsBasedDecoder : IListDecoder
    {
        /// <summary>
        /// Implementation of the Reed-Solomon code list decoding contract
        /// </summary>
        private readonly RsCodesTools.ListDecoder.IListDecoder _rsListDecoder;
        /// <summary>
        /// Implementation of the linear equations system solver
        /// </summary>
        private readonly ILinearSystemSolver _linearSystemSolver;

        /// <summary>
        /// Implementation of telemetry receiver's contract
        /// </summary>
        public IGsBasedDecoderTelemetryCollector TelemetryCollector { get; set; }

        /// <summary>
        /// Method for calculating generation plynomial <paramref name="generatingPolynomial"/> first zeros count on passing points <paramref name="decodedCodeword"/>
        /// </summary>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="decodedCodeword">Received codeword from which points'll be taken</param>
        private static int CalculateGeneratingPolynomialLeadZeroValuesCount(Polynomial generatingPolynomial,
            IReadOnlyList<Tuple<FieldElement, FieldElement>> decodedCodeword)
        {
            var count = 0;
            for (;
                count < decodedCodeword.Count && generatingPolynomial.Evaluate(decodedCodeword[count].Item1.Representation) == 0;
                count++)
            {
            }
            return count;
        }

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
            IReadOnlyList<Tuple<FieldElement, FieldElement>> decodedCodeword, int minCorrectValuesCount)
        {
            var field = decodedCodeword[0].Item1.Field;

            var correction = new FieldElement(field, n%field.Characteristic);
            var preparedCodeword = new Tuple<FieldElement, FieldElement>[n];
            for (var i = 0; i < n; i++)
            {
                var inversedSample = FieldElement.InverseForMultiplication(decodedCodeword[i].Item1);
                preparedCodeword[i] = new Tuple<FieldElement, FieldElement>(inversedSample,
                    decodedCodeword[i].Item2*correction*FieldElement.Pow(decodedCodeword[i].Item1, generationPolynomialLeadZeroValuesCount));
            }
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
            IReadOnlyList<Tuple<FieldElement, FieldElement>> decodedCodeword, IEnumerable<Polynomial> frequencyDecodingResults)
        {
            var correctPolynomials = new List<Polynomial>();

            var field = generatingPolynomial.Field;
            foreach (var frequencyDecodingResult in frequencyDecodingResults)
            {
                var linearSystemMatrix = new FieldElement[n - d + 1, k];
                for (var i = 0; i < n - d + 1; i++)
                {
                    var sample = decodedCodeword[i + generationPolynomialLeadZeroValuesCount].Item1;
                    var sampleSqr = sample * sample;
                    var samplePower = field.One();
                    var correction = new FieldElement(field, generatingPolynomial.Evaluate(sample.Representation));
                    for (var j = 0; j < k; j++)
                    {
                        linearSystemMatrix[i, j] = samplePower*correction;
                        samplePower *= sampleSqr;
                    }
                }

                var valuesVector = new FieldElement[n - d + 1];
                for (var i = 0; i <= frequencyDecodingResult.Degree; i++)
                    valuesVector[i] = new FieldElement(field, frequencyDecodingResult[i]);
                for (var i = frequencyDecodingResult.Degree + 1; i < n - d + 1; i++)
                    valuesVector[i] = field.Zero();

                var systemSolution = _linearSystemSolver.Solve(linearSystemMatrix, valuesVector);
                if (systemSolution.IsEmpty == false)
                    correctPolynomials.Add(new Polynomial(field, Enumerable.Select<FieldElement, int>(systemSolution.VariablesValues, x => x.Representation).ToArray()));
            }

            return correctPolynomials.ToArray();
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
        public GsBasedDecoder(RsCodesTools.ListDecoder.IListDecoder rsListDecoder, ILinearSystemSolver linearSystemSolver)
        {
            if (rsListDecoder == null)
                throw new ArgumentNullException(nameof(rsListDecoder));
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _rsListDecoder = rsListDecoder;
            _linearSystemSolver = linearSystemSolver;
        }
    }
}