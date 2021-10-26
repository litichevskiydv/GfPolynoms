namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.FiltersBanksIterator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms.GaloisFields;
    using SourceFiltersCalculator;
    using VariantsIterator;

    /// <summary>
    /// Iterates filter banks that can perform perfect reconstruction
    /// </summary>
    public class PerfectReconstructionFiltersBanksIterator : IFiltersBanksIterator
    {
        private readonly IVariantsIterator _variantsIterator;
        private readonly ISourceFiltersCalculator _filtersCalculator;

        public PerfectReconstructionFiltersBanksIterator(IVariantsIterator variantsIterator, ISourceFiltersCalculator filtersCalculator)
        {
            if (variantsIterator == null)
                throw new ArgumentNullException(nameof(variantsIterator));
            if (filtersCalculator == null)
                throw new ArgumentNullException(nameof(filtersCalculator));

            _variantsIterator = variantsIterator;
            _filtersCalculator = filtersCalculator;
        }

        /// <inheritdoc/>
        public IEnumerable<FiltersBankVectors> IterateFiltersBanksVectors(
            GaloisField field,
            int filtersLength,
            FiltersBankVectors initialFiltersBank = null
        )
        {
            return _variantsIterator.IterateVectors(field, filtersLength, initialFiltersBank?.SynthesisPair.h)
                .Skip(1)
                .SelectMany(filterH => _filtersCalculator.GetSourceFilters(filterH))
                .Where(filtersBank => filtersBank.CanPerformPerfectReconstruction());
        }

        /// <inheritdoc/>
        public IEnumerable<FiltersBankPolynomials> IterateFiltersBanksPolynomials(
            GaloisField field,
            int expectedDegree,
            FiltersBankPolynomials initialFiltersBank = null
        )
        {
            return _variantsIterator.IteratePolynomials(field, expectedDegree, initialFiltersBank?.SynthesisPair.h)
                .Skip(1)
                .SelectMany(filterH => _filtersCalculator.GetSourceFilters(filterH, expectedDegree))
                .Where(filtersBank => filtersBank.CanPerformPerfectReconstruction());
        }
    }
}