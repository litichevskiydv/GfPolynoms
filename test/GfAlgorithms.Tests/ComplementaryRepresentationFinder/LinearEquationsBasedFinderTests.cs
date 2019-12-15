namespace AppliedAlgebra.GfAlgorithms.Tests.ComplementaryRepresentationFinder
{
    using System;
    using System.Linq;
    using Extensions;
    using GfAlgorithms.ComplementaryRepresentationFinder;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using LinearSystemSolver;
    using TestCases;
    using Xunit;

    public class LinearEquationsBasedFinderTests
    {
        private readonly LinearEquationsBasedFinder _representationFinder;

        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryRepresentationFinderTestCase> FindParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<ComplementaryRepresentationFinderTestCase> ComplementaryRepresentationSearchTestCases;

        static LinearEquationsBasedFinderTests()
        {
            var gf2 = new PrimeOrderField(2);
            var gf3 = new PrimeOrderField(3);
            var gf9 = new PrimePowerOrderField(9, new Polynomial(gf3, 1, 0, 1));
            var gf11 = new PrimeOrderField(11);

            FindParametersValidationTestCases
                = new TheoryData<ComplementaryRepresentationFinderTestCase>
                  {
                      new ComplementaryRepresentationFinderTestCase {MaxDegree = 1},
                      new ComplementaryRepresentationFinderTestCase {Polynomial = new Polynomial(gf2), MaxDegree = 0},
                      new ComplementaryRepresentationFinderTestCase {Polynomial = new Polynomial(gf2, 1, 1, 1), MaxDegree = 1},
                      new ComplementaryRepresentationFinderTestCase {Polynomial = new Polynomial(gf2, 1, 1, 1), MaxDegree = 2},
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
                          Polynomial = new Polynomial(gf9, 2, 8, 3, 8, 0, 6, 2, 7),
                          MaxDegree = 7,
                          Lambda = gf9.CreateElement(2)
                      },
                      new ComplementaryRepresentationFinderTestCase
                      {
                          Polynomial = new Polynomial(gf11, 8, 10, 4, 6, 8, 9, 2, 10, 4, 5),
                          MaxDegree = 9,
                          Lambda = gf11.CreateElement(2)
                      }
                  };
        }

        public LinearEquationsBasedFinderTests()
        {
            _representationFinder = new LinearEquationsBasedFinder(new GaussSolver());
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
            var actualComplementaryRepresentations = _representationFinder.Find(testCase.Polynomial, testCase.MaxDegree, testCase.Lambda);

            // Then
            var field = testCase.Polynomial.Field;
            var one = new Polynomial(field.One());
            var lambda = (testCase.Lambda ?? field.One()).Representation;
            var modularPolynomial
                = new Polynomial(field.One()).RightShift(testCase.MaxDegree + 1)
                  + new Polynomial(field.One().InverseForAddition());
            var componentsModularPolynomial
                = new Polynomial(field.One()).RightShift((testCase.MaxDegree + 1) / 2)
                  + new Polynomial(field.One().InverseForAddition());
            Assert.All(
                actualComplementaryRepresentations,
                x =>
                {
                    var (h, g) = x;
                    Assert.Equal(testCase.Polynomial, (h + lambda * Polynomial.RightShift(g, 2)) % modularPolynomial);

                    var (he, ho) = h.GetPolyphaseComponents();
                    var (ge, go) = g.GetPolyphaseComponents();
                    Assert.Equal(one, (he * go - ge * ho) % componentsModularPolynomial);
                }
            );
        }
    }
}