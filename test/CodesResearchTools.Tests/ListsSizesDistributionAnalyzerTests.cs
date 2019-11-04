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
            public int CodewordLength => 3;
            public int InformationWordLength => 2;
            public int CodeDistance => 0;

            public FieldElement[] Encode(FieldElement[] informationWord)
            {
                return new[] {informationWord[0], informationWord[1], informationWord[0] + informationWord[1]};
            }

            public FieldElement[] Decode(FieldElement[] noisyCodeword)
            {
                return new[] {_gf2.One(), _gf2.One()};
            }

            public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
            {
                var list = new List<FieldElement[]> { new[] { noisyCodeword[0], noisyCodeword[1] } };

                if (Equals(noisyCodeword[0] + noisyCodeword[1], noisyCodeword[2]) == false || listDecodingRadius == 2)
                {
                    list.Add(new[] {noisyCodeword[0] + _gf2.One(), noisyCodeword[1]});
                    list.Add(new[] {noisyCodeword[0], noisyCodeword[1] + _gf2.One()});
                }

                if(Equals(noisyCodeword[0] + noisyCodeword[1], noisyCodeword[2]) && listDecodingRadius == 2)
                    list.Add(new[] { noisyCodeword[0] + _gf2.One(), noisyCodeword[1] + _gf2.One() });


                return list;
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