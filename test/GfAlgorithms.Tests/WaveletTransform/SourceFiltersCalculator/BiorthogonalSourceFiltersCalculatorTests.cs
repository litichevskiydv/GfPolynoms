﻿namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.SourceFiltersCalculator
{
    using ComplementaryFilterBuilder;
    using GfAlgorithms.WaveletTransform;
    using GfAlgorithms.WaveletTransform.SourceFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsGcdFinder;
    using VariantsIterator;
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
            var gf3 = GaloisField.Create(3);
            var gf7 = GaloisField.Create(7);
            var gf8 = GaloisField.Create(8, new[] { 1, 1, 0, 1 });
            var gf9 = GaloisField.Create(9, new[] { 1, 0, 1 });

            BiorthogonalSourceFiltersOfEvenLengthCalculationTestCase
                = new TheoryData<FieldElement[]>
                  {
                      new Polynomial(gf2, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1).GetCoefficients(15),
                      new Polynomial(gf3, 2, 1, 2, 1, 1, 0, 2, 1).GetCoefficients(11),
                      new Polynomial(gf3, 1, 0, 0, 0, 0, 0, 0, 1).GetCoefficients(11),
                      new Polynomial(gf2, 1, 1, 0, 0, 0, 0, 1).GetCoefficients(7),
                      new Polynomial(gf7, 3, 2, 5, 0, 4).GetCoefficients(5),
                      new Polynomial(gf7, 4, 2, 6, 4, 3, 4).GetCoefficients(5),
                      new Polynomial(gf9, 2, 7, 5, 1, 8, 3, 2, 5).GetCoefficients(7)
                  };
            BiorthogonalSourceFiltersOfOddLengthCalculationTestCase
                = new TheoryData<FieldElement[]>
                  {
                      gf8.CreateElementsVector(3, 2, 7, 6, 4, 2, 1),
                      gf9.CreateElementsVector(5, 6, 2, 6, 2, 8, 2, 1, 0, 2, 7)
                  };
        }

        public BiorthogonalSourceFiltersCalculatorTests() : base(
            new BiorthogonalSourceFiltersCalculator(new GcdBasedBuilder(new RecursiveGcdFinder()), new RecursiveIterator())
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
            var filtersBanks = SourceFiltersCalculator.GetSourceFilters(new Polynomial(sourceFilterH), sourceFilterH.Length - 1);

            // Then
            Assert.All(filtersBanks, filterBank => Assert.True(filterBank.CanPerformPerfectReconstruction()));
        }
    }
}