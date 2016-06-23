namespace RsCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using Xunit;

    public class RrFactorizatorTests
    {
        private readonly RrFactorizator _factorizator;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> FactorizationTestsData;

        static RrFactorizatorTests()
        {
            var gf19 = new PrimeOrderField(19);
            var gf8 = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });

            FactorizationTestsData = new[]
                                     {
                                         new object[]
                                         {
                                             new BiVariablePolynomial(gf19)
                                             {
                                                 [new Tuple<int, int>(0, 0)] = new FieldElement(gf19, 4),
                                                 [new Tuple<int, int>(1, 0)] = new FieldElement(gf19, 12),
                                                 [new Tuple<int, int>(2, 0)] = new FieldElement(gf19, 5),
                                                 [new Tuple<int, int>(3, 0)] = new FieldElement(gf19, 11),
                                                 [new Tuple<int, int>(4, 0)] = new FieldElement(gf19, 8),
                                                 [new Tuple<int, int>(5, 0)] = new FieldElement(gf19, 13),
                                                 [new Tuple<int, int>(0, 1)] = new FieldElement(gf19, 14),
                                                 [new Tuple<int, int>(1, 1)] = new FieldElement(gf19, 14),
                                                 [new Tuple<int, int>(2, 1)] = new FieldElement(gf19, 9),
                                                 [new Tuple<int, int>(3, 1)] = new FieldElement(gf19, 16),
                                                 [new Tuple<int, int>(4, 1)] = new FieldElement(gf19, 8),
                                                 [new Tuple<int, int>(0, 2)] = new FieldElement(gf19, 14),
                                                 [new Tuple<int, int>(1, 2)] = new FieldElement(gf19, 13),
                                                 [new Tuple<int, int>(2, 2)] = new FieldElement(gf19, 1),
                                                 [new Tuple<int, int>(0, 3)] = new FieldElement(gf19, 2),
                                                 [new Tuple<int, int>(1, 3)] = new FieldElement(gf19, 11),
                                                 [new Tuple<int, int>(2, 3)] = new FieldElement(gf19, 1),
                                                 [new Tuple<int, int>(0, 4)] = new FieldElement(gf19, 17)
                                             },
                                             1,
                                             new[]
                                             {
                                                 new Polynomial(gf19, 18, 14),
                                                 new Polynomial(gf19, 14, 16),
                                                 new Polynomial(gf19, 8, 8)
                                             }
                                         },
                                         new object[]
                                         {
                                             new BiVariablePolynomial(gf8)
                                             {
                                                 [new Tuple<int, int>(1, 1)] = gf8.One(),
                                                 [new Tuple<int, int>(0, 2)] = gf8.One()
                                             }, 
                                             1,
                                             new []
                                             {
                                                 new Polynomial(gf8),
                                                 new Polynomial(gf8, 0, 1) 
                                             }
                                         },
                                         new object[]
                                         {
                                             new BiVariablePolynomial(gf8)
                                             {
                                                 [new Tuple<int, int>(2, 1)] = gf8.One(),
                                                 [new Tuple<int, int>(0, 2)] = gf8.One()
                                             },
                                             2,
                                             new []
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
        public void ShouldPerformFactorization(BiVariablePolynomial polynomial, int maxFactorDegree, Polynomial[] expectedFactors)
        {
            // When
            var actualFactors = _factorizator.Factorize(polynomial, maxFactorDegree);

            // Then
            Assert.Equal(expectedFactors.Length, actualFactors.Length);
            Assert.True(actualFactors.All(expectedFactors.Contains));
        }
    }
}