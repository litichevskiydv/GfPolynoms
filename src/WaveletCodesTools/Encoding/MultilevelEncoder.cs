namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms;
    using MultilevelEncoderDependencies.WaveletCoefficientsGenerator;

    /// <summary>
    /// Wavelet codes encoder that supports multilevel wavelet transform
    /// </summary>
    public class MultilevelEncoder : IMultilevelEncoder
    {
        private readonly IWaveletCoefficientsGenerator _waveletCoefficientsGenerator;

        private readonly int _signalLength;
        private readonly (FieldElementsMatrix iterationMatrixH, FieldElementsMatrix iterationMatrixG)[] _iterationsMatrices;

        private static FieldElementsMatrix PrepareIterationMatrix(
            IIterationFiltersCalculator iterationFiltersCalculator,
            int levelNumber, 
            FieldElement[] sourceFilter
        ) =>
            FieldElementsMatrix.DoubleCirculantMatrix(iterationFiltersCalculator.GetIterationFilter(levelNumber, sourceFilter))
                .Transpose();

        private static (FieldElementsMatrix iterationMatrixH, FieldElementsMatrix iterationMatrixG)[] PrepareIterationMatrices(
            IIterationFiltersCalculator iterationFiltersCalculator,
            int levelsCount,
            (FieldElement[] h, FieldElement[] g) synthesisFilters
        )
        {
            return Enumerable.Range(0, levelsCount)
                .Select(levelNumber => (
                            iterationMatrixH: PrepareIterationMatrix(iterationFiltersCalculator, levelNumber, synthesisFilters.h),
                            iterationMatrixG: PrepareIterationMatrix(iterationFiltersCalculator, levelNumber, synthesisFilters.g)
                        )
                )
                .ToArray();
        }

        /// <summary>
        /// Initializes multilevel encoder
        /// </summary>
        /// <param name="iterationFiltersCalculator">Levels filters calculator</param>
        /// <param name="waveletCoefficientsGenerator">Wavelet coefficients generator</param>
        /// <param name="levelsCount">Wavelet decomposition levels count</param>
        /// <param name="synthesisFilters">Synthesis filters pair</param>
        public MultilevelEncoder(
            IIterationFiltersCalculator iterationFiltersCalculator,
            IWaveletCoefficientsGenerator waveletCoefficientsGenerator,
            int levelsCount,
            (FieldElement[] h, FieldElement[] g) synthesisFilters
        )
        {
            if (iterationFiltersCalculator == null)
                throw new ArgumentNullException(nameof(iterationFiltersCalculator));
            if (waveletCoefficientsGenerator == null)
                throw new ArgumentNullException(nameof(waveletCoefficientsGenerator));
            if (levelsCount <= 0)
                throw new ArgumentException($"{nameof(levelsCount)} must be positive");

            _signalLength = synthesisFilters.h.Length;
            if (_signalLength % 2.Pow(levelsCount) != 0)
                throw new ArgumentException($"{levelsCount} levels decomposition is not supported");

            _waveletCoefficientsGenerator = waveletCoefficientsGenerator;
            _iterationsMatrices = PrepareIterationMatrices(iterationFiltersCalculator, levelsCount, synthesisFilters);
        }

        /// <inheritdoc/>
        public FieldElement[] Encode(int codewordLength, FieldElement[] informationWord)
        {
            if(codewordLength <= 0)
                throw new ArgumentException($"{nameof(codewordLength)} must be positive");
            if(informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if(codewordLength < informationWord.Length)
                throw new ArgumentException($"{nameof(codewordLength)} is too short");

            var levelsCount = _iterationsMatrices.Length;
            var approximationVector = _waveletCoefficientsGenerator.GetApproximationVector(informationWord, _signalLength, levelsCount - 1);
            for (var levelNumber = levelsCount - 1; levelNumber >= 0; levelNumber--)
            {
                var (iterationMatrixH, iterationMatrixG) = _iterationsMatrices[levelNumber];
                var detailsVector = _waveletCoefficientsGenerator.GetDetailsVector(informationWord, levelNumber, approximationVector);

                approximationVector = iterationMatrixH * approximationVector + iterationMatrixG * detailsVector;
            }

            return approximationVector
                .GetColumn(0)
                .Take(codewordLength)
                .ToArray();
        }
    }
}