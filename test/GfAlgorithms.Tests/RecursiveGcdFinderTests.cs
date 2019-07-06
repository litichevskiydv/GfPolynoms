namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using PolynomialsGcdFinder;
    using Xunit;

    public class RecursiveGcdFinderTests
    {
        public class GcdFinderTestCase
        {
            public Polynomial A { get; set; }

            public Polynomial B { get; set; }

            public Polynomial Expected { get; set; }
        }

        public class GcdExtendedFinderTestCase : GcdFinderTestCase
        {

            public Polynomial[] ExpectedQuotients { get; set; }
        }


        private readonly RecursiveGcdFinder _gcdFinder;

        [UsedImplicitly]
        public static readonly TheoryData<GcdFinderTestCase> GcdTestData;
        [UsedImplicitly]
        public static readonly TheoryData<GcdExtendedFinderTestCase> GcdWithQuotientsTestData;

        static RecursiveGcdFinderTests()
        {
            var gf3 = new PrimeOrderField(3);

            GcdTestData
                = new TheoryData<GcdFinderTestCase>
                  {
                      new GcdFinderTestCase
                      {
                          A = new Polynomial(gf3, 0, 1, 1),
                          B = new Polynomial(gf3, 1, 2, 1),
                          Expected = new Polynomial(gf3, 2, 2)
                      },
                      new GcdFinderTestCase
                      {
                          A = new Polynomial(gf3, 0, 1, 2),
                          B = new Polynomial(gf3, 1, 2, 1),
                          Expected = new Polynomial(gf3, 1)
                      },
                      new GcdFinderTestCase
                      {
                          A = new Polynomial(gf3, 1, 1),
                          B = new Polynomial(gf3, 0, 0, 1, 1),
                          Expected = new Polynomial(gf3, 1, 1)
                      }
                  };

            GcdWithQuotientsTestData
                = new TheoryData<GcdExtendedFinderTestCase>
                  {
                      new GcdExtendedFinderTestCase
                      {
                          A = new Polynomial(gf3, 0, 1, 1),
                          B = new Polynomial(gf3, 1, 2, 1),
                          Expected = new Polynomial(gf3, 2, 2),
                          ExpectedQuotients
                              = new[]
                                {
                                    new Polynomial(gf3, 1),
                                    new Polynomial(gf3, 2, 2)
                                }
                      },
                      new GcdExtendedFinderTestCase
                      {
                          A = new Polynomial(gf3, 0, 1, 2),
                          B = new Polynomial(gf3, 1, 2, 1),
                          Expected = new Polynomial(gf3, 1),
                          ExpectedQuotients
                              = new[]
                                {
                                    new Polynomial(gf3, 2),
                                    new Polynomial(gf3, 1, 2, 1)
                                }
                      },
                      new GcdExtendedFinderTestCase
                      {
                          A = new Polynomial(gf3, 1, 1),
                          B = new Polynomial(gf3, 0, 0, 1, 1),
                          Expected = new Polynomial(gf3, 1, 1),
                          ExpectedQuotients
                              = new[]
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
        public void ShouldCalculateGcd(GcdFinderTestCase testCase)
        {
            Assert.Equal(testCase.Expected, _gcdFinder.Gcd(testCase.A, testCase.B));
        }

        [Theory]
        [MemberData(nameof(GcdWithQuotientsTestData))]
        public void ShouldCalculateGcdAndQuotients(GcdExtendedFinderTestCase testCase)
        {
            // When
            var result = _gcdFinder.GcdWithQuotients(testCase.A, testCase.B);

            // Then
            Assert.Equal(testCase.Expected, result.Gcd);
            Assert.Equal(testCase.ExpectedQuotients, result.Quotients);
        }

        [Theory]
        [MemberData(nameof(GcdTestData))]
        public void ShouldCalculateGcdExtendedResults(GcdFinderTestCase testCase)
        {
            // When
            var result = _gcdFinder.GcdExtended(testCase.A, testCase.B);

            //Then
            Assert.Equal(testCase.Expected, result.Gcd);
            Assert.Equal(testCase.Expected, testCase.A * result.X + testCase.B * result.Y);
        }
    }
}