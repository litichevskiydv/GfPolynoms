namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer
{
    using System;
    using System.Linq;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer;
    using Xunit;

    public class CanonicalInitializerTests
    {
        private readonly CanonicalInitializer _approximationVectorInitializer;

        [UsedImplicitly]
        public static readonly TheoryData<GetApproximationVectorParametersValidationTestCase> GetApproximationVectorParametersValidationTestCases;

        static CanonicalInitializerTests()
        {
            var gf3 = GaloisField.Create(3);

            GetApproximationVectorParametersValidationTestCases
                = new TheoryData<GetApproximationVectorParametersValidationTestCase>
                  {
                      new GetApproximationVectorParametersValidationTestCase(),
                      new GetApproximationVectorParametersValidationTestCase
                      {
                          InformationWord = new[] {gf3.One(), gf3.One(), gf3.Zero()},
                          SignalLength = 12,
                          LevelNumber = 2
                      },
                      new GetApproximationVectorParametersValidationTestCase
                      {
                          InformationWord = new[] {gf3.One(), gf3.One()},
                          SignalLength = 12,
                          LevelNumber = 1
                      }
                  };
        }

        public CanonicalInitializerTests()
        {
            _approximationVectorInitializer = new CanonicalInitializer();
        }

        [Theory]
        [MemberData(nameof(GetApproximationVectorParametersValidationTestCases))]
        public void GetApproximationVectorMustValidateParameters(GetApproximationVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _approximationVectorInitializer.GetApproximationVector(testCase.InformationWord, testCase.SignalLength, testCase.LevelNumber)
            );
        }

        [Fact]
        public void MustInitializeApproximationVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] {gf3.One(), gf3.One(), gf3.Zero(), gf3.CreateElement(2)};
            const int signalLength = 12;
            const int levelNumber = 1;

            // When
            var actualApproximationVector = _approximationVectorInitializer.GetApproximationVector(informationWord, signalLength, levelNumber);

            // Then
            var expectedApproximationVector = informationWord.Take(3).ToArray();
            Assert.Equal(expectedApproximationVector, actualApproximationVector);
        }
    }
}