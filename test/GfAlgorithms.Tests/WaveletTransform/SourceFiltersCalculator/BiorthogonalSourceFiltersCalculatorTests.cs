namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.SourceFiltersCalculator
{
    using System;
    using ComplementaryFilterBuilder;
    using Extensions;
    using GfAlgorithms.WaveletTransform.SourceFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsGcdFinder;
    using Xunit;

    public class BiorthogonalSourceFiltersCalculatorTests : SourceFiltersCalculatorTestsBase
    {
        [UsedImplicitly]
        public static TheoryData<FieldElement[]> BiorthogonalSourceFiltersOfEvenLengthCalculationTestCase;
        [UsedImplicitly]
        public static TheoryData<FieldElement[]> BiorthogonalSourceFiltersOfOddLengthCalculationTestCase;

        static BiorthogonalSourceFiltersCalculatorTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf7 = GaloisField.Create(7);
            var gf8 = GaloisField.Create(8, new[] { 1, 1, 0, 1 });
            var gf9 = GaloisField.Create(9, new[] { 1, 0, 1 });
            var gf11 = GaloisField.Create(11);
            var gf17 = GaloisField.Create(17);

            BiorthogonalSourceFiltersOfEvenLengthCalculationTestCase
                = new TheoryData<FieldElement[]>
                  {
                      new Polynomial(gf2, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1).GetCoefficients(23),
                      new Polynomial(gf2, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1).GetCoefficients(15),
                      new Polynomial(gf2, 1, 1, 0, 0, 0, 0, 1).GetCoefficients(7),
                      new Polynomial(gf7, 3, 2, 5, 0, 4).GetCoefficients(5),
                      new Polynomial(gf7, 4, 2, 6, 4, 3, 4).GetCoefficients(5),
                      new Polynomial(gf9, 2, 7, 5, 1, 8, 3, 2, 5).GetCoefficients(7),
                      new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4).GetCoefficients(9),
                      new Polynomial(gf17, 10, 16, 5, 0, 0, 0, 0, 16).GetCoefficients(15)
                  };
            BiorthogonalSourceFiltersOfOddLengthCalculationTestCase
                = new TheoryData<FieldElement[]>
                  {
                      gf8.CreateElementsVector(3, 2, 7, 6, 4, 2, 1),
                      gf9.CreateElementsVector(5, 6, 2, 6, 2, 8, 2, 1, 0, 2, 7)
                  };
        }

        public BiorthogonalSourceFiltersCalculatorTests() : base(
            new BiorthogonalSourceFiltersCalculator(new GcdBasedBuilder(new RecursiveGcdFinder()))
        )
        {
        }

        [Theory]
        [MemberData(nameof(BiorthogonalSourceFiltersOfEvenLengthCalculationTestCase))]
        public void MustCalculateBiorthogonalSourceFiltersVectorsOfEvenLength(FieldElement[] sourceFilterH) =>
            TestSourceFiltersVectorsCalculation(sourceFilterH);

        [Theory]
        [MemberData(nameof(BiorthogonalSourceFiltersOfEvenLengthCalculationTestCase))]
        public void MustCalculateBiorthogonalSourceFiltersPolynomialsOfEvenLength(FieldElement[] sourceFilterH) =>
            TestSourceFiltersPolynomialsCalculation(sourceFilterH);

        [Theory]
        [MemberData(nameof(BiorthogonalSourceFiltersOfOddLengthCalculationTestCase))]
        public void MustCalculateBiorthogonalSourceFiltersPolynomialsOfOddLength(FieldElement[] sourceFilterH)
        {
            // When
            var (filtersLength, (hWithTilde, gWithTilde), (h, g)) = SourceFiltersCalculator.GetSourceFilters(new Polynomial(sourceFilterH), sourceFilterH.Length - 1);

            // Then
            var (hWithTildeEvenComponent, hWithTildeOddComponent) = hWithTilde.GetPolyphaseComponents();
            var (gWithTildeEvenComponent, gWithTildeOddComponent) = gWithTilde.GetPolyphaseComponents();
            var (hEvenComponent, hOddComponent) = h.GetPolyphaseComponents();
            var (gEvenComponent, gOddComponent) = g.GetPolyphaseComponents();

            var field = hEvenComponent.Field;
            var one = new Polynomial(field, 1);
            var modularPolynomial = (one >> filtersLength) - one;

            Assert.Equal(
                one,
                (
                    hEvenComponent.RaiseVariableDegree(2) * hWithTildeEvenComponent.RaiseVariableDegree(filtersLength - 1)
                    + gEvenComponent.RaiseVariableDegree(2) * gWithTildeEvenComponent.RaiseVariableDegree(filtersLength - 1)
                ) % modularPolynomial
            );
            Assert.Equal(
                one,
                (
                    hOddComponent.RaiseVariableDegree(2) * hWithTildeOddComponent.RaiseVariableDegree(filtersLength - 1)
                    + gOddComponent.RaiseVariableDegree(2) * gWithTildeOddComponent.RaiseVariableDegree(filtersLength - 1)
                ) % modularPolynomial
            );

            var zero = new Polynomial(field);
            Assert.Equal(
                zero,
                (
                    hEvenComponent.RaiseVariableDegree(2) * hWithTildeOddComponent.RaiseVariableDegree(filtersLength - 1)
                    + gEvenComponent.RaiseVariableDegree(2) * gWithTildeOddComponent.RaiseVariableDegree(filtersLength - 1)
                ) % modularPolynomial
            );
            Assert.Equal(
                zero,
                (
                    hOddComponent.RaiseVariableDegree(2) * hWithTildeEvenComponent.RaiseVariableDegree(filtersLength - 1)
                    + gOddComponent.RaiseVariableDegree(2) * gWithTildeEvenComponent.RaiseVariableDegree(filtersLength - 1)
                ) % modularPolynomial
            );
        }
    }
}