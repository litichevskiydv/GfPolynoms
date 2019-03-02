namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using System;
    using Analyzers.CodeSpaceCovering;
    using GolayCodesTools;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NoiseGenerator;
    using Xunit;

    public class MinimalSphereCoveringAnalyzerTests
    {
        private readonly Mock<ILogger<MinimalSphereCoveringAnalyzer>> _mockLogger;
        private readonly MinimalSphereCoveringAnalyzer _analyzer;

        public MinimalSphereCoveringAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<MinimalSphereCoveringAnalyzer>>();
            _analyzer = new MinimalSphereCoveringAnalyzer(new RecursiveGenerator(), _mockLogger.Object);
        }

        [Fact]
        public void ShouldAnalyzeSphereCovering()
        {
            // Given
            var g11 = new G11GolayCode();

            // When
            var actualMinimalRadius = _analyzer.Analyze(g11);

            // Then
            const int expectedMinimalRadius = 2;
            Assert.Equal(expectedMinimalRadius, actualMinimalRadius);

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()
                ),
                Times.AtLeastOnce()
            );
        }
    }
}