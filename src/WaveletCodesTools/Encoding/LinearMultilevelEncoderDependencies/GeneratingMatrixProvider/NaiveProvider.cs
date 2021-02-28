namespace AppliedAlgebra.WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider
{
    using System;
    using GfAlgorithms.Matrices;
    using MultilevelEncoderDependencies.LevelMatricesProvider;

    /// <summary>
    /// Provides generating matrices for naive wavelet codes implementation
    /// </summary>
    public class NaiveProvider : IGeneratingMatrixProvider
    {
        private readonly ILevelMatricesProvider _levelMatricesProvider;
        private readonly FieldElementsMatrix[] _levelsTransforms;

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

            _levelMatricesProvider = levelMatricesProvider;
            _levelsTransforms = levelsTransforms;
        }

        /// <inheritdoc/>
        public FieldElementsMatrix GetGeneratingMatrix(int levelsCount)
        {
            if (levelsCount <= 0)
                throw new ArgumentException($"{nameof(levelsCount)} must be positive");
            if (levelsCount > _levelsTransforms.Length)
                throw new ArgumentException($"{nameof(levelsCount)} must not be greater than levels transforms count");

            var generatingMatrix = FieldElementsMatrix.IdentityMatrix(_levelsTransforms[0].Field, _levelsTransforms[0].RowsCount * 2);
            for (var levelNumber = 0; levelNumber < levelsCount; levelNumber++)
            {
                var levelTransform = _levelsTransforms[levelNumber];
                var (levelMatrixH, levelMatrixG) = _levelMatricesProvider.GetLevelMatrices(levelNumber);
                generatingMatrix *= levelMatrixH + levelMatrixG * levelTransform;
            }

            return generatingMatrix;
        }
    }
}