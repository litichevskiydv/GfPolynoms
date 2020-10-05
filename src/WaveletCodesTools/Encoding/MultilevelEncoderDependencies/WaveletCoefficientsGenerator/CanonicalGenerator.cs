namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
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
        public FieldElementsMatrix GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var multiplier = 2.Pow(levelNumber + 1);
            if (signalLength % multiplier != 0)
                throw new ArgumentException($"Can't initialize approximation vector for level {levelNumber}");

            var approximationVectorLength = signalLength / multiplier;
            if (informationWord.Length < approximationVectorLength)
                throw new ArgumentException($"{nameof(informationWord)} is too short");

            return FieldElementsMatrix.ColumnVector(informationWord.Take(approximationVectorLength).ToArray());
        }

        /// <inheritdoc/>
        public FieldElementsMatrix GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElementsMatrix approximationVector)
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
            return FieldElementsMatrix.ColumnVector(
                informationSymbols.Concat(Enumerable.Repeat(field.Zero(), approximationVectorLength - informationSymbols.Length)).ToArray()
            );
        }
    }
}