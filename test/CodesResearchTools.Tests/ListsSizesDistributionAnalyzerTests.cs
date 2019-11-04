namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Analyzers.ListsSizesDistribution;
    using CodesAbstractions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NoiseGenerator;
    using Stubs;
    using TestCases;
    using Xunit;

    public class ListsSizesDistributionAnalyzerTests
    {
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
            var listSizesDistributionForRadiusOne = actualResult.Single(x => x.ListDecodingRadius == 1);
            Assert.Equal(8, listSizesDistributionForRadiusOne.ProcessedSamplesCount);
            Assert.Equal(4, listSizesDistributionForRadiusOne.ListSizesDistribution[1]);
            Assert.Equal(4, listSizesDistributionForRadiusOne.ListSizesDistribution[3]);

            var listSizesDistributionForRadiusTwo = actualResult.Single(x => x.ListDecodingRadius == 2);
            Assert.Equal(8, listSizesDistributionForRadiusTwo.ProcessedSamplesCount);
            Assert.Equal(4, listSizesDistributionForRadiusTwo.ListSizesDistribution[3]);
            Assert.Equal(4, listSizesDistributionForRadiusTwo.ListSizesDistribution[4]);

            Assert.Equal(18, _analyzer.Messages.Count);
            Assert.Equal("x1,x2,x3,list_size", _analyzer.Messages[0]);
            Assert.Equal("x1,x2,x3,list_size", _analyzer.Messages[1]);
            Assert.Contains("0,0,0,1", _analyzer.Messages);
            Assert.Contains("0,0,0,4", _analyzer.Messages);
            Assert.Contains("1,0,0,3", _analyzer.Messages);
            Assert.Contains("0,1,0,3", _analyzer.Messages);
            Assert.Contains("0,0,1,3", _analyzer.Messages);
            Assert.Contains("0,0,1,3", _analyzer.Messages);
            Assert.Contains("1,1,0,1", _analyzer.Messages);
            Assert.Contains("1,1,0,4", _analyzer.Messages);
            Assert.Contains("1,0,1,1", _analyzer.Messages);
            Assert.Contains("1,0,1,4", _analyzer.Messages);
            Assert.Contains("0,1,1,1", _analyzer.Messages);
            Assert.Contains("0,1,1,4", _analyzer.Messages);
            Assert.Contains("1,1,1,3", _analyzer.Messages);


            Assert.All(_mockLogger.Invocations, x => Assert.Equal(LogLevel.Information, x.Arguments[0]));
            Assert.Equal(4, _mockLogger.Invocations.Count);
        }
    }
}