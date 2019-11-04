﻿namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using System.Collections.Generic;
    using Analyzers.CodeSpaceCovering;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NoiseGenerator;
    using Xunit;

    public class MinimalSphereCoveringAnalyzerTests
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
                return new[] { informationWord[0], informationWord[1], informationWord[0] + informationWord[1] };
            }

            public FieldElement[] Decode(FieldElement[] noisyCodeword)
            {
                return new[] { _gf2.One(), _gf2.One() };
            }

            public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
            {
                var list = new List<FieldElement[]> { new[] { noisyCodeword[0], noisyCodeword[1] } };

                if (Equals(noisyCodeword[0] + noisyCodeword[1], noisyCodeword[2]) == false || listDecodingRadius == 2)
                {
                    list.Add(new[] { noisyCodeword[0] + _gf2.One(), noisyCodeword[1] });
                    list.Add(new[] { noisyCodeword[0], noisyCodeword[1] + _gf2.One() });
                }

                if (Equals(noisyCodeword[0] + noisyCodeword[1], noisyCodeword[2]) && listDecodingRadius == 2)
                    list.Add(new[] { noisyCodeword[0] + _gf2.One(), noisyCodeword[1] + _gf2.One() });


                return list;
            }
        }

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
            var code = new FakeCode();

            // When
            var actualMinimalRadius = _analyzer.Analyze(code);

            // Then
            const int expectedMinimalRadius = 1;
            Assert.Equal(expectedMinimalRadius, actualMinimalRadius);
        }
    }
}