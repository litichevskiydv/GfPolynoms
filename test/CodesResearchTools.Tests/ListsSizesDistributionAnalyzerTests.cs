namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Analyzers.ListsSizesDistribution;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NoiseGenerator;
    using Xunit;

    public class ListsSizesDistributionAnalyzerTests
    {
        private class FakeCode : ICode
        {
            private readonly PrimeOrderField _gf2 = new PrimeOrderField(2);

            public GaloisField Field => _gf2;
            public int CodewordLength => 2;
            public int InformationWordLength => 2;
            public int CodeDistance => 0;

            public FieldElement[] Encode(FieldElement[] informationWord)
            {
                return informationWord.Select(x => new FieldElement(x)).ToArray();
            }

            public FieldElement[] Decode(FieldElement[] noisyCodeword)
            {
                return new[] {_gf2.One(), _gf2.One()};
            }

            public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
            {
                return new[]
                       {
                           new[] {_gf2.One(), _gf2.One()},
                           new[] {_gf2.Zero(), _gf2.Zero()}
                       };
            }
        }

        private readonly Mock<ILogger<ListsSizesDistributionAnalyzer>> _mockLogger;
        private readonly ListsSizesDistributionAnalyzer _analyzer;

        public ListsSizesDistributionAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<ListsSizesDistributionAnalyzer>>();
            _analyzer = new ListsSizesDistributionAnalyzer(new RecursiveGenerator(), _mockLogger.Object);
        }

        [Fact]
        public void ShouldAnalyzeCode()
        {
            // Given
            var code = new FakeCode();
            var analyzerOptions
                = new ListsSizesDistributionAnalyzerOptions
                  {
                      MaxDegreeOfParallelism = 1,
                      LoggingResolution = 2
                  };

            // When
            var actualResult = _analyzer.Analyze(code, analyzerOptions);

            // Then
            var listSizesDistribution = actualResult.Single();
            Assert.Equal(4, listSizesDistribution.ProcessedSamplesCount);
            Assert.Equal(1, listSizesDistribution.ListDecodingRadius);
            Assert.Equal(4, listSizesDistribution.ListSizesDistribution[2]);

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()
                ),
                Times.Exactly(2)
            );
        }
    }
}