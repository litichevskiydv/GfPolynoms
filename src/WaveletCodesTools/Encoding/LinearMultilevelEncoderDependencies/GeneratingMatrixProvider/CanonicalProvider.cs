namespace AppliedAlgebra.WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using MultilevelEncoderDependencies.LevelMatricesProvider;

    /// <summary>
    /// Provides generating matrices for canonical wavelet codes implementation
    /// </summary>
    public class CanonicalProvider : IGeneratingMatrixProvider
    {
        private readonly ILevelMatricesProvider _levelMatricesProvider;

        /// <summary>
        /// Initializes provider's dependencies
        /// </summary>
        /// <param name="levelMatricesProvider">Wavelet transform matrices provider for each encoding level</param>
        public CanonicalProvider(ILevelMatricesProvider levelMatricesProvider)
        {
            if (levelMatricesProvider == null)
                throw new ArgumentNullException(nameof(levelMatricesProvider));

            _levelMatricesProvider = levelMatricesProvider;
        }

        /// <inheritdoc/>
        public FieldElementsMatrix GetGeneratingMatrix(int levelsCount)
        {
            if (levelsCount <= 0)
                throw new ArgumentException($"{nameof(levelsCount)} must be positive");

            var (previousMatrixPart, zeroLevelMatrixG) = _levelMatricesProvider.GetLevelMatrices(0);
            var generatingMatrixParts = new List<FieldElementsMatrix> { zeroLevelMatrixG };
            for (var levelNumber = 1; levelNumber < levelsCount; levelNumber++)
            {
                var (levelMatrixH, levelMatrixG) = _levelMatricesProvider.GetLevelMatrices(levelNumber);

                generatingMatrixParts.Add(previousMatrixPart * levelMatrixG);
                previousMatrixPart *= levelMatrixH;
            }

            return Enumerable.Reverse(generatingMatrixParts)
                .Aggregate(previousMatrixPart, (acc, cur) => acc.ConcatColumns(cur));
        }
    }
}