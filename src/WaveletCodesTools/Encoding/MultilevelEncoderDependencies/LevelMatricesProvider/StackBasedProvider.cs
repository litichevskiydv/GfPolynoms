namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider
{
    using System;
    using GfAlgorithms.Matrices;

    /// <summary>
    /// Returns wavelet transform level matrices from prepared collection
    /// </summary>
    public class StackBasedProvider : ILevelMatricesProvider
    {
        private readonly (FieldElementsMatrix levelMatrixH, FieldElementsMatrix levelMatrixG)[] _levelsMatrices;

        public StackBasedProvider((FieldElementsMatrix levelMatrixH, FieldElementsMatrix levelMatrixG)[] levelsMatrices)
        {
            if (levelsMatrices == null)
                throw new ArgumentNullException(nameof(levelsMatrices));
            if (levelsMatrices.Length == 0)
                throw new ArgumentException($"{nameof(levelsMatrices)} must not be empty");

            _levelsMatrices = levelsMatrices;
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