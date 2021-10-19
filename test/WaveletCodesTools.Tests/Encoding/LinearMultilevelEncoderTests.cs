namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding
{
    using System;
    using System.Linq;
    using CodesAbstractions;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding;
    using WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider;
    using WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using Xunit;

    public class LinearMultilevelEncoderTests
    {
        public class ConstructorParametersValidationTestCase
        {
            public IGeneratingMatrixProvider GeneratingMatrixProvider { get; set; }

            public IInformationVectorProvider InformationVectorProvider { get; set; }

            public ICodewordMutator CodewordMutator { get; set; }

            public int LevelsCount { get; set; }
        }

        private static readonly GaloisField Gf3;

        private readonly LinearMultilevelEncoder _naiveSchemaEncoder;
        private readonly LinearMultilevelEncoder _canonicalSchemaEncoder;

        [UsedImplicitly]
        public static TheoryData<ConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<EncodeParametersValidationTestCase> EncodeParametersValidationTestCases;

        static LinearMultilevelEncoderTests()
        {
            Gf3 = GaloisField.Create(3);
            const int waveletTransformLevelsCount = 2;
            var generatingMatrixProvider = new CanonicalProvider(
                new RecursionBasedProvider(
                    new ConvolutionBasedCalculator(),
                    waveletTransformLevelsCount,
                    (
                        Gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                        Gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                    )
                )
            );

            ConstructorParametersValidationTestCases
                = new TheoryData<ConstructorParametersValidationTestCase>
                  {
                      new ConstructorParametersValidationTestCase(),
                      new ConstructorParametersValidationTestCase {GeneratingMatrixProvider = generatingMatrixProvider},
                      new ConstructorParametersValidationTestCase
                      {
                          GeneratingMatrixProvider = generatingMatrixProvider,
                          InformationVectorProvider = new LeadingZerosBasedProvider(),
                      },
                      new ConstructorParametersValidationTestCase
                      {
                          GeneratingMatrixProvider = generatingMatrixProvider,
                          InformationVectorProvider = new LeadingZerosBasedProvider(),
                          CodewordMutator = new BasicCodewordMutator(),
                          LevelsCount = -1
                      },
                      new ConstructorParametersValidationTestCase
                      {
                          GeneratingMatrixProvider = generatingMatrixProvider,
                          InformationVectorProvider = new LeadingZerosBasedProvider(),
                          CodewordMutator = new BasicCodewordMutator(),
                          LevelsCount = 50
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
                          InformationWord = Enumerable.Repeat(Gf3.Zero(), 13).ToArray()
                      },
                      new EncodeParametersValidationTestCase
                      {
                          CodewordLength = 12,
                          InformationWord = Enumerable.Repeat(Gf3.Zero(), 4).ToArray()
                      }
                  };
        }

        public LinearMultilevelEncoderTests()
        {
            const int waveletTransformLevelsCount = 2;
            const int encodingLevelsCount = 2;

            _naiveSchemaEncoder = new LinearMultilevelEncoder(
                new NaiveProvider(
                    new RecursionBasedProvider(
                        new ConvolutionBasedCalculator(),
                        waveletTransformLevelsCount,
                        (
                            Gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                            Gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                        )
                    ),
                    FieldElementsMatrix.CirculantMatrix(Gf3, 0, 0, 0, 0, 0, 1),
                    FieldElementsMatrix.CirculantMatrix(Gf3, 0, 0, 1)
                ),
                new LeadingZerosBasedProvider(),
                new BasicCodewordMutator(),
                encodingLevelsCount
            );
            _canonicalSchemaEncoder = new LinearMultilevelEncoder(
                new CanonicalProvider(
                    new RecursionBasedProvider(
                        new ConvolutionBasedCalculator(),
                        waveletTransformLevelsCount,
                        (
                            Gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                            Gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                        )
                    )
                ),
                new DetailsAbsenceBasedProvider(encodingLevelsCount),
                new BasicCodewordMutator(),
                encodingLevelsCount
            );
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void ConstructorMustValidateParameters(ConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => new LinearMultilevelEncoder(
                    testCase.GeneratingMatrixProvider,
                    testCase.InformationVectorProvider,
                    testCase.CodewordMutator,
                    testCase.LevelsCount
                )
            );
        }

        [Theory]
        [MemberData(nameof(EncodeParametersValidationTestCases))]
        public void EncodeMustValidateParameters(EncodeParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _naiveSchemaEncoder.Encode(testCase.CodewordLength, testCase.InformationWord)
            );
        }

        [Fact]
        public void MustPerformEncodingForNaiveSchema()
        {
            // Given
            var gf3 = GaloisField.Create(3);

            // When
            var actualCodeword = _naiveSchemaEncoder.Encode(12, gf3.CreateElementsVector(1, 1, 1));

            // Then
            var expectedCodeword = gf3.CreateElementsVector(1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2, 2);
            Assert.Equal(expectedCodeword, actualCodeword);
        }

        [Fact]
        public void MustPerformEncodingForCanonicalSchema()
        {
            // Given
            var gf3 = GaloisField.Create(3);

            // When
            var actualCodeword = _canonicalSchemaEncoder.Encode(12, gf3.CreateElementsVector(1, 1, 1, 1, 1, 1, 1));

            // Then
            var expectedCodeword = gf3.CreateElementsVector(1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 1, 2);
            Assert.Equal(expectedCodeword, actualCodeword);
        }

        [Fact]
        public void CanonicalSchemaMustProduceSameResultsAsClassical()
        {
            // Given
            const int sourceFiltersLength = 12;
            const int waveletTransformLevelsCount = 1;
            var h = Gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            var g = Gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            const int encodingLevelsCount = 1;
            const int codewordLength = 12;
            
            var informationWord = Gf3.CreateElementsVector(0, 1, 2, 0, 1, 2);
            var informationPolynomial = new Polynomial(informationWord);

            var multilevelEncoder = new LinearMultilevelEncoder(
                new CanonicalProvider(
                    new RecursionBasedProvider(
                        new ConvolutionBasedCalculator(),
                        waveletTransformLevelsCount,
                        (h, g)
                    )
                ),
                new RepetitionBasedProvider(),
                new BasicCodewordMutator(),
                encodingLevelsCount
            );

            var generatingPolynomial = new GeneratingPolynomialsFactory().Create(new Polynomial(h), new Polynomial(g), sourceFiltersLength - 1);
            var classicEncoder = new Encoder();

            // When
            var actual = multilevelEncoder.Encode(codewordLength, informationWord);

            // Then
            var expected = classicEncoder.Encode(codewordLength, generatingPolynomial, informationPolynomial).Select(x => x.yValue).ToArray();
            Assert.Equal(expected, actual);
        }
    }
}