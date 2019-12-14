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
            FindParametersValidationTestCases
                = new TheoryData<ComplementaryRepresentationFinderTestCase>
                  {
                      new ComplementaryRepresentationFinderTestCase {MaxDegree = 1},
                      new ComplementaryRepresentationFinderTestCase {Polynomial = new Polynomial(new PrimeOrderField(2)), MaxDegree = 0},
                      new ComplementaryRepresentationFinderTestCase {Polynomial = new Polynomial(new PrimeOrderField(2), 1, 1, 1), MaxDegree = 1}
                  };
            ComplementaryRepresentationSearchTestCases
                = new TheoryData<ComplementaryRepresentationFinderTestCase>
                  {
                      new ComplementaryRepresentationFinderTestCase
                      {
                          Polynomial = new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1),
                          MaxDegree = 14
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
            Assert.ThrowsAny<ArgumentException>(() => _representationFinder.Find(testCase.Polynomial, testCase.MaxDegree).ToArray());
        }

        [Theory]
        [MemberData(nameof(ComplementaryRepresentationSearchTestCases))]
        public void ShouldFindComplementaryRepresentations(ComplementaryRepresentationFinderTestCase testCase)
        {
            // When
            var actualComplementaryRepresentations = _representationFinder.Find(testCase.Polynomial, testCase.MaxDegree);

            // Then
            var field = testCase.Polynomial.Field;
            var modularPolynomial
                = new Polynomial(field.One()).RightShift(testCase.MaxDegree + 1)
                  + new Polynomial(field.One().InverseForAddition());
            Assert.All(
                actualComplementaryRepresentations,
                x => Assert.Equal(testCase.Polynomial, (x.h + x.g.RightShift(2)) % modularPolynomial)
            );
        }
    }
}