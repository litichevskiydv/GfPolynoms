namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.IterationFiltersCalculator
{
    using System;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using MoreLinq.Extensions;

    public class ConvolutionBasedCalculator : IIterationFiltersCalculator
    {
        /// <inheritdoc />
        public FieldElement[] GetIterationFilter(int iterationNumber, FieldElement[] sourceFilter)
        {
            if (iterationNumber <= 0)
                throw new ArgumentException($"{nameof(iterationNumber)} must be positive");
            if (sourceFilter == null)
                throw new ArgumentNullException(nameof(sourceFilter));
            if (sourceFilter.Length == 0)
                throw new ArgumentException($"{nameof(sourceFilter)} length must be positive");

            if (iterationNumber == 1)
                return sourceFilter;

            var sourceFilterLength = sourceFilter.Length;
            var filterLengthDecreasesTimes = 2.Pow(iterationNumber - 1);
            if (sourceFilterLength % (2 * filterLengthDecreasesTimes) != 0)
                throw new ArgumentException($"{nameof(sourceFilter)} length is incorrect");

            var field = sourceFilter.GetField();
            var iterationFilterLength = sourceFilterLength / filterLengthDecreasesTimes;
            return sourceFilter.Batch(iterationFilterLength).Aggregate(
                    Enumerable.Repeat(field.Zero(), iterationFilterLength).ToArray(),
                    (iterationFilter, part) =>
                    {
                        part.ForEach((element, i) => iterationFilter[i] += element);
                        return iterationFilter;
                    }
                )
                .ToArray();
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