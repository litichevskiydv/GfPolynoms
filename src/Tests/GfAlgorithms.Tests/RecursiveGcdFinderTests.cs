namespace GfAlgorithms.Tests
{
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsGcdFinder;
    using Xunit;

    public class RecursiveGcdFinderTests
    {
        private readonly RecursiveGcdFinder _gcdFinder;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> GcdTestData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> GcdWithQuotientsTestData;

        static RecursiveGcdFinderTests()
        {
            var gf3 = new PrimeOrderField(3);

            GcdTestData = new[]
                          {
                              new object[]
                              {
                                  new Polynomial(gf3, 0, 1, 1),
                                  new Polynomial(gf3, 1, 2, 1),
                                  new Polynomial(gf3, 2, 2)
                              },
                              new object[]
                              {
                                  new Polynomial(gf3, 0, 1, 2),
                                  new Polynomial(gf3, 1, 2, 1),
                                  new Polynomial(gf3, 1)
                              },
                              new object[]
                              {
                                  new Polynomial(gf3, 1, 1),
                                  new Polynomial(gf3, 0, 0, 1, 1),
                                  new Polynomial(gf3, 1, 1)
                              }
                          };

            GcdWithQuotientsTestData = new[]
                                       {
                                           new object[]
                                           {
                                               new Polynomial(gf3, 0, 1, 1),
                                               new Polynomial(gf3, 1, 2, 1),
                                               new Polynomial(gf3, 2, 2),
                                               new[]
                                               {
                                                   new Polynomial(gf3, 1),
                                                   new Polynomial(gf3, 2, 2)
                                               }
                                           },
                                           new object[]
                                           {
                                               new Polynomial(gf3, 0, 1, 2),
                                               new Polynomial(gf3, 1, 2, 1),
                                               new Polynomial(gf3, 1),
                                               new[]
                                               {
                                                   new Polynomial(gf3, 2),
                                                   new Polynomial(gf3, 1, 2, 1)
                                               }
                                           },
                                           new object[]
                                           {
                                               new Polynomial(gf3, 1, 1),
                                               new Polynomial(gf3, 0, 0, 1, 1),
                                               new Polynomial(gf3, 1, 1),
                                               new[]
                                               {
                                                   new Polynomial(gf3),
                                                   new Polynomial(gf3, 0, 0, 1)
                                               }
                                           }
                                       };
        }

        public RecursiveGcdFinderTests()
        {
            _gcdFinder = new RecursiveGcdFinder();
        }

        [Theory]
        [MemberData(nameof(GcdTestData))]
        public void ShouldCalculateGcd(Polynomial a, Polynomial b, Polynomial expectedGcd)
        {
            Assert.Equal(expectedGcd, _gcdFinder.Gcd(a, b));
        }

        [Theory]
        [MemberData(nameof(GcdWithQuotientsTestData))]
        public void ShouldCalculateGcdAndQuotients(Polynomial a, Polynomial b, Polynomial expectedGcd, Polynomial[] expectedQuotients)
        {
            // When
            var result = _gcdFinder.GcdWithQuotients(a, b);

            // Then
            Assert.Equal(expectedGcd, result.Gcd);
            Assert.Equal(expectedQuotients, result.Quotients);
        }

        [Theory]
        [MemberData(nameof(GcdTestData))]
        public void ShouldCalculateGcdExtendedResults(Polynomial a, Polynomial b, Polynomial expectedGcd)
        {
            // When
            var result = _gcdFinder.GcdExtended(a, b);

            //Then
            Assert.Equal(expectedGcd, result.Gcd);
            Assert.Equal(expectedGcd, a*result.X + b*result.Y);
        }
    }
}