namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases
{
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public class CombinationsCountCalculatorTestCase
    {
        public GaloisField Field { get; set; }

        public int N { get; set; }

        public int K { get; set; }

        public FieldElement Expected { get; set; }
    }
}