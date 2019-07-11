namespace AppliedAlgebra.RsCodesTools.Tests
{
    using System;
    using Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class SimplePolynomialBuilderTests
    {
        private readonly SimplePolynomialBuilder _polynomialBuilder;

        [UsedImplicitly]
        public static readonly TheoryData<InterpolationPolynomialBuilderTestCase> SuccessConstructionTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<InterpolationPolynomialBuilderTestCase> FailConstructionTestsData;

        static SimplePolynomialBuilderTests()
        {
            var gf5 = new PrimeOrderField(5);
            var gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));
            var gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));
            var degreeWeight = Tuple.Create(1, 2);

            SuccessConstructionTestsData
                = new TheoryData<InterpolationPolynomialBuilderTestCase>
                  {
                      new InterpolationPolynomialBuilderTestCase
                      {
                          DegreeWeight = degreeWeight,
                          MaxWeightedDegree = 3,
                          Roots = new[]
                                  {
                                      Tuple.Create(gf5.One(), gf5.CreateElement(3)),
                                      Tuple.Create(gf5.CreateElement(2), gf5.CreateElement(4))
                                  }
                      },
                      new InterpolationPolynomialBuilderTestCase
                      {
                          DegreeWeight = degreeWeight,
                          MaxWeightedDegree = 3,
                          Roots = new[]
                                  {
                                      Tuple.Create(gf8.CreateElement(3), gf8.CreateElement(7)),
                                      Tuple.Create(gf8.CreateElement(5), gf8.CreateElement(4))
                                  }
                      },
                      new InterpolationPolynomialBuilderTestCase
                      {
                          DegreeWeight = degreeWeight,
                          MaxWeightedDegree = 3,
                          Roots = new[]
                                  {
                                      Tuple.Create(gf27.CreateElement(15), gf27.CreateElement(26)),
                                      Tuple.Create(gf27.CreateElement(10), gf27.CreateElement(9))
                                  }
                      }
                  };

            FailConstructionTestsData
                = new TheoryData<InterpolationPolynomialBuilderTestCase>
                  {
                      new InterpolationPolynomialBuilderTestCase
                      {
                          DegreeWeight = degreeWeight,
                          MaxWeightedDegree = 2,
                          Roots = new[]
                                  {
                                      Tuple.Create(gf27.One(), gf27.CreateElement(16)),
                                      Tuple.Create(gf27.CreateElement(13), gf27.CreateElement(26)),
                                      Tuple.Create(gf27.CreateElement(10), gf27.CreateElement(15)),
                                      Tuple.Create(gf27.CreateElement(8), gf27.CreateElement(5))
                                  }
                      }
                  };
        }

        public SimplePolynomialBuilderTests()
        {
            _polynomialBuilder = new SimplePolynomialBuilder(new PascalsTriangleBasedCalculator(), new GaussSolver());
        }

        [Theory]
        [MemberData(nameof(SuccessConstructionTestsData))]
        public void ShouldConstructPolynomialWithSpecifiedSingleMultiplicityRoots(InterpolationPolynomialBuilderTestCase testCase)
        {
            // When
            var polynomial = _polynomialBuilder.Build(testCase.DegreeWeight, testCase.MaxWeightedDegree, testCase.Roots, 1);

            // Then
            foreach (var (xValue, yValue) in testCase.Roots)
                Assert.Equal(0, polynomial.Evaluate(xValue, yValue).Representation);
        }

        [Theory]
        [MemberData(nameof(FailConstructionTestsData))]
        public void ShouldNotConstructPolynomialWithSpecifiedSingleMultiplicityRoots(InterpolationPolynomialBuilderTestCase testCase)
        {
            Assert.Throws<NonTrivialPolynomialNotFoundException>(() => _polynomialBuilder.Build(testCase.DegreeWeight, testCase.MaxWeightedDegree, testCase.Roots, 1));
        }
    }
}