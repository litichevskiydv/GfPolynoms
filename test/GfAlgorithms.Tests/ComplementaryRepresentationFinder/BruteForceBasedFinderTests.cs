namespace AppliedAlgebra.GfAlgorithms.Tests.ComplementaryRepresentationFinder
{
    using System;
    using System.Linq;
    using GfAlgorithms.ComplementaryRepresentationFinder;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using VariantsIterator;
    using Xunit;

    public class BruteForceBasedFinderTests
    {
        private readonly BruteForceBasedFinder _representationFinder;

        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryRepresentationFinderTestCase> FindParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryRepresentationFinderTestCase> ComplementaryRepresentationSearchTestCases;

        static BruteForceBasedFinderTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);
            var gf5 = GaloisField.Create(5);
            var gf9 = GaloisField.Create(9, new[] { 1, 0, 1 });

            FindParametersValidationTestCases
                = new TheoryData<ComplementaryRepresentationFinderTestCase>
                  {
                      new ComplementaryRepresentationFinderTestCase {MaxDegree = 1},
                      new ComplementaryRepresentationFinderTestCase {Polynomial = new Polynomial(gf2), MaxDegree = 0},
                      new ComplementaryRepresentationFinderTestCase {Polynomial = new Polynomial(gf2, 1, 1, 1), MaxDegree = 1},
                      new ComplementaryRepresentationFinderTestCase
                      {
                          Polynomial = new Polynomial(gf2, 1, 1, 1),
                          MaxDegree = 2,
                          Lambda = new FieldElement(gf3, 2)
                      },
                      new ComplementaryRepresentationFinderTestCase
                      {
                          Polynomial = new Polynomial(gf9, 1, 0, 1),
                          MaxDegree = 7
                      }
                  };
            ComplementaryRepresentationSearchTestCases
                = new TheoryData<ComplementaryRepresentationFinderTestCase>
                  {
                      new ComplementaryRepresentationFinderTestCase
                      {
                          Polynomial = new Polynomial(GaloisField.Create(5), 2, 4, 3, 4),
                          MaxDegree = 3,
                          Lambda = gf5.CreateElement(2)
                      }
                  };
        }

        public BruteForceBasedFinderTests()
        {
            _representationFinder = new BruteForceBasedFinder(new RecursiveIterator());
        }

        [Theory]
        [MemberData(nameof(FindParametersValidationTestCases))]
        public void FindShouldValidateParameters(ComplementaryRepresentationFinderTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _representationFinder.Find(testCase.Polynomial, testCase.MaxDegree, testCase.Lambda).ToArray());
        }

        [Theory]
        [MemberData(nameof(ComplementaryRepresentationSearchTestCases))]
        public void ShouldFindComplementaryRepresentations(ComplementaryRepresentationFinderTestCase testCase)
        {
            // When
            var actualComplementaryRepresentations = _representationFinder.Find(testCase.Polynomial, testCase.MaxDegree, testCase.Lambda);

            // Then
            var field = testCase.Polynomial.Field;
            var one = new Polynomial(field.One());
            var modularPolynomial = (one >> (testCase.MaxDegree + 1)) - one;

            var lambda = (testCase.Lambda ?? field.One()).Representation;
            Assert.All(
                actualComplementaryRepresentations,
                x => Assert.Equal(testCase.Polynomial, (x.h + lambda * Polynomial.RightShift(x.g, 2)) % modularPolynomial)
            );
        }
    }
}