namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using VariantsIterator;
    using Xunit;

    public class RecursiveIteratorTests
    {
        #region TestCases

        public class IterateVectorsParametersValidationTestCase
        {
            public GaloisField Field { get; set; }

            public int Length { get; set; }
        }

        public class IteratePolynomialsParametersValidationTestCase
        {
            public GaloisField Field { get; set; }

            public int MaxDegree { get; set; }
        }

        #endregion

        private readonly RecursiveIterator _variantsIterator;

        [UsedImplicitly]
        public static readonly TheoryData<IterateVectorsParametersValidationTestCase> IterateVectorsParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<IteratePolynomialsParametersValidationTestCase> IteratePolynomialsParametersValidationTestCases;

        static RecursiveIteratorTests()
        {
            IterateVectorsParametersValidationTestCases
                = new TheoryData<IterateVectorsParametersValidationTestCase>
                  {
                      new IterateVectorsParametersValidationTestCase {Length = 1},
                      new IterateVectorsParametersValidationTestCase {Field = GaloisField.Create(2), Length = 0}
                  };

            IteratePolynomialsParametersValidationTestCases
                = new TheoryData<IteratePolynomialsParametersValidationTestCase>
                  {
                      new IteratePolynomialsParametersValidationTestCase {MaxDegree = 0},
                      new IteratePolynomialsParametersValidationTestCase {Field = GaloisField.Create(2), MaxDegree = -1}
                  };
        }

        public RecursiveIteratorTests()
        {
            _variantsIterator = new RecursiveIterator();
        }

        [Theory]
        [MemberData(nameof(IterateVectorsParametersValidationTestCases))]
        public void IterateVectorsShouldValidateParameters(IterateVectorsParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _variantsIterator.IterateVectors(testCase.Field, testCase.Length));
        }

        [Theory]
        [MemberData(nameof(IteratePolynomialsParametersValidationTestCases))]
        public void IteratePolynomialsShouldValidateParameters(IteratePolynomialsParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _variantsIterator.IteratePolynomials(testCase.Field, testCase.MaxDegree));
        }

        [Fact]
        public void ShouldIterateVectors()
        {
            // Given
            var field = GaloisField.Create(2);
            const int length = 2;

            // When
            var actualVectors = _variantsIterator.IterateVectors(field, length).ToArray();

            // Then
            var expectedVectors
                = new[]
                  {
                      new[] {field.Zero(), field.Zero()},
                      new[] {field.One(), field.Zero()},
                      new[] {field.Zero(), field.One()},
                      new[] {field.One(), field.One()}
                  };
            actualVectors.Should().BeEquivalentTo(expectedVectors);
        }

        [Fact]
        public void ShouldIteratePolynomials()
        {
            // Given
            var field = GaloisField.Create(2);
            const int maxDegree = 1;

            // When
            var actualPolynomials = _variantsIterator.IteratePolynomials(field, maxDegree).ToArray();

            // Then
            var expectedPolynomials
                = new[]
                  {
                      new Polynomial(field, 1, 1),
                      new Polynomial(field, 0, 1),
                      new Polynomial(field, 1),
                      new Polynomial(field)

                  };
            actualPolynomials.Should().BeEquivalentTo(expectedPolynomials);
        }
    }
}