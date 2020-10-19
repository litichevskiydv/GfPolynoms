namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    /// <summary>
    /// Multilevel encoder options
    /// </summary>
    public class MultilevelEncoderOptions
    {
        /// <summary>
        /// Maximum degree of parallelism if it is supported by encoder
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; }

        public MultilevelEncoderOptions()
        {
            MaxDegreeOfParallelism = -1;
        }
    }
}