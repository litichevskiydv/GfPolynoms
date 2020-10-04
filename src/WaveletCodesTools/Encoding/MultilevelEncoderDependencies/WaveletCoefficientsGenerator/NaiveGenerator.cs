namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Implementation of a wavelet coefficients vectors generator for codes that obtains
    /// details vector as a result of the linear transformation of the approximation vector
    /// </summary>
    public class NaiveGenerator : IWaveletCoefficientsGenerator
    {
        private readonly IReadOnlyList<FieldElementsMatrix> _levelsTransforms;

        public NaiveGenerator(params FieldElementsMatrix[] levelsTransforms)
        {
            if (levelsTransforms == null)
                throw new ArgumentNullException(nameof(levelsTransforms));

            _levelsTransforms = levelsTransforms;
        }

        /// <inheritdoc/>
        public FieldElement[] GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var computedSignalLength = 2.Pow(levelNumber + 1) * informationWord.Length;
            if (computedSignalLength != signalLength)
                throw new ArgumentException($"{nameof(signalLength)} and {nameof(informationWord)} length must be correlated");

            return informationWord;
        }

        /// <inheritdoc/>
        public FieldElement[] GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElement[] approximationVector)
        {
            if (levelNumber < 0)
                throw new ArgumentException($"{nameof(levelNumber)} must not be negative");
            if (levelNumber >= _levelsTransforms.Count)
                throw new ArgumentException($"Level {levelNumber} of transformation is not supported");

            return (_levelsTransforms[levelNumber] * FieldElementsMatrix.ColumnVector(approximationVector)).GetColumn(0);
        }
    }
}