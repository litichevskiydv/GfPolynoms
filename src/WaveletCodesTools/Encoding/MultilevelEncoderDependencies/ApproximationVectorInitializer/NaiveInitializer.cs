namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer
{
    using System;
    using GfAlgorithms.Extensions;
    using GfPolynoms;

    /// <summary>
    /// An approximation vector initializer for codes that transforms the entire information word
    /// </summary>
    public class NaiveInitializer: IApproximationVectorInitializer
    {
        /// <inheritdoc/>
        public FieldElement[] GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var computedSignalLength = 2.Pow(levelNumber + 1) * informationWord.Length;
            if (computedSignalLength != signalLength)
                throw new ArgumentException($"{nameof(signalLength)} and {nameof(informationWord)} length must be correlated");

            return informationWord;
        }
    }
}