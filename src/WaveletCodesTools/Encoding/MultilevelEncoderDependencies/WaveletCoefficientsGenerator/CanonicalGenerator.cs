namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Implementation of a wavelet coefficients vectors generator for codes that sequentially
    /// transforms parts of an information word
    /// </summary>
    public class CanonicalGenerator: IWaveletCoefficientsGenerator
    {
        /// <inheritdoc/>
        public FieldElementsMatrix GetInitialApproximationVector(FieldElement[] informationWord, int vectorLength)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (vectorLength <= 0)
                throw new ArgumentException($"{nameof(vectorLength)} must be positive");
            if (informationWord.Length < vectorLength)
                throw new ArgumentException($"{nameof(informationWord)} is too short");

            return FieldElementsMatrix.ColumnVector(informationWord.Take(vectorLength).ToArray());
        }

        /// <inheritdoc/>
        public DetailsVectorGenerationResult GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElementsMatrix approximationVector)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (approximationVector == null)
                throw new ArgumentNullException(nameof(approximationVector));
            if (approximationVector.ColumnsCount != 1)
                throw new ArgumentException($"{nameof(approximationVector)} must be a column vector");

            var field = approximationVector.Field;
            var approximationVectorLength = approximationVector.RowsCount;

            var informationSymbols = informationWord.Skip(approximationVectorLength).Take(approximationVectorLength).ToArray();
            var correctableComponentsCount = approximationVectorLength - informationSymbols.Length;
            return new DetailsVectorGenerationResult(
                FieldElementsMatrix.ColumnVector(
                    informationSymbols.Concat(Enumerable.Repeat(field.Zero(), correctableComponentsCount)).ToArray()
                ),
                correctableComponentsCount
            );
        }
    }
}