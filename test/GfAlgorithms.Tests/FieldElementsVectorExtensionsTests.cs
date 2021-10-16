namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class FieldElementsVectorExtensionsTests
    {
        private static readonly GaloisField Gf3;

        [UsedImplicitly]
        public static readonly TheoryData<FieldElement[]> GetSpectrumParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<CircularShiftParametersValidationTestCase> CircularShiftParametersValidationTestCases;

        static FieldElementsVectorExtensionsTests()
        {
            var gf2 = GaloisField.Create(2);

            Gf3 = GaloisField.Create(3);
            GetSpectrumParametersValidationTestCases
                = new TheoryData<FieldElement[]>
                  {
                      null,
                      new FieldElement[0],
                      new[] {gf2.One(), null},
                      new[] {gf2.One(), Gf3.One()}
                  };
            CircularShiftParametersValidationTestCases
                = new TheoryData<CircularShiftParametersValidationTestCase>
                  {
                      new CircularShiftParametersValidationTestCase(),
                      new CircularShiftParametersValidationTestCase { Vector = gf2.CreateElementsVector(0, 1), PositionsCount = -1 },
                      new CircularShiftParametersValidationTestCase { Vector = gf2.CreateElementsVector(0, 1), PositionsCount = 2 }
                  };
        }

        [Theory]
        [MemberData(nameof(GetSpectrumParametersValidationTestCases))]
        public void GetSpectrumMustValidateParameters(FieldElement[] vector)
        {
            Assert.ThrowsAny<ArgumentException>(vector.GetSpectrum);
        }

        [Theory]
        [MemberData(nameof(CircularShiftParametersValidationTestCases))]
        public void RightCircularShiftMustValidateParameters(CircularShiftParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Vector.RightCircularShift(testCase.PositionsCount));
        }


        [Fact]
        public void ShouldPerformRightCircularShift()
        {
            // Given
            var sourceVector = Gf3.CreateElementsVector(0, 1, 2);

            // When
            var actual = sourceVector.RightCircularShift(2);

            // Then
            var expected = Gf3.CreateElementsVector(1, 2, 0);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(CircularShiftParametersValidationTestCases))]
        public void LeftCircularShiftMustValidateParameters(CircularShiftParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Vector.LeftCircularShift(testCase.PositionsCount));
        }

        [Fact]
        public void ShouldPerformLeftCircularShift()
        {
            // Given
            var sourceVector = Gf3.CreateElementsVector(0, 1, 2);

            // When
            var actual = sourceVector.LeftCircularShift(2);

            // Then
            var expected = Gf3.CreateElementsVector(2, 0, 1);
            Assert.Equal(expected, actual);
        }
    }
}