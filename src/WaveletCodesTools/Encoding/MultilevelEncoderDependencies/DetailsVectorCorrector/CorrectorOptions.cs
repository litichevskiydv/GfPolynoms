namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector
{
    /// <summary>
    /// Details vector corrector options
    /// </summary>
    public class CorrectorOptions
    {
        /// <summary>
        /// Maximum degree of parallelism if it is supported by corrector
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; }

        public CorrectorOptions()
        {
            MaxDegreeOfParallelism = -1;
        }
    }
}