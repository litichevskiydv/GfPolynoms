namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.SourceFiltersCalculator
{
    using System;
    using System.Linq;
    using GfAlgorithms.WaveletTransform;
    using GfAlgorithms.WaveletTransform.SourceFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public abstract class SourceFiltersCalculatorTestsBase
    {
        protected readonly ISourceFiltersCalculator SourceFiltersCalculator;

        [UsedImplicitly]
        public static TheoryData<FieldElement[]> GetSourceFiltersVectorsParametersValidationTestCases;

        static SourceFiltersCalculatorTestsBase()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);
            var gf9 = GaloisField.Create(9, new[] { 1, 0, 1 });

            GetSourceFiltersVectorsParametersValidationTestCases
                = new TheoryData<FieldElement[]>
                  {
                      null,
                      new FieldElement[0],
                      new[] {gf2.One(), gf3.One()},
                      gf9.CreateElementsVector(5, 6, 2, 6, 2, 8, 2, 1, 0, 2, 7)
                  };
        }

        protected SourceFiltersCalculatorTestsBase(ISourceFiltersCalculator sourceFiltersCalculator)
        {
            SourceFiltersCalculator = sourceFiltersCalculator;
        }

        [Theory]
        [MemberData(nameof(GetSourceFiltersVectorsParametersValidationTestCases))]
        public void GetSourceFiltersVectorsMustValidateParameters(FieldElement[] h)
        {
            Assert.ThrowsAny<ArgumentException>(() => SourceFiltersCalculator.GetSourceFilters(h).ToArray());
        }

        [Fact]
        public void GetSourceFiltersPolynomialsMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => SourceFiltersCalculator.GetSourceFilters((Polynomial)null).ToArray());
        }

        protected void TestSourceFiltersVectorsCalculation(FieldElement[] sourceFilterH)
        {
            Assert.All(
                SourceFiltersCalculator.GetSourceFilters(sourceFilterH),
                filtersBank =>
                {
                    Assert.True(filtersBank.IsSatisfyBiorthogonalCondition());
                    Assert.True(filtersBank.CanPerformPerfectReconstruction());
                }
            );
        }

        protected void TestSourceFiltersPolynomialsCalculation(FieldElement[] sourceFilterH)
        {
            Assert.All(
                SourceFiltersCalculator.GetSourceFilters(new Polynomial(sourceFilterH), sourceFilterH.Length - 1),
                filtersBankPolynomials =>
                {
                    Assert.True(filtersBankPolynomials.CanPerformPerfectReconstruction());

                    var filtersBankVectors = filtersBankPolynomials.ToFiltersBankVectors();
                    Assert.True(filtersBankVectors.IsSatisfyBiorthogonalCondition());
                    Assert.True(filtersBankVectors.CanPerformPerfectReconstruction());
                }
            );
        }
    }
}