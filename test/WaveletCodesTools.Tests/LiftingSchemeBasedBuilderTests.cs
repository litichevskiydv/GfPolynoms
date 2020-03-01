namespace AppliedAlgebra.WaveletCodesTools.Tests
{
    using GeneratingPolynomialsBuilder;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class LiftingSchemeBasedBuilderTests
    {
        private readonly LiftingSchemeBasedBuilder _builder;

        [UsedImplicitly]
        public static readonly TheoryData<GeneratingPolynomialsBuilderTestCase> BuildTestsData;

        static LiftingSchemeBasedBuilderTests()
        {
            var gf8 = GaloisField.Create(8, new[] {1, 1, 0, 1});
            var gf9 = GaloisField.Create(9, new[] {1, 0, 1});
            var gf11 = GaloisField.Create(11);
            var gf13 = GaloisField.Create(13);
            var gf16 = GaloisField.Create(16, new[] {1, 0, 0, 1, 1});
            var gf17 = GaloisField.Create(17);
            var gf19 = GaloisField.Create(19);
            var gf27 = GaloisField.Create(27, new[] {2, 2, 0, 1});
            var gf32 = GaloisField.Create(32, new[] {1, 0, 0, 1, 0, 1});

            BuildTestsData =
                new TheoryData<GeneratingPolynomialsBuilderTestCase>
                {
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 7, D = 3, SourceFilter = new Polynomial(gf8, 3, 2, 7, 6, 4, 2)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 7, D = 4, SourceFilter = new Polynomial(gf8, 3, 2, 7, 6, 4, 2)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 7, D = 4, SourceFilter = new Polynomial(gf8, 0, 1, 3, 5, 7, 4, 5)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 8, D = 3, SourceFilter = new Polynomial(gf9, 1, 2, 3, 2, 2, 3, 2, 1)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 8, D = 4, SourceFilter = new Polynomial(gf9, 1, 2, 3, 2, 2, 3, 2, 1)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 10, D = 6, SourceFilter = new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 10, D = 5, SourceFilter = new Polynomial(gf11, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 12, D = 6, SourceFilter = new Polynomial(gf13, 0, 0, 0, 0, 0, 10, 5, 4, 3, 4, 11, 9)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 15, D = 7, SourceFilter = new Polynomial(gf16, 3, 2, 7, 6, 4, 2, 11, 7, 5)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 15, D = 8, SourceFilter = new Polynomial(gf16, 3, 2, 7, 6, 4, 2, 11, 7, 5)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 16, D = 8, SourceFilter = new Polynomial(gf17, 1, 13, 14, 5, 8, 9, 6, 14, 15, 8, 9, 1, 0, 5)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 16, D = 7, SourceFilter = new Polynomial(gf17, 1, 13, 14, 5, 8, 9, 6, 14, 15, 8, 9, 1, 0, 5)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 18, D = 9, SourceFilter = new Polynomial(gf19, 1, 13, 14, 5, 8, 9, 17, 15, 1, 8, 12, 5, 8)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 18, D = 8, SourceFilter = new Polynomial(gf19, 1, 13, 14, 5, 8, 9, 17, 15, 1, 8, 12, 5, 8)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 26, D = 12, SourceFilter = new Polynomial(gf27, 0, 0, 0, 1, 2, 3, 4, 1, 6, 7, 8, 9, 1, 10, 1, 12, 1, 14, 1, 17, 1, 19, 20, 1, 1, 1, 22)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 31, D = 16, SourceFilter = new Polynomial(gf32, 23, 13, 27, 1, 15, 13, 1, 16, 1, 21, 28, 30, 12, 19, 17, 4, 1, 19, 14, 0, 3, 5, 6)
                    },
                    new GeneratingPolynomialsBuilderTestCase
                    {
                        N = 31, D = 15, SourceFilter = new Polynomial(gf32, 23, 13, 27, 1, 15, 13, 1, 16, 1, 21, 28, 30, 12, 19, 17, 4, 1, 19, 14, 0, 3, 5, 6)
                    }
                };
        }

        public LiftingSchemeBasedBuilderTests()
        {
            _builder = new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), new GaussSolver());
        }

        [Theory]
        [MemberData(nameof(BuildTestsData))]
        public void ShouldBuildGeneratingPolynomial(GeneratingPolynomialsBuilderTestCase testCase)
        {
            // When
            var generatingPolynomial = _builder.Build(testCase.N, testCase.D, testCase.SourceFilter);

            // Then
            var field = testCase.SourceFilter.Field;
            var zeroValuesCount = 0;
            var nonZeroValuesCount = 0;

            var i = 0;
            var j = testCase.N - 1;
            for (; i < testCase.N && generatingPolynomial.Evaluate(field.GetGeneratingElementPower(i)) == 0; i++)
                zeroValuesCount++;
            for (; j > i && generatingPolynomial.Evaluate(field.GetGeneratingElementPower(j)) == 0; j--)
                zeroValuesCount++;
            for (; i <= j; i++)
                if (generatingPolynomial.Evaluate(field.GetGeneratingElementPower(i)) != 0)
                    nonZeroValuesCount++;

            Assert.True(
                testCase.D - 1 <= zeroValuesCount
                && (
                    testCase.N % 2 == 0 && nonZeroValuesCount >= testCase.N / 2
                    || testCase.N % 2 == 1 && nonZeroValuesCount >= (testCase.N - 1) / 2
                )
            );
        }
    }
}