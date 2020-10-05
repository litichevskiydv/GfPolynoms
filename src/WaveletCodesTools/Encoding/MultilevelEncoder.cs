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
        private readonly IIterationFiltersCalculator _iterationFiltersCalculator;
        private readonly IWaveletCoefficientsGenerator _waveletCoefficientsGenerator;

        public MultilevelEncoder(
            IIterationFiltersCalculator iterationFiltersCalculator,
            IWaveletCoefficientsGenerator waveletCoefficientsGenerator
        )
        {
            if (iterationFiltersCalculator == null)
                throw new ArgumentNullException(nameof(iterationFiltersCalculator));
            if (waveletCoefficientsGenerator == null)
                throw new ArgumentNullException(nameof(waveletCoefficientsGenerator));

            _iterationFiltersCalculator = iterationFiltersCalculator;
            _waveletCoefficientsGenerator = waveletCoefficientsGenerator;
        }

        private FieldElementsMatrix PrepareIterationMatrix(int levelNumber, FieldElement[] sourceFilter) =>
            FieldElementsMatrix.DoubleCirculantMatrix(_iterationFiltersCalculator.GetIterationFilter(levelNumber, sourceFilter))
                .Transpose();

        private (FieldElementsMatrix iterationMatrixH, FieldElementsMatrix iterationMatrixG)[] PrepareIterationMatrices(
            int levelsCount,
            (FieldElement[] h, FieldElement[] g) synthesisFilters
        )
        {
            return Enumerable.Range(0, levelsCount)
                .Select(levelNumber => (
                            iterationMatrixH: PrepareIterationMatrix(levelNumber, synthesisFilters.h),
                            iterationMatrixG: PrepareIterationMatrix(levelNumber, synthesisFilters.g)
                        )
                )
                .ToArray();
        }

        /// <inheritdoc/>
        public FieldElement[] Encode(
            int codewordLength,
            int levelsCount,
            (FieldElement[] h, FieldElement[] g) synthesisFilters,
            FieldElement[] informationWord
        )
        {
            if(codewordLength <= 0)
                throw new ArgumentException($"{nameof(codewordLength)} must be positive");
            if(levelsCount <= 0)
                throw new ArgumentException($"{nameof(levelsCount)} must be positive");
            if(informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var signalLength = synthesisFilters.h.Length;
            if(signalLength % 2.Pow(levelsCount) != 0)
                throw new ArgumentException($"{levelsCount} levels decomposition is not supported");
            if (codewordLength < signalLength / 2)
                throw new ArgumentException($"{nameof(codewordLength)} is too small");

            var iterationMatrices = PrepareIterationMatrices(levelsCount, synthesisFilters);
            var approximationVector = _waveletCoefficientsGenerator.GetApproximationVector(informationWord, signalLength, levelsCount - 1);
            for (var levelNumber = levelsCount - 1; levelNumber >= 0; levelNumber--)
            {
                var (iterationMatrixH, iterationMatrixG) = iterationMatrices[levelNumber];
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