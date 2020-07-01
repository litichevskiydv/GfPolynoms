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

    public class OrthogonalSourceFiltersCalculatorTests
    {
        private readonly OrthogonalSourceFiltersCalculator _sourceFiltersCalculator;

        [UsedImplicitly]
        public static TheoryData<FieldElement[]> GetSourceFiltersVectorsParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<FieldElement[]> OrthogonalSourceFiltersCalculationTestCase;

        static OrthogonalSourceFiltersCalculatorTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);

            GetSourceFiltersVectorsParametersValidationTestCases
                = new TheoryData<FieldElement[]>
                  {
                      null,
                      new FieldElement[0],
                      new[] {gf2.One(), gf3.One()}
                  };
            OrthogonalSourceFiltersCalculationTestCase
                = new TheoryData<FieldElement[]>
                  {
                      gf2.CreateElementsVector(1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1),
                      gf3.CreateElementsVector(2, 1, 0, 0, 1, 1, 0, 0)
                  };
        }

        public OrthogonalSourceFiltersCalculatorTests()
        {
            _sourceFiltersCalculator = new OrthogonalSourceFiltersCalculator();
        }

        [Theory]
        [MemberData(nameof(GetSourceFiltersVectorsParametersValidationTestCases))]
        public void GetSourceFiltersVectorsMustValidateParameters(FieldElement[] h)
        {
            Assert.ThrowsAny<ArgumentException>(() => _sourceFiltersCalculator.GetSourceFilters(h));
        }

        [Fact]
        public void GetSourceFiltersPolynomialsMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => _sourceFiltersCalculator.GetSourceFilters((Polynomial) null));
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

        [Theory]
        [MemberData(nameof(OrthogonalSourceFiltersCalculationTestCase))]
        public void MustCalculateOrthogonalSourceFiltersVectors(FieldElement[] sourceFilterH)
        {
            // When
            var ((hWithTilde, gWithTilde), (h, g)) = _sourceFiltersCalculator.GetSourceFilters(sourceFilterH);

            // Then
            CheckSourceFilters(hWithTilde, gWithTilde, h, g);
        }

        [Theory]
        [MemberData(nameof(OrthogonalSourceFiltersCalculationTestCase))]
        public void MustCalculateOrthogonalSourceFiltersPolynomials(FieldElement[] sourceFilterH)
        {
            // When
            var (filtersLength, (hWithTilde, gWithTilde), (h, g)) = _sourceFiltersCalculator.GetSourceFilters(new Polynomial(sourceFilterH), sourceFilterH.Length - 1);

            // Then
            CheckSourceFilters(
                hWithTilde.GetCoefficients(filtersLength - 1),
                gWithTilde.GetCoefficients(filtersLength - 1),
                h.GetCoefficients(filtersLength - 1),
                g.GetCoefficients(filtersLength - 1)
            );
        }
    }
}