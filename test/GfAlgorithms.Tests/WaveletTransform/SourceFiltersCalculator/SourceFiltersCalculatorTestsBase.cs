namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.SourceFiltersCalculator
{
    using System;
    using GfAlgorithms.WaveletTransform.SourceFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Matrices;
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
            Assert.ThrowsAny<ArgumentException>(() => SourceFiltersCalculator.GetSourceFilters(h));
        }

        [Fact]
        public void GetSourceFiltersPolynomialsMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => SourceFiltersCalculator.GetSourceFilters((Polynomial)null));
        }

        private static void CheckSourceFilters(FieldElement[] hWithTilde, FieldElement[] gWithTilde, FieldElement[] h, FieldElement[] g)
        {
            var hMatrixWithTilde = FieldElementsMatrix.DoubleCirculantMatrix(hWithTilde);
            var gMatrixWithTilde = FieldElementsMatrix.DoubleCirculantMatrix(gWithTilde);
            var hMatrixTransposed = FieldElementsMatrix.DoubleCirculantMatrix(h).Transpose();
            var gMatrixTransposed = FieldElementsMatrix.DoubleCirculantMatrix(g).Transpose();

            var fullSizeIdentityMatrix = FieldElementsMatrix.IdentityMatrix(hMatrixTransposed.Field, h.Length);
            Assert.Equal(fullSizeIdentityMatrix, hMatrixTransposed * hMatrixWithTilde + gMatrixTransposed * gMatrixWithTilde);

            var halfSizeIdentityMatrix = FieldElementsMatrix.IdentityMatrix(hMatrixTransposed.Field, h.Length / 2);
            var halfSizeZeroMatrix = FieldElementsMatrix.ZeroMatrix(hMatrixTransposed.Field, h.Length / 2);
            Assert.Equal(halfSizeIdentityMatrix, hMatrixWithTilde * hMatrixTransposed);
            Assert.Equal(halfSizeIdentityMatrix, gMatrixWithTilde * gMatrixTransposed);
            Assert.Equal(halfSizeZeroMatrix, hMatrixWithTilde * gMatrixTransposed);
            Assert.Equal(halfSizeZeroMatrix, gMatrixWithTilde * hMatrixTransposed);
        }

        protected void TestSourceFiltersVectorsCalculation(FieldElement[] sourceFilterH)
        {
            var ((hWithTilde, gWithTilde), (h, g)) = SourceFiltersCalculator.GetSourceFilters(sourceFilterH);

            CheckSourceFilters(hWithTilde, gWithTilde, h, g);
        }

        protected void TestSourceFiltersPolynomialsCalculation(FieldElement[] sourceFilterH)
        {
            var (filtersLength, (hWithTilde, gWithTilde), (h, g)) = SourceFiltersCalculator.GetSourceFilters(new Polynomial(sourceFilterH), sourceFilterH.Length - 1);

            CheckSourceFilters(
                hWithTilde.GetCoefficients(filtersLength - 1),
                gWithTilde.GetCoefficients(filtersLength - 1),
                h.GetCoefficients(filtersLength - 1),
                g.GetCoefficients(filtersLength - 1)
            );
        }
    }
}