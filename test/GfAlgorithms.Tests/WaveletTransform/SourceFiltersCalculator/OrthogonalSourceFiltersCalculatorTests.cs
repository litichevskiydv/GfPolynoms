namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.SourceFiltersCalculator
{
    using GfAlgorithms.WaveletTransform.SourceFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class OrthogonalSourceFiltersCalculatorTests : SourceFiltersCalculatorTestsBase
    {
        [UsedImplicitly]
        public static TheoryData<FieldElement[]> OrthogonalSourceFiltersCalculationTestCase;

        static OrthogonalSourceFiltersCalculatorTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);

            OrthogonalSourceFiltersCalculationTestCase
                = new TheoryData<FieldElement[]>
                  {
                      gf2.CreateElementsVector(1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1),
                      gf3.CreateElementsVector(2, 1, 0, 0, 1, 1, 0, 0)
                  };
        }

        public OrthogonalSourceFiltersCalculatorTests() : base(new OrthogonalSourceFiltersCalculator())
        {
        }

        [Theory]
        [MemberData(nameof(OrthogonalSourceFiltersCalculationTestCase))]
        public void MustCalculateOrthogonalSourceFiltersVectors(FieldElement[] sourceFilterH) =>
            TestSourceFiltersVectorsCalculation(sourceFilterH);

        [Theory]
        [MemberData(nameof(OrthogonalSourceFiltersCalculationTestCase))]
        public void MustCalculateOrthogonalSourceFiltersPolynomials(FieldElement[] sourceFilterH) =>
            TestSourceFiltersPolynomialsCalculation(sourceFilterH);
    }
}