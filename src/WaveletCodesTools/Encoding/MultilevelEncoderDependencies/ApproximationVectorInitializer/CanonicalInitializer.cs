namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfPolynoms;

    /// <summary>
    /// An initializer of an approximation vector for codes that sequentially transforms parts of an information word
    /// </summary>
    public class CanonicalInitializer : IApproximationVectorInitializer
    {
        /// <inheritdoc/>
        public FieldElement[] GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var multiplier = 2.Pow(levelNumber + 1);
            if(signalLength % multiplier != 0)
                throw new ArgumentException($"Can't initialize approximation vector for level {levelNumber}");

            var approximationVectorLength = signalLength / multiplier;
            if(informationWord.Length < approximationVectorLength)
                throw new ArgumentException($"{nameof(informationWord)} is too short");

            return informationWord.Take(approximationVectorLength).ToArray();
        }
    }
}