namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.GeneratingMatrixProvider
{
    using System;
    using GfAlgorithms.Matrices;
    using LevelMatricesProvider;

    /// <summary>
    /// Provides generating matrices for naive wavelet codes implementation
    /// </summary>
    public class NaiveProvider : IGeneratingMatrixProvider
    {
        private readonly FieldElementsMatrix _generatingMatrix;

        /// <summary>
        /// Initializes provider's dependencies
        /// </summary>
        /// <param name="levelMatricesProvider">Wavelet transform matrices provider for each encoding level</param>
        /// <param name="levelsTransforms">Transformation matrices for each encoding level</param>
        public NaiveProvider(ILevelMatricesProvider levelMatricesProvider, params FieldElementsMatrix[] levelsTransforms)
        {
            if (levelMatricesProvider == null)
                throw new ArgumentNullException(nameof(levelMatricesProvider));
            if (levelsTransforms == null)
                throw new ArgumentNullException(nameof(levelsTransforms));
            if (levelsTransforms.Length == 0)
                throw new ArgumentException("At least one transform must be provided");

            _generatingMatrix = FieldElementsMatrix.IdentityMatrix(levelsTransforms[0].Field, levelsTransforms[0].RowsCount * 2);
            for (var levelNumber = 0; levelNumber < levelsTransforms.Length; levelNumber++)
            {
                var levelTransform = levelsTransforms[levelNumber];
                var (levelMatrixH, levelMatrixG) = levelMatricesProvider.GetLevelMatrices(levelNumber);
                _generatingMatrix *= levelMatrixH + levelMatrixG * levelTransform;
            }
        }

        /// <inheritdoc/>
        public FieldElementsMatrix GetGeneratingMatrix() => _generatingMatrix;
    }
}