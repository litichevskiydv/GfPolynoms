namespace AppliedAlgebra.WaveletCodesTools.Decoding
{
    using System;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Class with basic methods for decoding wavelet codes with fixed code distance
    /// </summary>
    public abstract class FixedDistanceCodesDecoderBase
    {
        /// <summary>
        /// Implementation of the linear equations system solver
        /// </summary>
        private readonly ILinearSystemSolver _linearSystemSolver;

        /// <summary>
        /// Method for calculating generation plynomial <paramref name="generatingPolynomial"/> first zeros count on passing points <paramref name="decodedCodeword"/>
        /// </summary>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="decodedCodeword">Received codeword from which points'll be taken</param>
        protected int CalculateGeneratingPolynomialLeadZeroValuesCount(Polynomial generatingPolynomial,
            Tuple<FieldElement, FieldElement>[] decodedCodeword)
        {
            var count = 0;
            for (;
                count < decodedCodeword.Length && generatingPolynomial.Evaluate(decodedCodeword[count].Item1.Representation) == 0;
                count++)
            {
            }
            return count;
        }

        /// <summary>
        /// Method for preparing received  codeword for frequency decoding
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="generationPolynomialLeadZeroValuesCount">Generating polynomial's first zeros count on received points</param>
        /// <param name="decodedCodeword">Received codeword for decoding</param>
        /// <returns>Prepared codeword</returns>
        protected Tuple<FieldElement, FieldElement>[] PrepareCodewordForFrequenceDecoding(
            int n, int generationPolynomialLeadZeroValuesCount, 
            Tuple<FieldElement, FieldElement>[] decodedCodeword)
        {
            var field = decodedCodeword[0].Item1.Field;
            var correction = new FieldElement(field, n % field.Characteristic);

            var preparedCodeword = new Tuple<FieldElement, FieldElement>[n];
            for (var i = 0; i < n; i++)
            {
                var inversedSample = FieldElement.InverseForMultiplication(decodedCodeword[i].Item1);
                preparedCodeword[i] = new Tuple<FieldElement, FieldElement>(inversedSample,
                    decodedCodeword[i].Item2 * correction * FieldElement.Pow(decodedCodeword[i].Item1, generationPolynomialLeadZeroValuesCount));
            }

            return preparedCodeword;
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
        /// <param name="frequencyDecodingResult">Decoding result in the frequency domain</param>
        /// <returns>Decoding result in the time domain</returns>
        protected Polynomial ComputeTimeDecodingResult(int n, int k, int d,
            Polynomial generatingPolynomial, int generationPolynomialLeadZeroValuesCount,
            Tuple<FieldElement, FieldElement>[] decodedCodeword, Polynomial frequencyDecodingResult)
        {
            var field = generatingPolynomial.Field;

            var linearSystemMatrix = new FieldElement[n - d + 1, k];
            for (var i = 0; i < n - d + 1; i++)
            {
                var sample = decodedCodeword[i + generationPolynomialLeadZeroValuesCount].Item1;
                var sampleSqr = sample * sample;
                var samplePower = field.One();
                var correction = new FieldElement(field, generatingPolynomial.Evaluate(sample.Representation));
                for (var j = 0; j < k; j++)
                {
                    linearSystemMatrix[i, j] = samplePower * correction;
                    samplePower *= sampleSqr;
                }
            }

            var valuesVector = new FieldElement[n - d + 1];
            for (var i = 0; i <= frequencyDecodingResult.Degree; i++)
                valuesVector[i] = new FieldElement(field, frequencyDecodingResult[i]);
            for (var i = frequencyDecodingResult.Degree + 1; i < n - d + 1; i++)
                valuesVector[i] = field.Zero();

            var systemSolution = _linearSystemSolver.Solve(linearSystemMatrix, valuesVector);
            return systemSolution.IsEmpty == false
                ? new Polynomial(field, systemSolution.VariablesValues.Select(x => x.Representation).ToArray())
                : null;

        }

        /// <summary>
        /// Constructor for initialization of internal objects
        /// </summary>
        /// <param name="linearSystemSolver">Implementation of the linear equations system solver</param>
        protected FixedDistanceCodesDecoderBase(ILinearSystemSolver linearSystemSolver)
        {
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _linearSystemSolver = linearSystemSolver;
        }
    }
}