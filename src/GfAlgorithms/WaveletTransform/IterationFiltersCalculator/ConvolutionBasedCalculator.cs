namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.IterationFiltersCalculator
{
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using MoreLinq.Extensions;

    public class ConvolutionBasedCalculator : IterationFiltersCalculatorBase
    {
        protected override FieldElement[] GetIterationFilter(
            int iterationNumber,
            GaloisField field,
            FieldElement[] sourceFilter,
            int sourceFilterLength,
            int filterLengthDecreasesTimes
        )
        {
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
    }
}