namespace WaveletCodesTools.Tests
{
    using System.Collections.Generic;
    using GeneratingPolynomialsBuilder;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class LiftingSchemeBasedBuilderTests
    {
        private readonly LiftingSchemeBasedBuilder _builder;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> BuildTestsData;

        static LiftingSchemeBasedBuilderTests()
        {
            var gf11 = new PrimeOrderField(11);
            var gf13 = new PrimeOrderField(13);

            BuildTestsData = new[]
                             {
                                 new object[]
                                 {
                                     10, 6, new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4),
                                     new Polynomial(gf11, 0, 0, 7, 3, 4, 1, 8, 1, 8, 2, 7, 5)
                                 },
                                 new object[]
                                 {
                                     10, 5, new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4),
                                     new Polynomial(gf11, 0, 0, 2, 0, 10, 9, 3, 9, 3, 10, 2, 2)
                                 },
                                 new object[]
                                 {
                                     12, 6, new Polynomial(gf13, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4, 11, 9),
                                     new Polynomial(gf13, 0, 0, 0, 8, 1, 12, 2, 11, 5, 6, 4, 2, 3, 12, 2, 4)
                                 }
                             };
        }

        public LiftingSchemeBasedBuilderTests()
        {
            _builder = new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), new GaussSolver());
        }

        [Theory]
        [MemberData(nameof(BuildTestsData))]
        public void ShouldBuildGeneratingPolynomial(int n, int d, Polynomial sourceFilter, Polynomial expectedPolynomial)
        {
            Assert.Equal(expectedPolynomial, _builder.Build(n, d, sourceFilter));
        }
    }
}