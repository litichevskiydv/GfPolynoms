namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer
{
    using System;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer;
    using Xunit;

    public class GetApproximationVectorParametersValidationTestCase
    {
        public FieldElement[] InformationWord { get; set; } 
        
        public int LevelNumber { get; set; }
    }

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
                      new GetApproximationVectorParametersValidationTestCase {InformationWord = new FieldElement[0]},
                      new GetApproximationVectorParametersValidationTestCase
                      {
                          InformationWord = new[] {gf3.One()},
                          LevelNumber = -1
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
                () => _approximationVectorInitializer.GetApproximationVector(testCase.InformationWord, testCase.LevelNumber)
            );
        }

        [Fact]
        public void MustInitializeApproximationVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] {gf3.One(), gf3.Zero(), gf3.CreateElement(2)};
            const int levelNumber = 5;

            // When
            var actualApproximationVector = _approximationVectorInitializer.GetApproximationVector(informationWord, levelNumber);

            // Then
            Assert.Equal(informationWord, actualApproximationVector);
        }
    }
}