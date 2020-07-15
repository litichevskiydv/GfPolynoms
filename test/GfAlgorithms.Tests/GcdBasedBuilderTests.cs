namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using ComplementaryFilterBuilder;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsGcdFinder;
    using TestCases;
    using Xunit;

    public class GcdBasedBuilderTests
    {
        private readonly GcdBasedBuilder _builder;

        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryPolynomialBuildingTestCase> ComplementaryFilterEvenLengthBuildingTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryPolynomialBuildingTestCase> ComplementaryFilterOddLengthBuildingTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryPolynomialBuildingTestCase> ComplementaryFilterBuildingFailTestsData;

        static GcdBasedBuilderTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf7 = GaloisField.Create(7);
            var gf8 = GaloisField.Create(8, new[] {1, 1, 0, 1});
            var gf9 = GaloisField.Create(9, new[] {1, 0, 1});
            var gf11 = GaloisField.Create(11);
            var gf17 = GaloisField.Create(17);

            ComplementaryFilterEvenLengthBuildingTestsData
                = new TheoryData<ComplementaryPolynomialBuildingTestCase>
                  {
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf2, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1),
                          MaxFilterLength = 24
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf2, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
                          MaxFilterLength = 16
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf2, 1, 1, 0, 0, 0, 0, 1),
                          MaxFilterLength = 8
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
                          SourceFilter = new Polynomial(gf9, 2, 7, 5, 1, 8, 3, 2, 5),
                          MaxFilterLength = 8
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
            ComplementaryFilterOddLengthBuildingTestsData
                = new TheoryData<ComplementaryPolynomialBuildingTestCase>
                  {
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf8, 3, 2, 7, 6, 4, 2, 1),
                          MaxFilterLength = 7
                      }
                  };
            ComplementaryFilterBuildingFailTestsData
                = new TheoryData<ComplementaryPolynomialBuildingTestCase>
                  {
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf7, 0, 0, 6, 6, 2, 2),
                          MaxFilterLength = 6
                      },
                      new ComplementaryPolynomialBuildingTestCase
                      {
                          SourceFilter = new Polynomial(gf8, 0, 0, 7, 6, 4, 2, 1),
                          MaxFilterLength = 7
                      }
                  };
        }

        public GcdBasedBuilderTests()
        {
            _builder = new GcdBasedBuilder(new RecursiveGcdFinder());
        }

        [Theory]
        [MemberData(nameof(ComplementaryFilterEvenLengthBuildingTestsData))]
        public void ShouldBuildComplementaryFilterEvenLength(ComplementaryPolynomialBuildingTestCase testCase)
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

        [Theory]
        [MemberData(nameof(ComplementaryFilterOddLengthBuildingTestsData))]
        public void ShouldBuildComplementaryFilterOddLength(ComplementaryPolynomialBuildingTestCase testCase)
        {
            // When
            var actualFilter = _builder.Build(testCase.SourceFilter, testCase.MaxFilterLength);

            // Then
            var field = testCase.SourceFilter.Field;
            var one = new Polynomial(field, 1);
            var modularPolynomial = (one >> testCase.MaxFilterLength) - one;
            var (he, ho) = testCase.SourceFilter.GetPolyphaseComponents();
            var (ge, go) = actualFilter.GetPolyphaseComponents();


            Assert.Equal(one, (he.RaiseVariableDegree(2) * go.RaiseVariableDegree(2) - ho.RaiseVariableDegree(2) * ge.RaiseVariableDegree(2)) % modularPolynomial);
        }

        [Theory]
        [MemberData(nameof(ComplementaryFilterBuildingFailTestsData))]
        public void ShouldNotBuildComplementaryFilter(ComplementaryPolynomialBuildingTestCase testCase)
        {
            Assert.Throws<InvalidOperationException>(() => _builder.Build(testCase.SourceFilter, testCase.MaxFilterLength));
        }
    }
}