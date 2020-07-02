﻿namespace AppliedAlgebra.WaveletCodesTools.Decoding
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
        /// Method for calculating generation polynomial <paramref name="generatingPolynomial"/> first zeros count on passing points <paramref name="decodedCodeword"/>
        /// </summary>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="decodedCodeword">Received codeword from which points'll be taken</param>
        protected int CalculateGeneratingPolynomialLeadZeroValuesCount(
            Polynomial generatingPolynomial,
            (FieldElement xValue, FieldElement yValue)[] decodedCodeword
        )
        {
            var count = 0;
            for (;
                count < decodedCodeword.Length && generatingPolynomial.Evaluate(decodedCodeword[count].xValue.Representation) == 0;
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
        protected (FieldElement xValue, FieldElement yValue)[] PrepareCodewordForFrequenceDecoding(
            int n,
            int generationPolynomialLeadZeroValuesCount,
            (FieldElement xValue, FieldElement yValue)[] decodedCodeword
        )
        {
            var field = decodedCodeword[0].xValue.Field;
            var correction = new FieldElement(field, n % field.Characteristic);

            var preparedCodeword = new (FieldElement xValue, FieldElement yValue)[n];
            for (var i = 0; i < n; i++)
            {
                var invertedSample = FieldElement.InverseForMultiplication(decodedCodeword[i].xValue);
                preparedCodeword[i] = (
                    invertedSample,
                    decodedCodeword[i].yValue * correction
                                              * FieldElement.Pow(decodedCodeword[i].xValue, generationPolynomialLeadZeroValuesCount)
                );
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
            (FieldElement xValue, FieldElement yValue)[] decodedCodeword, Polynomial frequencyDecodingResult)
        {
            var field = generatingPolynomial.Field;
            var firstSampleNumber = Math.Min(generationPolynomialLeadZeroValuesCount, d - 1);

            var linearSystemMatrix = new FieldElement[n - d + 1, k];
            for (var i = 0; i < n - d + 1; i++)
            {
                var sample = decodedCodeword[i + firstSampleNumber].xValue;
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