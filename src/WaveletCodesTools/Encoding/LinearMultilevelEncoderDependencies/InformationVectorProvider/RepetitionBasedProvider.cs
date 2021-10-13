namespace AppliedAlgebra.WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Continuously repeats information words until necessary length was reached
    /// </summary>
    public class RepetitionBasedProvider : IInformationVectorProvider
    {
        public FieldElementsMatrix GetInformationVector(FieldElement[] informationWord, int requiredLength)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (informationWord.Length > requiredLength)
                throw new ArgumentException($"{nameof(informationWord)} length must not be greater than required length");

            return FieldElementsMatrix.ColumnVector(
                Enumerable.Repeat(informationWord, (requiredLength + informationWord.Length - 1) / informationWord.Length)
                    .SelectMany(word => word.Select(x => x))
                    .Take(requiredLength)
                    .ToArray()
            );
        }
    }
}