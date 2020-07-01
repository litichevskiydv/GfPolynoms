namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.SourceFiltersCalculator
{
    using GfPolynoms;

    /// <summary>
    /// Filters bank definition in the polynomial style
    /// </summary>
    public class FiltersBankPolynomials
    {
        /// <summary>
        /// Filters length
        /// </summary>
        public int FiltersLength { get; }

        /// <summary>
        /// Analysis filters pair
        /// </summary>
        public (Polynomial hWithTilde, Polynomial gWithTilde) AnalysisPair { get; }

        /// <summary>
        /// Synthesis filters pair
        /// </summary>
        public (Polynomial h, Polynomial g) SynthesisPair { get; }

        public FiltersBankPolynomials(
            int filtersLength,
            (Polynomial hWithTilde, Polynomial gWithTilde) analysisPair,
            (Polynomial h, Polynomial g) synthesisPair
        )
        {
            FiltersLength = filtersLength;
            AnalysisPair = analysisPair;
            SynthesisPair = synthesisPair;
        }

        public void Deconstruct(
            out int filtersLength,
            out (Polynomial hWithTilde, Polynomial gWithTilde) analysisPair,
            out (Polynomial h, Polynomial g) synthesisPair
        )
        {
            filtersLength = FiltersLength;
            analysisPair = AnalysisPair;
            synthesisPair = SynthesisPair;
        }
    }
}