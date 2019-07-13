namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases
{
    using GfPolynoms;

    public class ComplementaryPolynomialBuildingTestCase
    {
        public Polynomial SourceFilter { get; set; }

        public int MaxFilterLength { get; set; }

        public Polynomial Expected { get; set; }
    }
}