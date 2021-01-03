namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator;
    using Xunit;

    public class MultilevelEncoderTests
    {
        public class MultilevelEncoderConstructorParametersValidationTestCase
        {
            public ILevelMatricesProvider LevelMatricesProvider { get; set; }

            public IWaveletCoefficientsGenerator WaveletCoefficientsGenerator { get; set; }

            public IDetailsVectorCorrector DetailsVectorCorrector { get; set; }

            public int LevelsCount { get; set; }
        }

        private readonly MultilevelEncoder _multilevelEncoder;

        [UsedImplicitly]
        public static TheoryData<MultilevelEncoderConstructorParametersValidationTestCase> MultilevelEncoderConstructorParametersValidationTestCases;
        [UsedImplicitly] 
        public static TheoryData<EncodeParametersValidationTestCase> EncodeParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<MultilevelEncodingTestCase> MultilevelEncodingTestCases;

        static MultilevelEncoderTests()
        {
            var gf3 = GaloisField.Create(3);
            const int levelsCount = 2;
            var levelMatricesProvider = new RecursionBasedProvider(
                new ConvolutionBasedCalculator(),
                levelsCount,
                (
                    gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                    gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                )
            );
            MultilevelEncoderConstructorParametersValidationTestCases
                = new TheoryData<MultilevelEncoderConstructorParametersValidationTestCase>
                  {
                      new MultilevelEncoderConstructorParametersValidationTestCase(),
                      new MultilevelEncoderConstructorParametersValidationTestCase
                      {
                          LevelMatricesProvider = levelMatricesProvider
                      },
                      new MultilevelEncoderConstructorParametersValidationTestCase
                      {
                          LevelMatricesProvider = levelMatricesProvider,
                          WaveletCoefficientsGenerator = new NaiveGenerator()
                      },
                      new MultilevelEncoderConstructorParametersValidationTestCase
                      {
                          LevelMatricesProvider = levelMatricesProvider,
                          WaveletCoefficientsGenerator = new NaiveGenerator(),
                          DetailsVectorCorrector = new DummyCorrector()
                      }
                  };
            EncodeParametersValidationTestCases
                = new TheoryData<EncodeParametersValidationTestCase>
                  {
                      new EncodeParametersValidationTestCase {CodewordLength = -1},
                      new EncodeParametersValidationTestCase {CodewordLength = 12},
                      new EncodeParametersValidationTestCase
                      {
                          CodewordLength = 12,
                          InformationWord = Enumerable.Repeat(gf3.Zero(), 13).ToArray()
                      },
                  };
            MultilevelEncodingTestCases
                = new TheoryData<MultilevelEncodingTestCase>
                  {
                      new MultilevelEncodingTestCase
                      {
                          CodewordLength = 12,
                          InformationWord = gf3.CreateElementsVector(1, 2, 2),
                          ExpectedCodeword = gf3.CreateElementsVector(1, 1, 0, 0, 2, 1, 0, 0, 2, 0, 1, 1)
                      }
                  };
        }

        public MultilevelEncoderTests()
        {
            var gf3 = GaloisField.Create(3);
            const int levelsCount = 2;
            _multilevelEncoder = new MultilevelEncoder(
                new RecursionBasedProvider(
                    new ConvolutionBasedCalculator(),
                    levelsCount,
                    (
                        gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                        gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                    )
                ),
                new NaiveGenerator(
                    FieldElementsMatrix.CirculantMatrix(gf3.CreateElementsVector(0, 0, 0, 0, 0, 1)),
                    FieldElementsMatrix.CirculantMatrix(gf3.CreateElementsVector(0, 0, 1))
                ),
                new DummyCorrector(),
                levelsCount
            );
        }

        [Theory]
        [MemberData(nameof(MultilevelEncoderConstructorParametersValidationTestCases))]
        public void MultilevelEncoderConstructorMustValidateParameters(MultilevelEncoderConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => new MultilevelEncoder(
                    testCase.LevelMatricesProvider,
                    testCase.WaveletCoefficientsGenerator,
                    testCase.DetailsVectorCorrector,
                    testCase.LevelsCount
                )
            );
        }

        [Theory]
        [MemberData(nameof(EncodeParametersValidationTestCases))]
        public void EncodeMustValidateParameters(EncodeParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _multilevelEncoder.Encode(testCase.CodewordLength, testCase.InformationWord)
            );
        }

        [Theory]
        [MemberData(nameof(MultilevelEncodingTestCases))]
        public void MustPerformMultilevelEncoding(MultilevelEncodingTestCase testCase)
        {
            // When
            var actualCodeword = _multilevelEncoder.Encode(
                testCase.CodewordLength,
                testCase.InformationWord,
                new MultilevelEncoderOptions {MaxDegreeOfParallelism = 1}
            );

            // Then
            Assert.Equal(testCase.ExpectedCodeword, actualCodeword);
        }
    }
}