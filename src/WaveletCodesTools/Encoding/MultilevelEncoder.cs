namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfPolynoms;
    using MultilevelEncoderDependencies.DetailsVectorCorrector;
    using MultilevelEncoderDependencies.LevelMatricesProvider;
    using MultilevelEncoderDependencies.WaveletCoefficientsGenerator;

    /// <summary>
    /// Wavelet codes encoder that supports multilevel wavelet transform
    /// </summary>
    public class MultilevelEncoder : IMultilevelEncoder
    {
        private readonly ILevelMatricesProvider _levelMatricesProvider;
        private readonly IWaveletCoefficientsGenerator _waveletCoefficientsGenerator;
        private readonly IDetailsVectorCorrector _detailsVectorCorrector;

        private readonly int _levelsCount;

        /// <summary>
        /// Initializes multilevel encoder
        /// </summary>
        /// <param name="levelMatricesProvider">Provides wavelet transform matrices for each encoding level</param>
        /// <param name="waveletCoefficientsGenerator">Wavelet coefficients generator</param>
        /// <param name="detailsVectorCorrector">Details vector corrector</param>
        /// <param name="levelsCount">Wavelet decomposition levels count</param>
        public MultilevelEncoder(
            ILevelMatricesProvider levelMatricesProvider,
            IWaveletCoefficientsGenerator waveletCoefficientsGenerator,
            IDetailsVectorCorrector detailsVectorCorrector,
            int levelsCount
        )
        {
            if (levelMatricesProvider == null)
                throw new ArgumentNullException(nameof(levelMatricesProvider));
            if (waveletCoefficientsGenerator == null)
                throw new ArgumentNullException(nameof(waveletCoefficientsGenerator));
            if (detailsVectorCorrector == null)
                throw new ArgumentNullException(nameof(detailsVectorCorrector));
            if (levelsCount <= 0)
                throw new ArgumentException($"{nameof(levelsCount)} must be positive");

            _levelMatricesProvider = levelMatricesProvider;
            _waveletCoefficientsGenerator = waveletCoefficientsGenerator;
            _detailsVectorCorrector = detailsVectorCorrector;

            _levelsCount = levelsCount;
        }

        /// <inheritdoc/>
        public FieldElement[] Encode(int codewordLength, FieldElement[] informationWord, MultilevelEncoderOptions options = null)
        {
            if(codewordLength <= 0)
                throw new ArgumentException($"{nameof(codewordLength)} must be positive");
            if(informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if(codewordLength < informationWord.Length)
                throw new ArgumentException($"{nameof(codewordLength)} is too short");

            FieldElementsMatrix approximationVector = null;
            var opts = options ?? new MultilevelEncoderOptions();
            
            for (var levelNumber = _levelsCount - 1; levelNumber >= 0; levelNumber--)
            {
                var (iterationMatrixH, iterationMatrixG) = _levelMatricesProvider.GetLevelMatrices(levelNumber);

                if(approximationVector == null)
                    approximationVector = _waveletCoefficientsGenerator.GetInitialApproximationVector(informationWord, iterationMatrixH.ColumnsCount);

                var (detailsVector, correctableComponentsCount) = _waveletCoefficientsGenerator.GetDetailsVector(informationWord, levelNumber, approximationVector);
                if (levelNumber == 0)
                    detailsVector = _detailsVectorCorrector.CorrectDetailsVector(
                        (iterationMatrixH, iterationMatrixG),
                        approximationVector,
                        detailsVector,
                        correctableComponentsCount,
                        iterationMatrixH.RowsCount - codewordLength,
                        new CorrectorOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism}
                    );

                approximationVector = iterationMatrixH * approximationVector + iterationMatrixG * detailsVector;
            }

            return approximationVector
                .GetColumn(0)
                .Take(codewordLength)
                .ToArray();
        }
    }
}