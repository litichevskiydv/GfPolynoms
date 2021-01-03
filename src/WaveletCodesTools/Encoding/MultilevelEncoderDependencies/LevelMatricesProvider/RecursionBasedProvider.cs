namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms;

    /// <summary>
    /// Calculates the wavelet transforms for each encoding level from the source transform
    /// </summary>
    public class RecursionBasedProvider : ILevelMatricesProvider
    {
        private readonly (FieldElementsMatrix levelMatrixH, FieldElementsMatrix levelMatrixG)[] _levelsMatrices;

        private static FieldElementsMatrix PrepareLevelMatrix(
            IIterationFiltersCalculator iterationFiltersCalculator,
            int levelNumber,
            FieldElement[] sourceFilter
        ) =>
            FieldElementsMatrix.DoubleCirculantMatrix(iterationFiltersCalculator.GetIterationFilter(levelNumber, sourceFilter))
                .Transpose();

        private static (FieldElementsMatrix levelMatrixH, FieldElementsMatrix levelMatrixG)[] PrepareLevelsMatrices(
            IIterationFiltersCalculator iterationFiltersCalculator,
            int levelsCount,
            (FieldElement[] h, FieldElement[] g) synthesisFilters
        ) =>
            Enumerable.Range(0, levelsCount)
                .Select(
                    levelNumber =>
                    (
                        levelMatrixH: PrepareLevelMatrix(iterationFiltersCalculator, levelNumber, synthesisFilters.h),
                        levelMatrixG: PrepareLevelMatrix(iterationFiltersCalculator, levelNumber, synthesisFilters.g)
                    )
                )
                .ToArray();

        /// <summary>
        /// Initializes provider
        /// </summary>
        /// <param name="iterationFiltersCalculator">Levels filters calculator</param>
        /// <param name="levelsCount">Wavelet decomposition levels count</param>
        /// <param name="synthesisFilters">Synthesis filters pair</param>
        public RecursionBasedProvider(
            IIterationFiltersCalculator iterationFiltersCalculator,
            int levelsCount,
            (FieldElement[] h, FieldElement[] g) synthesisFilters
        )
        {
            if (iterationFiltersCalculator == null)
                throw new ArgumentNullException(nameof(iterationFiltersCalculator));
            if (levelsCount <= 0)
                throw new ArgumentException($"{nameof(levelsCount)} must be positive");

            _levelsMatrices = PrepareLevelsMatrices(iterationFiltersCalculator, levelsCount, synthesisFilters);
        }

        /// <inheritdoc/>
        public (FieldElementsMatrix levelMatrixH, FieldElementsMatrix levelMatrixG) GetLevelMatrices(int levelNumber)
        {
            if (levelNumber < 0)
                throw new ArgumentException($"{nameof(levelNumber)} must not be negative");
            if (levelNumber >= _levelsMatrices.Length)
                throw new ArgumentException($"Level {levelNumber} of transformation is not supported");

            return _levelsMatrices[levelNumber];
        }
    }
}