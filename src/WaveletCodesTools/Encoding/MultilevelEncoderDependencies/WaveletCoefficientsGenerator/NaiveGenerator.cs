namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using System.Collections.Generic;
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
        public FieldElementsMatrix GetInitialApproximationVector(FieldElement[] informationWord, int vectorLength)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (informationWord.Length == 0)
                throw new ArgumentException($"{nameof(informationWord)} must not be empty");
            if (informationWord.Length != vectorLength)
                throw new ArgumentException($"{nameof(vectorLength)} and {nameof(informationWord)} length must be equal");

            return FieldElementsMatrix.ColumnVector(informationWord);
        }

        /// <inheritdoc/>
        public DetailsVectorGenerationResult GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElementsMatrix approximationVector)
        {
            if (levelNumber < 0)
                throw new ArgumentException($"{nameof(levelNumber)} must not be negative");
            if (levelNumber >= _levelsTransforms.Count)
                throw new ArgumentException($"Level {levelNumber} of transformation is not supported");
            if(approximationVector == null)
                throw new ArgumentNullException(nameof(approximationVector));
            if(approximationVector.ColumnsCount != 1)
                throw new ArgumentException($"{nameof(approximationVector)} must be a column vector");

            return new DetailsVectorGenerationResult(_levelsTransforms[levelNumber] * approximationVector, 0);
        }
    }
}