namespace AppliedAlgebra.WaveletCodesTools.Tests.FixedDistanceCodesFactory
{
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes;
    using WaveletCodesTools.FixedDistanceCodesFactory;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using Xunit;

    public class StandardCodesFactoryTests
    {
        private static readonly StandardCodesFactory CodesFactory;

        static StandardCodesFactoryTests()
        {
            var gaussSolver = new GaussSolver();
            CodesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(
                        new GcdBasedBuilder(new RecursiveGcdFinder()),
                        gaussSolver,
                        new GeneratingPolynomialsFactory()
                    ),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator()),
                        gaussSolver
                    )
                );
        }

        [Fact]
        public void ShouldCreateCodeFromGeneratingPolynomial()
        {
            // Given
            var generationPolynomial = new Polynomial(GaloisField.Create(8, new[] {1, 1, 0, 1}), 0, 0, 2, 5, 6, 0, 1);

            // When
            var code = CodesFactory.Create(generationPolynomial);

            // Then
            Assert.Equal(7, code.CodewordLength);
            Assert.Equal(3, code.InformationWordLength);
            Assert.Equal(4, code.CodeDistance);
        }
    }
}