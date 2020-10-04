namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Implementation of a wavelet coefficients vectors generator for codes that sequentially
    /// transforms parts of an information word
    /// </summary>
    public class CanonicalGenerator: IWaveletCoefficientsGenerator
    {
        /// <inheritdoc/>
        public FieldElement[] GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var multiplier = 2.Pow(levelNumber + 1);
            if (signalLength % multiplier != 0)
                throw new ArgumentException($"Can't initialize approximation vector for level {levelNumber}");

            var approximationVectorLength = signalLength / multiplier;
            if (informationWord.Length < approximationVectorLength)
                throw new ArgumentException($"{nameof(informationWord)} is too short");

            return informationWord.Take(approximationVectorLength).ToArray();
        }

        /// <inheritdoc/>
        public FieldElement[] GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElement[] approximationVector)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));

            var field = approximationVector.GetField();
            var informationSymbols = informationWord.Skip(approximationVector.Length).Take(approximationVector.Length).ToArray();
            return informationSymbols.Concat(Enumerable.Repeat(field.Zero(), approximationVector.Length - informationSymbols.Length))
                .ToArray();
        }
    }
}