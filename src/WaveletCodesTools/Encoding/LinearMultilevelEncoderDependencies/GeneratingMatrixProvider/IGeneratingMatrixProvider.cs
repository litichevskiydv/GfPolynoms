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
        /// <returns></returns>
        FieldElementsMatrix GetGeneratingMatrix();
    }
}