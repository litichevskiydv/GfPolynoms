namespace AppliedAlgebra.WaveletCodesTools.Tests
{
    using Decoding.ListDecoderForFixedDistanceCodes;
    using Decoding.StandartDecoderForFixedDistanceCodes;
    using FixedDistanceCodesFactory;
    using GeneratingPolynomialsBuilder;
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
    using Xunit;

    public class StandardCodesFactoryTests
    {
        private static readonly StandardCodesFactory CodesFactory;

        static StandardCodesFactoryTests()
        {
            var gaussSolver = new GaussSolver();
            CodesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), gaussSolver),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()), new RrFactorizator()),
                        gaussSolver
                    )
                );
        }

        [Fact]
        public void ShouldCreateCodeFromGeneratingPolynomial()
        {
            // Given
            var generationPolynomial = new Polynomial(
                new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                0, 0, 2, 5, 6, 0, 1
            );

            // When
            var code = CodesFactory.Create(generationPolynomial);

            // Then
            Assert.Equal(7, code.CodewordLength);
            Assert.Equal(3, code.InformationWordLength);
            Assert.Equal(4, code.CodeDistance);
        }
    }
}