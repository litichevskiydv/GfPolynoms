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
            var gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));
            var gf9 = new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1));
            var gf11 = new PrimeOrderField(11);
            var gf13 = new PrimeOrderField(13);
            var gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));

            BuildTestsData =
                new[]
                {
                    new object[]
                    {
                        7, 3, new Polynomial(gf8, 3, 2, 7, 6, 4, 2)
                    },
                    new object[]
                    {
                        8, 4, new Polynomial(gf9, 1, 2, 3, 2, 2, 3, 2, 1)
                    },
                    new object[]
                    {
                        10, 6, new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4)
                    },
                    new object[]
                    {
                        10, 5, new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4)
                    },
                    new object[]
                    {
                        12, 6, new Polynomial(gf13, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4, 11, 9)
                    },
                    new object[]
                    {
                        26, 12, new Polynomial(gf27, 0, 0, 0, 1, 2, 3, 4, 1, 6, 7, 8, 9, 1, 10, 1, 12, 1, 14, 1, 17, 1, 19, 20, 1, 1, 1, 22)
                    }
                };
        }

        public LiftingSchemeBasedBuilderTests()
        {
            _builder = new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), new GaussSolver());
        }

        [Theory]
        [MemberData(nameof(BuildTestsData))]
        public void ShouldBuildGeneratingPolynomial(int n, int d, Polynomial sourceFilter)
        {
            // When
            var generatingPolynomial = _builder.Build(n, d, sourceFilter);

            // Then
            var field = sourceFilter.Field;
            var zeroValuesCount = 0;
            var nonZeroValuesCount = 0;

            var i = 0;
            var j = n - 1;
            for (; i < n && generatingPolynomial.Evaluate(field.GetGeneratingElementPower(i)) == 0; i++)
                zeroValuesCount++;
            for (; j > i && generatingPolynomial.Evaluate(field.GetGeneratingElementPower(j)) == 0; j--)
                zeroValuesCount++;
            for (; i <= j; i++)
                if (generatingPolynomial.Evaluate(field.GetGeneratingElementPower(i)) != 0)
                    nonZeroValuesCount++;

            Assert.True(d - 1 <= zeroValuesCount && (n % 2 == 0 && nonZeroValuesCount >= n / 2 || n % 2 == 1 && nonZeroValuesCount >= (n - 1) / 2));
        }
    }
}