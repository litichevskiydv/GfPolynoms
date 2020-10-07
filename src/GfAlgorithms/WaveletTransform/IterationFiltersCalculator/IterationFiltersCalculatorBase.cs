namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.IterationFiltersCalculator
{
    using System;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public abstract class IterationFiltersCalculatorBase : IIterationFiltersCalculator
    {
        protected abstract FieldElement[] GetIterationFilter(
            int iterationNumber,
            GaloisField field,
            FieldElement[] sourceFilter,
            int sourceFilterLength,
            int filterLengthDecreasesTimes
        );

        public FieldElement[] GetIterationFilter(int iterationNumber, FieldElement[] sourceFilter)
        {
            if (iterationNumber < 0)
                throw new ArgumentException($"{nameof(iterationNumber)} must not be negative");
            if (sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));
            if (sourceFilter.Length == 0)
                throw new ArgumentException($"{nameof(sourceFilter)} length must be positive");

            if (iterationNumber == 0)
                return sourceFilter;

            var sourceFilterLength = sourceFilter.Length;
            var filterLengthDecreasesTimes = 2.Pow(iterationNumber);
            if (sourceFilterLength % (2 * filterLengthDecreasesTimes) != 0)
                throw new ArgumentException($"{nameof(sourceFilter)} length is incorrect");

            return GetIterationFilter(iterationNumber, sourceFilter.GetField(), sourceFilter, sourceFilterLength, filterLengthDecreasesTimes);
        }

        /// <inheritdoc />
        public Polynomial GetIterationFilter(int iterationNumber, Polynomial sourceFilter, int? expectedDegree = null)
        {
            if (sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));

            return new Polynomial(GetIterationFilter(iterationNumber, sourceFilter.GetCoefficients(expectedDegree)));
        }
    }
}