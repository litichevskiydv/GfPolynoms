namespace AppliedAlgebra.WaveletCodesTools.Tests
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
            var gf9 = new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1));
            var gf11 = new PrimeOrderField(11);
            var gf13 = new PrimeOrderField(13);
            var gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));

            BuildTestsData = new[]
                             {
                                 new object[]
                                 {
                                     8, 4, new Polynomial(gf9, 1, 2, 3, 1, 2, 3, 2, 1),
                                     new Polynomial(gf9, 1, 2, 7, 2, 2, 2, 1, 5, 7)
                                 },
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
                                 },
                                 new object[]
                                 {
                                     26, 12,
                                     new Polynomial(gf27, 0, 0, 0, 1, 2, 3, 4, 1, 6, 7, 8, 9, 1, 10, 1, 12, 1, 14, 1, 17, 1, 19, 20, 1, 1, 1,
                                         22),
                                     new Polynomial(gf27, 0, 0, 20, 18, 14, 15, 2, 5, 2, 19, 17, 4, 23, 1, 8, 6, 5, 4, 20, 26, 6, 5, 16, 23,
                                         26, 15, 6, 25, 18, 22, 8, 4, 17, 20, 19, 18, 8, 6, 23, 12, 20, 22, 8, 7, 0, 7, 6, 3, 11)
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