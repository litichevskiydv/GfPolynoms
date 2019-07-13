namespace AppliedAlgebra.CodesResearchTools.Tests.TestCases
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class NoisePositionsAndValuesVariationTestCase : NoiseValuesVariationTestCase
    {
        public GaloisField Field { get; set; }

        public int CodewordLength { get; set; }

        public int ErrorsCount { get; set; }
    }
}