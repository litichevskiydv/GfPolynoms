namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System.Collections.Generic;
    using ComplementaryFilterBuilder;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsGcdFinder;
    using Xunit;

    public class GcdBasedBuilderTests
    {
        public class ComplementaryPolynomialBuildingTestCase
        {
            public Polynomial SourceFilter { get; set; }

            public int MaxFilterLength { get; set; }

            public Polynomial Expected { get; set; }
        }

        private readonly GcdBasedBuilder _builder;

        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryPolynomialBuildingTestCase> BuildTestsData;

        static GcdBasedBuilderTests()
        {
            var gf2 = new PrimeOrderField(2);
            var gf7 = new PrimeOrderField(7);
            var gf11 = new PrimeOrderField(11);
            var gf17 = new PrimeOrderField(17);

            BuildTestsData
                = new TheoryData<ComplementaryPolynomialBuildingTestCase>
                  {
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf2, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1),
                          MaxFilterLength = 24,
                          Expected = new Polynomial(gf2, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1)
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf7, 3, 2, 5, 0, 4),
                          MaxFilterLength = 6,
                          Expected = new Polynomial(gf7, 3)
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf7, 4, 2, 6, 4, 3, 4),
                          MaxFilterLength = 6,
                          Expected = new Polynomial(gf7, 0, 2, 2, 5)
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4),
                          MaxFilterLength = 10,
                          Expected = new Polynomial(gf11, 0, 0, 0, 0, 0, 0, 1, 1, 10, 6)
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf17, 10, 16, 5, 0, 0, 0, 0, 16),
                          MaxFilterLength = 16,
                          Expected = new Polynomial(gf17, 12, 4, 0, 15, 0, 1)
                      }
                  };
        }

        public GcdBasedBuilderTests()
        {
            _builder = new GcdBasedBuilder(new RecursiveGcdFinder());
        }

        [Theory]
        [MemberData(nameof(BuildTestsData))]
        public void ShouldBuildComplementaryFilter(ComplementaryPolynomialBuildingTestCase testCase)
        {
            Assert.Equal(testCase.Expected, _builder.Build(testCase.SourceFilter, testCase.MaxFilterLength));
        }
    }
}