namespace AppliedAlgebra.WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider
{
    using GfAlgorithms.Matrices;

    /// <summary>
    /// Contract for linear codes' generating matrices provider
    /// </summary>
    public interface IGeneratingMatrixProvider
    {
        /// <summary>
        /// Provides linear codes generating matrix
        /// </summary>
        /// <param name="levelsCount">Number of the levels of the wavelet decomposition used in generating matrix</param>
        /// <returns>Generating matrix</returns>
        FieldElementsMatrix GetGeneratingMatrix(int levelsCount);
    }
}