namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.Matrices;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using LinearMultilevelEncoderDependencies.GeneratingMatrixProvider;

    /// <summary>
    /// Wavelet codes linear encoder that supports multilevel wavelet transform
    /// </summary>
    public class LinearMultilevelEncoder : IMultilevelEncoder
    {
        private readonly FieldElementsMatrix _generatingMatrix;

        /// <summary>
        /// Initializes encoder dependencies
        /// </summary>
        /// <param name="generatingMatrixProvider">Generating matrix provider</param>
        public LinearMultilevelEncoder(IGeneratingMatrixProvider generatingMatrixProvider)
        {
            if (generatingMatrixProvider == null)
                throw new ArgumentNullException(nameof(generatingMatrixProvider));

            _generatingMatrix = generatingMatrixProvider.GetGeneratingMatrix();
        }

        private static FieldElementsMatrix CreateInformationVector(FieldElement[] informationWord, int requiredLength)
        {
            var field = informationWord.GetField();
            return FieldElementsMatrix.ColumnVector(
                Enumerable.Repeat(field.Zero(), requiredLength - informationWord.Length)
                    .Concat(informationWord)
                    .ToArray()
            );

        }

        /// <inheritdoc/>
        public FieldElement[] Encode(int codewordLength, FieldElement[] informationWord, MultilevelEncoderOptions options = null)
        {
            if (codewordLength <= 0)
                throw new ArgumentException($"{nameof(codewordLength)} must be positive");
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (codewordLength < informationWord.Length)
                throw new ArgumentException($"{nameof(codewordLength)} is too short");
            if(informationWord.Length > _generatingMatrix.ColumnsCount)
                throw new ArgumentException($"{nameof(informationWord)} length is too long");

            return (_generatingMatrix * CreateInformationVector(informationWord, _generatingMatrix.ColumnsCount)).GetColumn(0);
        }
    }
}