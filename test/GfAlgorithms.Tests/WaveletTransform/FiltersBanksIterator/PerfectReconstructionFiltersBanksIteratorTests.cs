namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.FiltersBanksIterator
{
    using ComplementaryFilterBuilder;
    using GfAlgorithms.WaveletTransform;
    using GfAlgorithms.WaveletTransform.FiltersBanksIterator;
    using GfAlgorithms.WaveletTransform.SourceFiltersCalculator;
    using GfPolynoms.GaloisFields;
    using PolynomialsGcdFinder;
    using VariantsIterator;
    using Xunit;

    public class PerfectReconstructionFiltersBanksIteratorTests
    {
        private readonly PerfectReconstructionFiltersBanksIterator _filtersBanksIterator;

        public PerfectReconstructionFiltersBanksIteratorTests()
        {
            _filtersBanksIterator = new PerfectReconstructionFiltersBanksIterator(
                new RecursiveIterator(),
                new BiorthogonalSourceFiltersCalculator(new GcdBasedBuilder(new RecursiveGcdFinder()))
            );
        }

        [Fact]
        public void MustIterateFiltersBanksVectors()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            const int filtersLength = 4;

            // When
            var filtersBanks = _filtersBanksIterator.IterateFiltersBanksVectors(gf3, filtersLength);


            // Then
            var filtersBanksCount = 0;
            Assert.All(
                filtersBanks,
                filtersBank =>
                {
                    filtersBanksCount++;
                    Assert.True(filtersBank.CanPerformPerfectReconstruction());
                }
            );
            Assert.True(filtersBanksCount > 0);
        }

        [Fact]
        public void MustIterateFiltersBanksPolynomials()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            const int expectedDegree = 3;

            // When
            var filtersBanks = _filtersBanksIterator.IterateFiltersBanksPolynomials(gf3, expectedDegree);


            // Then
            var filtersBanksCount = 0;
            Assert.All(
                filtersBanks,
                filtersBank =>
                {
                    filtersBanksCount++;
                    Assert.True(filtersBank.CanPerformPerfectReconstruction());
                }
            );
            Assert.True(filtersBanksCount > 0);
        }
    }
}