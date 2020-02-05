namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using ComplementaryFilterBuilder;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsGcdFinder;
    using TestCases;
    using Xunit;

    public class GcdBasedBuilderTests
    {
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
                          MaxFilterLength = 24
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf7, 3, 2, 5, 0, 4),
                          MaxFilterLength = 6
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf7, 4, 2, 6, 4, 3, 4),
                          MaxFilterLength = 6
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4),
                          MaxFilterLength = 10
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf17, 10, 16, 5, 0, 0, 0, 0, 16),
                          MaxFilterLength = 16
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
            // When
            var actualFilter = _builder.Build(testCase.SourceFilter, testCase.MaxFilterLength);

            // Then
            var field = testCase.SourceFilter.Field;
            var one = new Polynomial(field, 1);
            var modularPolynomial = (one >> (testCase.MaxFilterLength / 2)) - one;
            var (he, ho) = testCase.SourceFilter.GetPolyphaseComponents();
            var (ge, go) = actualFilter.GetPolyphaseComponents();

           
            Assert.Equal(one, (he * go - ho * ge) % modularPolynomial);
        }
    }
}