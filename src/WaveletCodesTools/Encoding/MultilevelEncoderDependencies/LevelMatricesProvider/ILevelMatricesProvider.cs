namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider
{
    using GfAlgorithms.Matrices;

    /// <summary>
    /// Contract for wavelet transform matrices provider for each encoding level
    /// </summary>
    public interface ILevelMatricesProvider
    {
        /// <summary>
        /// Provides wavelet transform matrices for required encoding level
        /// </summary>
        /// <param name="levelNumber">Encoding level number</param>
        /// <returns>Wavelet transform matrices</returns>
        (FieldElementsMatrix levelMatrixH, FieldElementsMatrix levelMatrixG) GetLevelMatrices(int levelNumber);
    }
}