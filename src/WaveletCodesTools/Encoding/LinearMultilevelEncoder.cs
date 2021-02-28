namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    using System;
    using GfAlgorithms.Matrices;
    using GfPolynoms;
    using LinearMultilevelEncoderDependencies.GeneratingMatrixProvider;
    using LinearMultilevelEncoderDependencies.InformationVectorProvider;

    /// <summary>
    /// Wavelet codes linear encoder that supports multilevel wavelet transform
    /// </summary>
    public class LinearMultilevelEncoder : IMultilevelEncoder
    {
        private readonly FieldElementsMatrix _generatingMatrix;
        private readonly IInformationVectorProvider _informationVectorProvider;

        /// <summary>
        /// Initializes encoder dependencies
        /// </summary>
        /// <param name="generatingMatrixProvider">Generating matrix provider</param>
        /// <param name="informationVectorProvider">Information vector provider</param>
        /// <param name="levelsCount">Number of the levels of the wavelet decomposition used in encoding</param>
        public LinearMultilevelEncoder(
            IGeneratingMatrixProvider generatingMatrixProvider,
            IInformationVectorProvider informationVectorProvider,
            int levelsCount
        )
        {
            if (generatingMatrixProvider == null)
                throw new ArgumentNullException(nameof(generatingMatrixProvider));
            if (informationVectorProvider == null)
                throw new ArgumentNullException(nameof(informationVectorProvider));

            _generatingMatrix = generatingMatrixProvider.GetGeneratingMatrix(levelsCount);
            _informationVectorProvider = informationVectorProvider;
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

            return (_generatingMatrix * _informationVectorProvider.GetInformationVector(informationWord, _generatingMatrix.ColumnsCount))
                .GetColumn(0);
        }
    }
}