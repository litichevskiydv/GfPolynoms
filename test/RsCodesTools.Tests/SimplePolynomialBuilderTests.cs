namespace AppliedAlgebra.RsCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using Xunit;

    public class SimplePolynomialBuilderTests
    {
        private readonly SimplePolynomialBuilder _polynomialBuilder;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> SuccessConstructionTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> FailConstructionTestsData;

        static SimplePolynomialBuilderTests()
        {
            var gf5 = new PrimeOrderField(5);
            var gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));
            var gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));
            var degreeWeight = new Tuple<int, int>(1, 2);

            SuccessConstructionTestsData = new[]
                                           {
                                               new object[]
                                               {
                                                   degreeWeight, 3,
                                                   new[]
                                                   {
                                                       new Tuple<FieldElement, FieldElement>(new FieldElement(gf5, 1), new FieldElement(gf5, 3)),
                                                       new Tuple<FieldElement, FieldElement>(new FieldElement(gf5, 2), new FieldElement(gf5, 4))
                                                   }
                                               },
                                               new object[]
                                               {
                                                   degreeWeight, 3,
                                                   new[]
                                                   {
                                                       new Tuple<FieldElement, FieldElement>(new FieldElement(gf8, 3), new FieldElement(gf8, 7)),
                                                       new Tuple<FieldElement, FieldElement>(new FieldElement(gf8, 5), new FieldElement(gf8, 4))
                                                   }
                                               },
                                               new object[]
                                               {
                                                   degreeWeight, 3,
                                                   new[]
                                                   {
                                                       new Tuple<FieldElement, FieldElement>(new FieldElement(gf27, 15), new FieldElement(gf27, 26)),
                                                       new Tuple<FieldElement, FieldElement>(new FieldElement(gf27, 10), new FieldElement(gf27, 9))
                                                   }
                                               }
                                           };

            FailConstructionTestsData = new[]
                                        {
                                            new object[]
                                            {
                                                degreeWeight, 2,
                                                new[]
                                                {
                                                    new Tuple<FieldElement, FieldElement>(new FieldElement(gf27, 1), new FieldElement(gf27, 16)),
                                                    new Tuple<FieldElement, FieldElement>(new FieldElement(gf27, 13), new FieldElement(gf27, 26)),
                                                    new Tuple<FieldElement, FieldElement>(new FieldElement(gf27, 10), new FieldElement(gf27, 15)),
                                                    new Tuple<FieldElement, FieldElement>(new FieldElement(gf27, 8), new FieldElement(gf27, 5))
                                                }
                                            }
                                        };
        }

        public SimplePolynomialBuilderTests()
        {
            _polynomialBuilder = new SimplePolynomialBuilder(new PascalsTriangleBasedCalcualtor(), new GaussSolver());
        }

        [Theory]
        [MemberData(nameof(SuccessConstructionTestsData))]
        public void ShouldConstructPolynomialWithSpecifiedSingleMultiplicityRoots(Tuple<int, int> degreeWeight, int maxWeightedDegree,
            Tuple<FieldElement, FieldElement>[] roots)
        {
            // When
            var polynomial = _polynomialBuilder.Build(degreeWeight, maxWeightedDegree, roots, 1);

            // Then
            foreach (var root in roots)
                Assert.Equal(0, polynomial.Evaluate(root.Item1, root.Item2).Representation);
        }

        [Theory]
        [MemberData(nameof(FailConstructionTestsData))]
        public void ShouldNotConstructPolynomialWithSpecifiedSingleMultiplicityRoots(Tuple<int, int> degreeWeight, int maxWeightedDegree,
            Tuple<FieldElement, FieldElement>[] roots)
        {
            Assert.Throws<NonTrivialPolynomialNotFoundException>(() => _polynomialBuilder.Build(degreeWeight, maxWeightedDegree, roots, 1));
        }
    }
}