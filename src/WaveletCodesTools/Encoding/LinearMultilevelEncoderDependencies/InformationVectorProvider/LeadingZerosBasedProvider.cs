namespace AppliedAlgebra.WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.Matrices;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Adds trailing zeros to the information word in order to achieve required length
    /// </summary>
    public class LeadingZerosBasedProvider : IInformationVectorProvider
    {
        /// <inheritdoc/>
        public FieldElementsMatrix GetInformationVector(FieldElement[] informationWord, int requiredLength)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (informationWord.Length > requiredLength)
                throw new ArgumentException($"{nameof(informationWord)} length must not be greater than required length");

            var field = informationWord.GetField();
            return FieldElementsMatrix.ColumnVector(
                Enumerable.Repeat(field.Zero(), requiredLength - informationWord.Length)
                    .Concat(informationWord)
                    .ToArray()
            );
        }
    }
}