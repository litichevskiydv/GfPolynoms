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
                           new[] {noisyCodeword[0], noisyCodeword[1] + _gf2.One()},
                           new[] {noisyCodeword[0] + _gf2.One(), noisyCodeword[1]}
                       };
            }
        }

        private class ListsSizesDistributionAnalyzerForTests : ListsSizesDistributionAnalyzer
        {
            private readonly List<string> _messages;

            public IReadOnlyList<string> Messages => _messages;

            public ListsSizesDistributionAnalyzerForTests(
                INoiseGenerator noiseGenerator,
                ILogger<ListsSizesDistributionAnalyzer> logger) : base(noiseGenerator, logger)
            {
                _messages = new List<string>();
            }

            internal override void WriteLineToLog(string fullLogsPath, ICode code, int listDecodingRadius, string line, bool append = true)
            {
                _messages.Add(line);
            }
        }

        private readonly Mock<ILogger<ListsSizesDistributionAnalyzer>> _mockLogger;
        private readonly ListsSizesDistributionAnalyzerForTests _analyzer;

        public ListsSizesDistributionAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<ListsSizesDistributionAnalyzer>>();
            _analyzer = new ListsSizesDistributionAnalyzerForTests(new RecursiveGenerator(), _mockLogger.Object);
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
                      LoggingResolution = 2,
                      FullLogsPath = "."
                  };

            // When
            var actualResult = _analyzer.Analyze(code, analyzerOptions);

            // Then
            var listSizesDistribution = actualResult.Single();
            Assert.Equal(4, listSizesDistribution.ProcessedSamplesCount);
            Assert.Equal(1, listSizesDistribution.ListDecodingRadius);
            Assert.Equal(4, listSizesDistribution.ListSizesDistribution[2]);

            Assert.Equal(5, _analyzer.Messages.Count);
            Assert.Equal("x1,x2,list_size", _analyzer.Messages.First());
            Assert.Contains("0,0,2", _analyzer.Messages);
            Assert.Contains("0,1,2", _analyzer.Messages);
            Assert.Contains("1,0,2", _analyzer.Messages);
            Assert.Contains("1,1,2", _analyzer.Messages);

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