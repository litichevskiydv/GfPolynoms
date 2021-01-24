namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.GeneratingMatrixProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using LevelMatricesProvider;

    /// <summary>
    /// Provides generating matrices for canonical wavelet codes implementation
    /// </summary>
    public class CanonicalProvider : IGeneratingMatrixProvider
    {
        private readonly FieldElementsMatrix _generatingMatrix;

        /// <summary>
        /// Initializes provider's dependencies
        /// </summary>
        /// <param name="levelMatricesProvider">Wavelet transform matrices provider for each encoding level</param>
        /// <param name="levelsCount">Wavelet decomposition levels count</param>
        public CanonicalProvider(ILevelMatricesProvider levelMatricesProvider, int levelsCount)
        {
            if (levelMatricesProvider == null)
                throw new ArgumentNullException(nameof(levelMatricesProvider));
            if (levelsCount <= 0)
                throw new ArgumentException($"{nameof(levelsCount)} must be positive");

            var (previousMatrixPart, zeroLevelMatrixG) = levelMatricesProvider.GetLevelMatrices(0);
            var generatingMatrixParts = new List<FieldElementsMatrix> {zeroLevelMatrixG};
            for (var levelNumber = 1; levelNumber < levelsCount; levelNumber++)
            {
                var (levelMatrixH, levelMatrixG) = levelMatricesProvider.GetLevelMatrices(levelNumber);

                generatingMatrixParts.Add(previousMatrixPart * levelMatrixG);
                previousMatrixPart *= levelMatrixH;
            }

            _generatingMatrix = Enumerable.Reverse(generatingMatrixParts)
                .Aggregate(previousMatrixPart, (acc, cur) => acc.ConcatColumns(cur));
        }

        /// <inheritdoc/>
        public FieldElementsMatrix GetGeneratingMatrix() => _generatingMatrix;
    }
}