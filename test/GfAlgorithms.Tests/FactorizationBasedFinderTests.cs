namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using System.Linq;
    using GfPolynoms.GaloisFields;
    using IrreduciblePolynomialsFinder;
    using JetBrains.Annotations;
    using PolynomialsFactorizer;
    using PolynomialsGcdFinder;
    using Xunit;

    public class FindParametersValidationTestCase
    {
        public GaloisField Field { get; set; }

        public int Degree { get; set; }
    }

    public class FactorizationBasedFinderTests
    {
        private readonly FactorizationBasedFinder _irreduciblePolynomialsFinder;

        [UsedImplicitly] 
        public static readonly TheoryData<FindParametersValidationTestCase> FindParametersValidationTestCase;

        static FactorizationBasedFinderTests()
        {
            var gf2 = GaloisField.Create(2);

            FindParametersValidationTestCase
                = new TheoryData<FindParametersValidationTestCase>
                  {
                      new FindParametersValidationTestCase {Degree = 2},
                      new FindParametersValidationTestCase {Field = gf2, Degree = -1},
                      new FindParametersValidationTestCase {Field = gf2, Degree = 1}
                  };
        }

        public FactorizationBasedFinderTests()
        {
            _irreduciblePolynomialsFinder = new FactorizationBasedFinder(new BerlekampFactorizer(new RecursiveGcdFinder()));
        }

        [Theory]
        [MemberData(nameof(FindParametersValidationTestCase))]
        public void FindMustValidateParameters(FindParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _irreduciblePolynomialsFinder.Find(testCase.Field, testCase.Degree));
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        [InlineData(3, 3)]
        [InlineData(3, 4)]
        [InlineData(5, 3)]
        public void ShouldFindIrreduciblePolynomials(int fieldOrder, int degree)
        {
            // When
            var irreduciblePolynomials = _irreduciblePolynomialsFinder.Find(GaloisField.Create(fieldOrder), degree);

            // Then
            Assert.All(irreduciblePolynomials,
                polynomial =>
                {
                    Assert.Equal(degree, polynomial.Degree);
                    Assert.NotNull(
                        GaloisField.Create(
                            (int)Math.Pow(fieldOrder, degree),
                            Enumerable.Range(0, polynomial.Degree + 1).Select(x => polynomial[x]).ToArray()
                        )
                    );
                }
            );
        }
    }
}