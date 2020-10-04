namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// An approximation vector initializer for codes that transforms the entire information word
    /// </summary>
    public class NaiveInitializer: IApproximationVectorInitializer
    {
        /// <inheritdoc/>
        public FieldElement[] GetApproximationVector(FieldElement[] informationWord, int levelNumber)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (informationWord.Length == 0)
                throw new ArgumentException($"{nameof(informationWord)} must not be empty");
            if(levelNumber < 0)
                throw new ArgumentException($"{nameof(levelNumber)} must not be negative");

            return informationWord;
        }
    }
}