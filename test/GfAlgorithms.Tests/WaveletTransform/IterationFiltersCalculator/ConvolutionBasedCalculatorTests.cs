namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using JetBrains.Annotations;

    [UsedImplicitly]
    public class ConvolutionBasedCalculatorTests : IterationFiltersCalculatorTestsBase
    {
        public ConvolutionBasedCalculatorTests() : base(new ConvolutionBasedCalculator())
        {
        }
    }
}