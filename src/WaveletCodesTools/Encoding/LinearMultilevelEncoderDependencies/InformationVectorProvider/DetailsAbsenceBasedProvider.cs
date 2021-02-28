namespace AppliedAlgebra.WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider
{
    using System;
    using System.Linq;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.Matrices;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Replace part of details by zeros
    /// </summary>
    public class DetailsAbsenceBasedProvider : IInformationVectorProvider
    {
        private readonly int _levelsCount;

        public DetailsAbsenceBasedProvider(int levelsCount)
        {
            if (levelsCount <= 0)
                throw new ArgumentException($"{levelsCount} must be positive");

            _levelsCount = levelsCount;
        }

        public FieldElementsMatrix GetInformationVector(FieldElement[] informationWord, int requiredLength)
        {
            if (informationWord == null)
                throw new ArgumentNullException(nameof(informationWord));
            if (informationWord.Length > requiredLength)
                throw new ArgumentException($"{nameof(informationWord)} length must not be greater than required length");

            var field = informationWord.GetField();
            var lastLevelApproximationLength = requiredLength / 2.Pow(_levelsCount);
            return FieldElementsMatrix.ColumnVector(
                informationWord.Take(lastLevelApproximationLength)
                    .Concat(Enumerable.Repeat(field.Zero(), requiredLength - informationWord.Length))
                    .Concat(informationWord.Skip(lastLevelApproximationLength))
                    .ToArray()
            );
        }
    }
}