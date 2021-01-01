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
        public IEnumerable<FiltersBankVectors> IterateFiltersBanksVectors(GaloisField field, int filtersLength)
        {
            foreach (var filterH in _variantsIterator.IterateVectors(field, filtersLength).Skip(1))
            {
                FiltersBankVectors filtersBank;
                try
                {
                    filtersBank = _filtersCalculator.GetSourceFilters(filterH);
                }
                catch (Exception)
                {
                    continue;
                }

                if (filtersBank.CanPerformPerfectReconstruction())
                    yield return filtersBank;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FiltersBankPolynomials> IterateFiltersBanksPolynomials(GaloisField field, int expectedDegree)
        {
            foreach (var filterH in _variantsIterator.IteratePolynomials(field, expectedDegree))
            {
                FiltersBankPolynomials filtersBank;
                try
                {
                    filtersBank = _filtersCalculator.GetSourceFilters(filterH, expectedDegree);
                }
                catch (Exception)
                {
                    continue;
                }

                if (filtersBank.CanPerformPerfectReconstruction())
                    yield return filtersBank;
            }
        }
    }
}