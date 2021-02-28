namespace AppliedAlgebra.WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider
{
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Provides information vector for encoding
    /// </summary>
    public interface IInformationVectorProvider
    {
        /// <summary>
        /// Creates information vector by information word
        /// </summary>
        /// <param name="informationWord">Information word</param>
        /// <param name="requiredLength">Information vector required length</param>
        /// <returns>Information vector</returns>
        FieldElementsMatrix GetInformationVector(FieldElement[] informationWord, int requiredLength);
    }
}