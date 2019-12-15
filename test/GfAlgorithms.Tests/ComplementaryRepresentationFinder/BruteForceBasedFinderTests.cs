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
            var gf2 = new PrimeOrderField(2);
            var gf3 = new PrimeOrderField(3);
            var gf5 = new PrimeOrderField(5);

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
                      }
                  };
            ComplementaryRepresentationSearchTestCases
                = new TheoryData<ComplementaryRepresentationFinderTestCase>
                  {
                      new ComplementaryRepresentationFinderTestCase
                      {
                          Polynomial = new Polynomial(new PrimeOrderField(5), 2, 4, 3, 4),
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
            var lambda = (testCase.Lambda ?? field.One()).Representation;
            var modularPolynomial
                = new Polynomial(field.One()).RightShift(testCase.MaxDegree + 1)
                  + new Polynomial(field.One().InverseForAddition());
            Assert.All(
                actualComplementaryRepresentations,
                x => Assert.Equal(testCase.Polynomial, (x.h + lambda * Polynomial.RightShift(x.g, 2)) % modularPolynomial)
            );
        }
    }
}