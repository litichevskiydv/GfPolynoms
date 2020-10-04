namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer
{
    using System;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer;
    using Xunit;

    public class NaiveInitializerTests
    {
        private readonly NaiveInitializer _approximationVectorInitializer;

        [UsedImplicitly]
        public static readonly TheoryData<GetApproximationVectorParametersValidationTestCase> GetApproximationVectorParametersValidationTestCases;

        static NaiveInitializerTests()
        {
            var gf3 = GaloisField.Create(3);

            GetApproximationVectorParametersValidationTestCases
                = new TheoryData<GetApproximationVectorParametersValidationTestCase>
                  {
                      new GetApproximationVectorParametersValidationTestCase(),
                      new GetApproximationVectorParametersValidationTestCase
                      {
                          InformationWord = new[] {gf3.One(), gf3.One()},
                          SignalLength = 5,
                          LevelNumber = 0
                      }
                  };
        }

        public NaiveInitializerTests()
        {
            _approximationVectorInitializer = new NaiveInitializer();
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
            var informationWord = new[] {gf3.One(), gf3.Zero(), gf3.CreateElement(2)};
            const int signalLength = 24;
            const int levelNumber = 2;

            // When
            var actualApproximationVector = _approximationVectorInitializer.GetApproximationVector(informationWord, signalLength, levelNumber);

            // Then
            Assert.Equal(informationWord, actualApproximationVector);
        }
    }
}