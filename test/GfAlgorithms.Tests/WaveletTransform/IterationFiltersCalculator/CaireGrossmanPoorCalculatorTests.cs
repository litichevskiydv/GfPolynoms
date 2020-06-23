namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using JetBrains.Annotations;

    [UsedImplicitly]
    public class CaireGrossmanPoorCalculatorTests : IterationFiltersCalculatorTestsBase
    {
        public CaireGrossmanPoorCalculatorTests() : base(new CaireGrossmanPoorCalculator())
        {
        }
    }
}