namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using Analyzers.CodeSpaceCovering;
    using GfAlgorithms.VariantsIterator;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Stubs;
    using Xunit;

    public class MinimalSphereCoveringAnalyzerTests
    {
        private readonly Mock<ILogger<MinimalSphereCoveringAnalyzer>> _mockLogger;
        private readonly MinimalSphereCoveringAnalyzer _analyzer;

        public MinimalSphereCoveringAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<MinimalSphereCoveringAnalyzer>>();
            _analyzer = new MinimalSphereCoveringAnalyzer(new RecursiveIterator(), _mockLogger.Object);
        }

        [Fact]
        public void ShouldAnalyzeSphereCovering()
        {
            // Given
            var code = new FakeCode();

            // When
            var actualMinimalRadius = _analyzer.Analyze(code);

            // Then
            const int expectedMinimalRadius = 1;
            Assert.Equal(expectedMinimalRadius, actualMinimalRadius);
        }
    }
}