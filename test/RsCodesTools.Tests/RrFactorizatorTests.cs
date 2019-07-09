namespace AppliedAlgebra.RsCodesTools.Tests
{
    using System;
    using System.Linq;
    using Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using GfAlgorithms.BiVariablePolynomials;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class RrFactorizatorTests
    {
        private readonly RrFactorizator _factorizator;

        [UsedImplicitly]
        public static readonly TheoryData<InterpolationPolynomialFactorizatorTestCase> FactorizationTestsData;

        static RrFactorizatorTests()
        {
            var gf19 = new PrimeOrderField(19);
            var gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));

            FactorizationTestsData
                = new TheoryData<InterpolationPolynomialFactorizatorTestCase>
                  {
                      new InterpolationPolynomialFactorizatorTestCase
                      {
                          Polynomial = new BiVariablePolynomial(gf19)
                                       {
                                           [Tuple.Create(0, 0)] = gf19.CreateElement(4),
                                           [Tuple.Create(1, 0)] = gf19.CreateElement(12),
                                           [Tuple.Create(2, 0)] = gf19.CreateElement(5),
                                           [Tuple.Create(3, 0)] = gf19.CreateElement(11),
                                           [Tuple.Create(4, 0)] = gf19.CreateElement(8),
                                           [Tuple.Create(5, 0)] = gf19.CreateElement(13),
                                           [Tuple.Create(0, 1)] = gf19.CreateElement(14),
                                           [Tuple.Create(1, 1)] = gf19.CreateElement(14),
                                           [Tuple.Create(2, 1)] = gf19.CreateElement(9),
                                           [Tuple.Create(3, 1)] = gf19.CreateElement(16),
                                           [Tuple.Create(4, 1)] = gf19.CreateElement(8),
                                           [Tuple.Create(0, 2)] = gf19.CreateElement(14),
                                           [Tuple.Create(1, 2)] = gf19.CreateElement(13),
                                           [Tuple.Create(2, 2)] = gf19.CreateElement(1),
                                           [Tuple.Create(0, 3)] = gf19.CreateElement(2),
                                           [Tuple.Create(1, 3)] = gf19.CreateElement(11),
                                           [Tuple.Create(2, 3)] = gf19.CreateElement(1),
                                           [Tuple.Create(0, 4)] = gf19.CreateElement(17)
                                       },
                          MaxFactorDegree = 1,
                          Expected = new[]
                                     {
                                         new Polynomial(gf19, 18, 14),
                                         new Polynomial(gf19, 14, 16),
                                         new Polynomial(gf19, 8, 8)
                                     }
                      },
                      new InterpolationPolynomialFactorizatorTestCase
                      {
                          Polynomial = new BiVariablePolynomial(gf8)
                                       {
                                           [Tuple.Create(1, 1)] = gf8.One(),
                                           [Tuple.Create(0, 2)] = gf8.One()
                                       },
                          MaxFactorDegree = 1,
                          Expected = new[]
                                     {
                                         new Polynomial(gf8),
                                         new Polynomial(gf8, 0, 1)
                                     }
                      },
                      new InterpolationPolynomialFactorizatorTestCase
                      {
                          Polynomial = new BiVariablePolynomial(gf8)
                                       {
                                           [Tuple.Create(2, 1)] = gf8.One(),
                                           [Tuple.Create(0, 2)] = gf8.One()
                                       },
                          MaxFactorDegree = 2,
                          Expected = new[]
                                     {
                                         new Polynomial(gf8),
                                         new Polynomial(gf8, 0, 0, 1)
                                     }
                      }
                  };
        }

        public RrFactorizatorTests()
        {
            _factorizator = new RrFactorizator();
        }

        [Theory]
        [MemberData(nameof(FactorizationTestsData))]
        public void ShouldPerformFactorization(InterpolationPolynomialFactorizatorTestCase testCase)
        {
            // When
            var actualFactors = _factorizator.Factorize(testCase.Polynomial, testCase.MaxFactorDegree);

            // Then
            Assert.Equal(testCase.Expected.Length, actualFactors.Length);
            Assert.True(actualFactors.All(testCase.Expected.Contains));
        }
    }
}